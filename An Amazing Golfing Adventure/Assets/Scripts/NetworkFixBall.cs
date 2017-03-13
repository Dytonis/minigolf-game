using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class NetworkFixBall : NetworkBehaviour
{
    private float time;
    public float TickRate;
    public GameObject Ball;

	// Use this for initialization
	void Start ()
    {
	    
	}
	
	// Update is called once per frame
	void Update ()
    {
        time += Time.deltaTime;

        if(time >= 1/TickRate)
        {
            time = 0;

            CmdTellBallPos(Ball.transform.localPosition);
        }
	}

    [Command]
    public void CmdTellBallPos(Vector3 pos)
    {
        //ran on the player on the server
        RpcTellBallPos(pos);
    }

    [ClientRpc]
    public void RpcTellBallPos(Vector3 pos)
    {
        //for the ball that is now on the server. We don't want this to affect the local player
        if(!isLocalPlayer)
            Ball.transform.localPosition = pos;
    }
}
