using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.UI;
using System;
using System.Linq;

public class NetworkLobby : NetworkManager
{

    public Text IPConnectText;
    public Text PortConnectText;
    public Text PortStartText;

    public GameObject BallPlayerPrefab;
    public GameObject BasketBallPrefab;
    public GameObject BasketBallSpawn;
    public GameObject PlayerLobby;

    public List<LobbyPlayer> PlayerList = new List<LobbyPlayer>();

    public LobbyPlayer LocalPlayer;

    [HideInInspector]
    public UIScreens Screens;

    public bool Networked = true;

    public GameServer gameManager;

    public void NLChangeLevel(string level)
    {
        ServerChangeScene(level);
    }

    public void NLStartAsHost()
    {
        SetStartPort();
        singleton.StartHost();
    }

    public void NLStartSingleplayer()
    {
        Networked = false;
        singleton.networkPort = 7777;
        singleton.StartHost();
    }

    public void NLConnect()
    {
        SetConnectPort();
        SetConnectIP();
        singleton.StartClient();
    }

    public void SetConnectPort()
    {
        try
        {
            singleton.networkPort = Convert.ToInt16(PortConnectText.text);
        }
        catch
        {

        }
    }
    public void SetStartPort()
    {
        try
        {
            singleton.networkPort = Convert.ToInt16(PortStartText.text);
        }
        catch
        {

        }
    }

    public void SetConnectIP()
    {
        singleton.networkAddress = IPConnectText.text;
    }

    public override void OnServerConnect(NetworkConnection conn)
    {
        base.OnServerConnect(conn);

        if (conn.address == "localClient" || conn.address == "localServer")
            return;

        print("got connect " + conn.address);
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);

        if(Networked)
            Screens.SwitchScreen(4);
    }

    public void SetScreens(UIScreens screens)
    {
        this.Screens = screens;
    }

    public void GeneratePlayerList()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        PlayerList = players.Select(x => x.GetComponent<LobbyPlayer>()).ToList();
        //FixPlayerMetadataToObjects();
    }

    public void FixPlayerMetadataToObjects()
    {
        foreach(LobbyPlayer p in PlayerList)
        {
            p.gameObject.name = p.meta.name;
        }
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        base.OnServerDisconnect(conn);

        print("server disconnect client " + conn.address);
    }
}
