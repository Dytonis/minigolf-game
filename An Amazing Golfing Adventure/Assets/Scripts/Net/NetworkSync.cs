using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class NetworkSync : NetworkBehaviour
{
    public bool Setup = false;
    [Range(0, 64)]
    public int TickRate;
    [Range(0, 64)]
    public int LerpRate;
    public float LerpStrength;

    public Rigidbody Ball;
    public PlayerRoot Root;

    ////////////
    [SyncVar]
    public Vector3 ballPosServer;
    [SyncVar]
    public Vector3 masterPosServer;
    [SyncVar]
    public Quaternion ballRotServer;
    [SyncVar]
    public Vector3 ballVelServer;
    [SyncVar]
    public Vector3 ballAngServer;
    ////////////

    // Use this for initialization
    void Start ()
    {
	
	}

    public void SetupSync(Rigidbody ball, PlayerRoot root)
    {
        this.Ball = ball;
        this.Root = root;

        if(ball != null && root != null)
        {
            Setup = true;
        }
        else
        {
            Debug.LogError("Setup failed!");
        }
    }

    private float time;
    private float lerptime;
	// Update is called once per frame
	void FixedUpdate ()
    {
        if (!Setup)
            return;

        time += Time.deltaTime;
        lerptime += Time.deltaTime;

        if (!Ball.gameObject.GetComponent<PlayerController>().IsClientControlled)
        {
            Ball.velocity = this.ballVelServer;
            Ball.angularVelocity = this.ballAngServer;
            Ball.transform.rotation = this.ballRotServer;
            Root.transform.position = masterPosServer;
        }

        if(lerptime >= (float)(1f / (float)LerpRate))
        {
            lerptime = 0;
            if(!Ball.gameObject.GetComponent<PlayerController>().IsClientControlled)
                LerpPosition();
        }

        if (time >= (float)(1f / (float)TickRate))
        {
            time = 0;
            if (Ball.gameObject.GetComponent<PlayerController>().IsClientControlled)
            {
                CmdTellServerRigidbody(Ball.velocity, Ball.angularVelocity);
                CmdTellServerPositions(Root.transform.position, Ball.transform.localPosition, Ball.transform.rotation);
            }
        }
	}

    public void LerpPosition()
    {
        Ball.transform.localPosition = Vector3.Lerp(Ball.transform.localPosition, this.ballPosServer, Time.deltaTime * LerpStrength);
    }

    [ClientRpc]
    public void RpcGotMessageTeleported(Vector3 MasterPosition, Vector3 BallLocalPosition, Quaternion BallRotation)
    {
        if (!Ball.gameObject.GetComponent<PlayerController>().IsClientControlled)
        {
            Ball.transform.rotation = BallRotation;
            Ball.transform.localPosition = BallLocalPosition;
            Root.transform.position = MasterPosition;
        }
    }

    [ClientRpc]
    public void RpcGotMessageForceRigidbody(Vector3 velocity, Vector3 angular)
    {
        if (!Ball.gameObject.GetComponent<PlayerController>().IsClientControlled)
        {
            Ball.velocity = velocity;
            Ball.angularVelocity = angular;
        }
    }

    [Command]
    public void CmdTellServerTeleported(Vector3 MasterPosition, Vector3 BallLocalPosition, Quaternion BallRotation)
    {
        this.masterPosServer = MasterPosition;
        this.ballPosServer = BallLocalPosition;
        this.ballRotServer = BallRotation;
        RpcGotMessageTeleported(MasterPosition, BallLocalPosition, BallRotation);
    }

    [Command]
    public void CmdTellServerRigidbody(Vector3 velocity, Vector3 angular)
    {
        this.ballVelServer = velocity;
        this.ballAngServer = angular;
    }

    [Command]
    public void CmdTellServerPositions(Vector3 MasterPosition, Vector3 BallLocalPosition, Quaternion BallRotation)
    {
        this.masterPosServer = MasterPosition;
        this.ballPosServer = BallLocalPosition;
        this.ballRotServer = BallRotation;
    }

    [Command]
    public void CmdTellServerForceRigidbody(Vector3 velocity, Vector3 angular)
    {
        this.ballVelServer = velocity;
        this.ballAngServer = angular;
        RpcGotMessageForceRigidbody(velocity, angular);
    }
}
