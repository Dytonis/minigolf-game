using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class NetworkLobby : NetworkManager
{

    public Text IPConnectText;
    public Text PortConnectText;
    public Text PortStartText;

    public GameObject BasketBallPrefab;
    public GameObject BasketBallSpawn;
    public GameObject PlayerLobby;

    public List<LobbyPlayer> PlayerList = new List<LobbyPlayer>();

    public LobbyPlayer LocalPlayer;

    private UIScreens Screens;

    public void NLStartAsHost()
    {
        SetStartPort();
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

        Screens.SwitchScreen(4);
    }

    public void SetScreens(UIScreens screens)
    {
        this.Screens = screens;
    }
}
