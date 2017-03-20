using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using JAMGG.Net;
using System.Collections;

public class LobbyPlayer : NetworkBehaviour
{
    public GameServer game;
    public NetworkLobby lobby;
    public NetworkSync sync;
    [SerializeField, SyncVar]
    public PlayerMetadata meta; //will be the same for every client. server must assign this

    private int spawnMultiplayerBallQueue = 0;

    public PlayerController ControlledBall;

    public bool Ready = false;

    public bool isPlayer;


    public void Awake()
    {
        lobby = GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<NetworkLobby>();
        meta = new JAMGG.Net.PlayerMetadata { name = "Lobby Player " + transform.GetInstanceID().ToString(), ID = this.GetInstanceID() };
        sync = GetComponent<NetworkSync>();
        DontDestroyOnLoad(transform);
    }

    public void LocalGameStartedInit()
    {
        //called when GameServer starts the game on the local player's ball (locally, use OfficialGameStartedInit for when the multiplayer match begins)

        print("start co from " + meta.name + " gm: " + lobby.gameManager);

        lobby.GeneratePlayerList();

        foreach(LobbyPlayer p in lobby.PlayerList)
        {
            p.ControlledBall = (Instantiate(lobby.BallPlayerPrefab) as GameObject).GetComponent<PlayerRoot>().ball;
            p.GetComponent<NetworkSync>().SetupSync(p.ControlledBall.GetComponent<Rigidbody>(), p.ControlledBall.transform.parent.GetComponent<PlayerRoot>());
            p.ControlledBall.Master = p;
            p.game = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameServer>();
        }

        ControlledBall.IsClientControlled = true;

        StartCoroutine(WaitToReady());
    }

    IEnumerator WaitToReady()
    {
        yield return new WaitForSeconds(2.5f);

        GameObject cam = GameObject.FindGameObjectWithTag("SceneCamera");
        print(cam + " from " + meta.name);
        cam.SetActive(false);
        CmdPlayerSetReady(true);
    }

    IEnumerator WaitToChangeHole(int hole)
    {
        ControlledBall.HaltBackupResetForEvent = true;

        yield return new WaitForSeconds(1f);

        ControlledBall.ShowScorecardForce();

        yield return new WaitForSeconds(6f);

        ControlledBall.HideScorecard();
        ControlledBall.MoveBallToHole(hole);
        ControlledBall.HaltBackupResetForEvent = false;
        game.ResetFinishedCounter();
    }

    public override void OnStartLocalPlayer()
    { 
        print("local player start");

        lobby.LocalPlayer = this;

        isPlayer = true;

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
            //NetworkServer.SetAllClientsNotReady();
            lobby.NLChangeLevel("LoadingGlen");
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

        lobby.LocalPlayer.RpcRecievePlayerConnection(connect, packet);
    }

    [Command]
    public void CmdPrintServer(string data)
    {
        print(data);
    }

    [Command]
    public void CmdPlayerSetReady(bool ready)
    {
        Ready = ready;

        foreach(LobbyPlayer p in lobby.PlayerList)
        {
            if (p.Ready == false)
                return;
        }

        game.HoleNumber = 1;
        RpcOfficialGameStartedInit();
    }

    [Command]
    public void CmdSetClientFinishedHole(int playerID, int strokes)
    {
        RpcGotOtherPlayerFinishedHole(playerID, game.HoleNumber-1, strokes);
    }

    [Command]
    public void CmdFinishedHole()
    {
        game.FinishedPlayersPerHole++;
    }

    [ClientRpc, System.Obsolete]
    public void RpcChangeLevel()
    {
        //tell the serve we are not ready
        //CmdSetReadyState()

        SceneManager.LoadScene("LoadingGlen");
    }

    [ClientRpc]
    public void RpcRecievePlayerConnection(JAMGG.Net.PlayerMetadata connect, JAMGG.Net.LobbyConnectionPacket lobbyInfo)
    {
        transform.name = connect.name;

        if (!lobby.Networked) //return if singleplayer
            return;

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

            foreach (PlayerMetadata m in lobbyInfo.OtherMetadatas)
            {
                lobby.Screens.Playernames.text += m.name + "\n";

                if (m.ID == meta.ID)
                    continue;

                lobby.LocalPlayer.spawnMultiplayerBallQueue++;
            }
        }

        lobby.GeneratePlayerList();

        if (ControlledBall != null)
        {
            ControlledBall.Scorecard.UpdatePlayers(lobby.PlayerList.Select(x => x.meta).ToArray());
        }
    }

    [ClientRpc]
    public void RpcOfficialGameStartedInit()
    {
        //called on host's ball!

        print("CLIENT + " + meta.name + " Recieve Command: RpcNextHole");
        print(lobby.gameManager);

        lobby.GeneratePlayerList();
        ControlledBall.Scorecard.Initialize(lobby.PlayerList.Select(x => x.meta).ToArray());

        lobby.gameManager.FinishedPlayersPerHole = 0;
        foreach (LobbyPlayer p in lobby.PlayerList)
        {
            print("moving player " + p.transform.GetInstanceID());
            //move all objects for client
            p.ControlledBall.transform.parent.position = lobby.gameManager.HoleSpawns[1].transform.position;

            p.ControlledBall.transform.localPosition = Vector3.zero;
            p.ControlledBall.GetComponent<Rigidbody>().velocity = Vector3.zero;
            p.ControlledBall.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        }
    }

    [ClientRpc]
    public void RpcGotChangeHole(int hole)
    {
        StartCoroutine(WaitToChangeHole(hole));
    }

    [ClientRpc]
    public void RpcGotOtherPlayerFinishedHole(int playerID, int hole, int strokes)
    {
        //ran on the finished ball. we need to run this on the local player.

        lobby.LocalPlayer.ControlledBall.Scorecard.ApplyScores(hole, playerID, strokes);
    }

    [Command]
    public void CmdHoleFinishedAllReady()
    {
        //ran on the host's ball
        
        print("hole finished all ready");

        game.HoleNumber++;

        foreach (LobbyPlayer p in lobby.PlayerList) //run the rpc on all balls
            p.RpcGotChangeHole(game.HoleNumber);
    }

    public void WaitingForOthers()
    {
        if (isLocalPlayer)
        {
            ControlledBall.HaltBackupResetForEvent = true;
        }
    }

    public void LocalClientFinishedHole()
    {
        if (isLocalPlayer)
        {
            CmdSetClientFinishedHole(meta.ID, ControlledBall.strokes);
            ControlledBall.strokes = 0;
            CmdFinishedHole();
        }
    }
}
