using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using JAMGG.Net;

public class LobbyPlayer : NetworkBehaviour
{
    public NetworkLobby lobby;
    [SerializeField, SyncVar]
    public PlayerMetadata meta;

    private int spawnMultiplayerBallQueue = 0;

    public void Awake()
    {
        lobby = GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<NetworkLobby>();
        meta = new JAMGG.Net.PlayerMetadata { name = "Lobby Player " + transform.GetInstanceID().ToString(), ID = this.GetInstanceID() };
        DontDestroyOnLoad(transform);
    }

    public override void OnStartLocalPlayer()
    { 
        print("local player start");

        lobby.LocalPlayer = this;

        CmdPlayerJoined(meta);
    }

    private float time;
    public void Update()
    {
        if (!isLocalPlayer)
            return;

        if (spawnMultiplayerBallQueue > 0)
        {
            time += Time.deltaTime;

            if(time >= 0.25f)
            {
                GameObject ball = Instantiate(lobby.BasketBallPrefab) as GameObject;
                ball.transform.position = new Vector3(lobby.BasketBallSpawn.transform.position.x + Random.Range(-0.15f, 0.15f),
                    lobby.BasketBallSpawn.transform.position.y, lobby.BasketBallSpawn.transform.position.z + Random.Range(-0.15f, 0.15f));
                time = 0;
                spawnMultiplayerBallQueue--;
            }
        }
        else
        {
            time = 0;
        }
    }

    public void HandleLevelChange()
    {
        if(isServer)
        {
            RpcChangeLevel();
        }
        else
        {
            CmdPrintServer("Non-server start game attempt blocked.");
        }
    }
    
    [Command]
    public void CmdPlayerJoined(JAMGG.Net.PlayerMetadata connect)
    {
        //called on the connecting client

        lobby.GeneratePlayerList();

        LobbyConnectionPacket packet = new LobbyConnectionPacket
        {
            OtherMetadatas = lobby.PlayerList.Select(x => x.meta).ToArray(),
            LobbyInfo = new LobbyUpdateGeneralPacket
            {
                LobbyName = lobby.LocalPlayer.meta.name + "\'s Game",
                MapSelected = "NOT IMPLEMENTED"
            }
        };

        RpcRecievePlayerConnection(connect, packet);
    }

    [Command]
    public void CmdPrintServer(string data)
    {
        print(data);
    }

    [ClientRpc]
    public void RpcChangeLevel()
    {
        SceneManager.LoadScene("LoadingGlen");
    }

    [ClientRpc]
    public void RpcRecievePlayerConnection(JAMGG.Net.PlayerMetadata connect, JAMGG.Net.LobbyConnectionPacket lobbyInfo)
    {
        transform.name = connect.name;

        if (!isLocalPlayer)
        {
            //IF A DIFFERENT CLIENT CONNECTS

            print("got connection name " + connect.name);

            lobby.LocalPlayer.spawnMultiplayerBallQueue++;
            lobby.Screens.Playernames.text = "";

            foreach (PlayerMetadata m in lobbyInfo.OtherMetadatas)
            {
                lobby.Screens.Playernames.text += m.name + "\n";
            }
        }
        else
        {
            //you are connecting
            print("connecting with name " + connect.name);

            lobby.Screens.Lobbyname.text = lobbyInfo.LobbyInfo.LobbyName;
            lobby.Screens.MapName.text = lobbyInfo.LobbyInfo.MapSelected;
            lobby.Screens.Playernames.text = "";

            print("lobby hook " + lobbyInfo.LobbyInfo.LobbyName);

            foreach (PlayerMetadata m in lobbyInfo.OtherMetadatas)
            {
                print("metadata hook " + m.name);

                lobby.Screens.Playernames.text += m.name + "\n";

                if (m.ID == meta.ID)
                    continue;

                lobby.LocalPlayer.spawnMultiplayerBallQueue++;
            }
        }
    }
}
