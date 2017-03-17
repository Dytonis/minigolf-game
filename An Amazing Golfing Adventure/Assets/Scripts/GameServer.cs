using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Networking;
using System.Linq;

public class GameServer : NetworkBehaviour
{
    [SyncVar(hook = "FinishedPlayersChanged")]
    public int FinishedPlayersPerHole;

    [SyncVar]
    public int HoleNumber;

    [SyncVar]
    public bool GameStarted;

    public int HoleCount;

    [SyncVar]
    public int PlayerCount;

    public List<GameObject> HoleSpawns = new List<GameObject>();

    public int ExpectingPlayers;

    public NetworkLobby lobby;

    public void FinishedPlayersChanged(int change)
    {
        FinishedPlayersPerHole = change;

        if(change >= lobby.PlayerList.Count)
        {
            //lobby.LocalPlayer.GetComponent<PlayerControllerNetwork>().TriggerNextHole();            
        }
    }

    public void Starta()
    {
        for(int i = 0; i < HoleCount; i++)
        {
            HoleSpawns.Add(GameObject.FindGameObjectWithTag("Spawn" + i));
        }
    }

    public void Start()
    {
        lobby = GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<NetworkLobby>();

        print("awake gameserver");

        lobby.gameManager = this;
        lobby.LocalPlayer.LocalGameStartedInit();
    }

    [ClientRpc]
    public void RpcResetHole(GameObject player)
    {
       player.transform.position = HoleSpawns[HoleNumber].transform.position;
    }

    [ClientRpc]
    public void RpcResetPosition(GameObject player, Vector3 LRTS)
    {
        player.transform.localPosition = LRTS;
    }
}

public struct NamePacket
{
    public string name;
    public uint id;
}
