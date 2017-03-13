﻿using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Networking;
using System.Linq;

public class GameServer : NetworkBehaviour
{
    public PlayerControllerNetwork LocalPlayer;

    public List<GameObject> PlayerList = new List<GameObject>();
    public List<NamePacket> PlayerNamePackets = new List<NamePacket>();

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

    public void FinishedPlayersChanged(int change)
    {
        FinishedPlayersPerHole = change;

        GeneratePlayerList();

        if(change >= PlayerList.Count)
        {
            LocalPlayer.GetComponent<PlayerControllerNetwork>().TriggerNextHole();            
        }
    }

    public void GeneratePlayerList()
    {
        PlayerList = GameObject.FindGameObjectsWithTag("Player").ToList();

        foreach(GameObject obj in PlayerList)
        {
            PlayerNamePackets.Add(new NamePacket() { id = obj.GetComponent<NetworkIdentity>().netId.Value, name = obj.name });
        }
    }

    public void Starta()
    {
        for(int i = 0; i < HoleCount; i++)
        {
            HoleSpawns.Add(GameObject.FindGameObjectWithTag("Spawn" + i));
        }
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
