using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Networking;
using System.Linq;

public class PlayerControllerNetwork : NetworkBehaviour
{
    public bool IsClientControlled = true;
    public PlayerControllerNetworkBall ChildBall;

    public GameObject PowerIndicator;
    public GameObject XIndicator;
    public CameraOrbit Orbiter;
    public float PowerLimit;

    public float Power;

    public bool Playable;
    public float PlayableSeconds;
    public float StationaryUnplayableTime;

    public Vector3 LastPositionRTS;

    public GameServer server;
    public UIScoreboard Scoreboard;

    public void Awake()
    {
        server = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameServer>();
        Scoreboard = GameObject.FindGameObjectWithTag("UIScoreboard").GetComponent<UIScoreboard>();
        ChildBall.network = this;
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        gameObject.name = gameObject.GetInstanceID().ToString();
    }

    public override void OnStartLocalPlayer()
    {
        if (isLocalPlayer)
        {
            XIndicator.SetActive(true);
            print("LOCALPLAYER");
            IsClientControlled = true;
            ChildBall.IsClientControlled = true;
            //ChildBall.game = game;
            CmdJoin(gameObject);
            server.LocalPlayer = this;
            server.GeneratePlayerList();

            GameObject.FindGameObjectWithTag("SceneCamera").SetActive(false);

            List<string> playerNames = new List<string>();

            foreach(GameObject obj in server.PlayerList)
            {
                playerNames.Add(obj.name);
            }

            Scoreboard.Initialize(playerNames.ToArray());
            //Scoreboard.Initialize(new string[] { "test1", "test2", "test3" });
        }
    }

    public void UpdateScoreboardPlayerlist()
    {
        server.GeneratePlayerList();

        List<string> playerNames = new List<string>();

        foreach (GameObject obj in server.PlayerList)
        {
            playerNames.Add(obj.name);
        }

        Scoreboard.UpdatePlayers(playerNames.ToArray());
        //Scoreboard.UpdatePlayers(new string[] { "test1", "test2", "test3" });
    }

    [Command]
    public void CmdReadyPlayerHole()
    {
        server.FinishedPlayersPerHole++;
    }

    [Command]
    public void CmdJoin(GameObject player)
    {
        //RUNNING ON SERVER
        print("SERVER Recive Command Attempt: CmdJoin");

        if (!isServer)
            return;

        print("SERVER Recive Command: CmdJoin");

        server.PlayerCount++;

        server.GeneratePlayerList();

        RpcGotPlayerConnection(player.name, server.PlayerNamePackets.ToArray());

        print("server players " + server.PlayerCount);

        if (server.PlayerCount >= server.ExpectingPlayers)
        {
            server.HoleNumber = 1;
            //Send move all command to all clients' new connecting object

            if (!server.GameStarted)
            {
                server.GameStarted = true;
                RpcMoveAllToHole(1); //move all players to new hole
            }
            else
            {
                //game already started
                RpcSpawnCurrentHole(); //move current player to current hole
            }
        }
    }

    public void TriggerNextHole()
    {
        //ran on server!
        if(!isServer)
        {
            return;
        }

        //server authority
        server.FinishedPlayersPerHole = 0;

        if (server.HoleNumber < server.HoleSpawns.Count - 1)
        {
            server.HoleNumber++;
            RpcMoveAllToHole(server.HoleNumber);
        }
        else
            RpcSpawnAllCurrentHole();
    }

    [Command]
    public void CmdUpdateScore(string playerName, int hole, int score)
    {
        //server send command to clients
        RpcGotUpdatedScore(playerName, hole, score);
    }

    [ClientRpc]
    public void RpcGotUpdatedScore(string playerName, int hole, int score)
    {
        //server has told us to update the score. the args are the updated data
        //this will not be our local player - this is the player who recently holed. we need to get our player

    }

    [ClientRpc]
    public void RpcGotPlayerConnection(string playerName, NamePacket[] otherPlayersProvided)
    {
        try
        {
            print("CLIENT Recieve Command: RpcGotPlayerConnection " + playerName + ", " + otherPlayersProvided[0] + " " + otherPlayersProvided[1]);
        }
        catch { }
        //this is ran on the connecting player - we have to get the local player.
        gameObject.name = playerName;

        server.GeneratePlayerList();
        
        //assign names to the matching objects recieved from the server.
        foreach(GameObject obj in server.PlayerList)
        {
            foreach(NamePacket packet in otherPlayersProvided)
            {
                if(obj.GetComponent<NetworkIdentity>().netId.Value == packet.id)
                {
                    obj.name = packet.name;
                }
            }
        }

        server.LocalPlayer.UpdateScoreboardPlayerlist();
    }

    [ClientRpc]
    public void RpcMoveAllToHole(int holeNumber)
    {
        print("CLIENT Recieve Command: RpcNextHole");

        server.FinishedPlayersPerHole = 0;
        server.GeneratePlayerList();
        print(server.PlayerList);
        foreach (GameObject p in server.PlayerList)
        {
            print("moving player " + p.transform.GetInstanceID());
            //move all objects for client
            p.transform.position = server.HoleSpawns[holeNumber].transform.position;

            PlayerControllerNetwork pN = p.GetComponent<PlayerControllerNetwork>();

            pN.ChildBall.transform.localPosition = Vector3.zero;
            pN.ChildBall.GetComponent<Rigidbody>().velocity = Vector3.zero;
            pN.ChildBall.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        }
    }

