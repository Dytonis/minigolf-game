using UnityEngine;
using System.Collections;
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

    public void Awake()
    {
        server = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameServer>();
        ChildBall.network = this;
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
        }
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

        print("server players " + server.PlayerCount);

        if (server.PlayerCount >= server.ExpectingPlayers)
        {
            //Send move all command to all clients' new connecting object
            RpcNextHole();
        }
    }

    public void TriggerNextHole()
    {
        if(!isServer)
        {
            return;
        }

        //server authority

        RpcNextHole();
    }

    [ClientRpc]
    public void RpcNextHole()
    {
        print("CLIENT Recieve Command: RpcNextHole");
        print(gameObject.GetInstanceID());

        print(server.HoleSpawns.Count + " > " + (server.HoleNumber + 1));
        if (server.HoleSpawns.Count > (server.HoleNumber + 1))
        {
            server.GeneratePlayerList();
            print(server.PlayerList);
            foreach (GameObject p in server.PlayerList)
            {
                print("moving player " + p.transform.GetInstanceID());
                //move all objects for client
                p.transform.position = server.HoleSpawns[server.HoleNumber + 1].transform.position;

                PlayerControllerNetwork pN = p.GetComponent<PlayerControllerNetwork>();

                pN.ChildBall.transform.localPosition = Vector3.zero;
                pN.ChildBall.GetComponent<Rigidbody>().velocity = Vector3.zero;
                pN.ChildBall.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            }

            server.HoleNumber++;
        }
    }

    void Update()
    {
        if(!IsClientControlled)
        {
            return;
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
