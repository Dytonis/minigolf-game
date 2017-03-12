using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Networking;
using System.Linq;

public class GameServer : NetworkBehaviour
{
    public List<GameObject> PlayerList = new List<GameObject>();

    [SyncVar(hook = "FinishedPlayersChanged")]
    public int FinishedPlayersPerHole;

    [SyncVar]
    public int HoleNumber;

    public int HoleCount;

    [SyncVar]
    public int PlayerCount;

    public List<GameObject> HoleSpawns = new List<GameObject>();

    public int ExpectingPlayers;

    public void FinishedPlayersChanged(int change)
    {
        FinishedPlayersPerHole = change;

        GeneratePlayerList();

        if(FinishedPlayersPerHole >= PlayerList.Count)
        {
            foreach(GameObject p in PlayerList)
            {
                p.GetComponent<PlayerControllerNetwork>().TriggerNextHole();
            }
        }
    }

    public void GeneratePlayerList()
    {
        PlayerList = GameObject.FindGameObjectsWithTag("Player").ToList();
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