    [ClientRpc]
    public void RpcSpawnAllCurrentHole()
    {
        print("CLIENT Recieve Command: RpcNextHole");
        print(gameObject.GetInstanceID());

        print(server.HoleSpawns.Count + " > " + (server.HoleNumber));
        if (server.HoleSpawns.Count > (server.HoleNumber))
        {
            server.GeneratePlayerList();
            print(server.PlayerList);
            foreach (GameObject p in server.PlayerList)
            {
                print("moving player " + p.transform.GetInstanceID());
                //move all objects for client
                p.transform.position = server.HoleSpawns[server.HoleNumber].transform.position;

                PlayerControllerNetwork pN = p.GetComponent<PlayerControllerNetwork>();

                pN.ChildBall.transform.localPosition = Vector3.zero;
                pN.ChildBall.GetComponent<Rigidbody>().velocity = Vector3.zero;
                pN.ChildBall.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            }
        }
    }

    [ClientRpc]
    public void RpcSpawnCurrentHole()
    {
        print("CLIENT Recieve Command: RpcNextHole");

        print(server.HoleSpawns.Count + " > " + (server.HoleNumber));
        if (server.HoleSpawns.Count > (server.HoleNumber))
        {
            server.GeneratePlayerList();
            print(server.PlayerList);

            print("moving player " + gameObject.name);
            //move all objects for client
            transform.position = server.HoleSpawns[server.HoleNumber].transform.position;

            ChildBall.transform.localPosition = Vector3.zero;
            ChildBall.GetComponent<Rigidbody>().velocity = Vector3.zero;
            ChildBall.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
           
        }
    }

    void Update()
    {
        if(!IsClientControlled)
        {
            return;
        }

        if(Input.GetKeyDown(KeyCode.Tab))
        {
            Scoreboard.gameObject.SetActive(true);
        }
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            Scoreboard.gameObject.SetActive(false);
        }

        RaycastHit info;
        if (IsGrounded(out info))
        {
            if (info.collider.gameObject.tag == "Carpet" || info.collider.gameObject.tag == "Brick" || info.collider.gameObject.tag == "Concrete")
            {
                if (IsStationary())
                    Playable = true;
                else
                    Playable = false;
            }
            else
            {
                Playable = false;
            }
        }
        else
        {
            Playable = false;
        }

        if (Input.GetMouseButton(0))
        {
            Orbiter.EnabledY = false;
        }

        if (Playable)
        {
            StationaryUnplayableTime = 0;
            PlayableSeconds += Time.deltaTime;
            if (PlayableSeconds >= 0.2)
            {
                if (Input.GetMouseButton(0))
                {
                    PowerIndicator.SetActive(true);
                    Power += Input.GetAxisRaw("Mouse Y");
                    if (Power <= 0)
                    {
                        Power = 0;
                    }
                    if (Power >= PowerLimit)
                    {
                        Power = PowerLimit;
                    }

                    Orbiter.EnabledY = false;
                }
                else
                {
                    Orbiter.EnabledY = true;
                }
            }
        }
        else
        {
            if(IsStationary())
            {
                StationaryUnplayableTime += Time.deltaTime;
                if(StationaryUnplayableTime >= 6f)
                {
                    ResetVelocityAndPosition();
                    //game.RpcResetPosition(ChildBall.gameObject, LastPositionRTS);
                }
            }

            PlayableSeconds = 0;
        }
        if (Input.GetMouseButtonUp(0))
        {
            if(Power >= 0 && IsStationary())
            {
                ChildBall.GetComponent<Rigidbody>().AddForce(XIndicator.transform.forward * Mathf.Pow(3.6f * Power, 0.88f) * 10);
                Power = 0;
                PowerIndicator.SetActive(false);
            }
        }

        PowerIndicator.transform.localScale = new Vector3(0.05f, 0.05f, Power / 40);
        PowerIndicator.transform.rotation = XIndicator.transform.rotation;
        PowerIndicator.transform.position = ChildBall.transform.position;
    }

    public void ResetVelocityAndPosition()
    {
        ChildBall.transform.localPosition = Vector3.zero;
        ChildBall.GetComponent<Rigidbody>().velocity = Vector3.zero;
        ChildBall.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
    }

    bool IsStationary()
    {
        return (ChildBall.GetComponent<Rigidbody>().velocity.magnitude <= 0.01f);
    }

    bool IsGrounded()
    {
        Debug.DrawRay(ChildBall.transform.position, -Vector3.up * 0.1f, Color.red, 10f);
        return Physics.Raycast(ChildBall.transform.position, -Vector3.up, 0.1f);
    }

    bool IsGrounded(out RaycastHit hit)
    {
        Debug.DrawRay(ChildBall.transform.position, -Vector3.up * 0.1f, Color.red, 10f);
        return Physics.Raycast(new Ray(ChildBall.transform.position, -Vector3.up), out hit, 0.1f);
    }
}
