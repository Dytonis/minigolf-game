using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class LobbyPlayer : NetworkBehaviour
{
    public void Awake()
    {

    }

    public override void OnStartLocalPlayer()
    { 
        print("local player start");

        gameObject.tag = "PlayerLocal";
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
    public void CmdPrintServer(string data)
    {
        print(data);
    }

    [ClientRpc]
    public void RpcChangeLevel()
    {
        SceneManager.LoadScene("LoadingGlen");
    }
}
