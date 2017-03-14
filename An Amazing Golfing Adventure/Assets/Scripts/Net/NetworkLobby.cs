using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;
using System;

public class NetworkLobby : NetworkManager
{

    public Text IPText;
    public Text PortText;

    public void StartAsHost()
    {
        SetPort();
        singleton.StartHost();
    }

    public void Connect()
    {
        SetPort();
        SetIP();
        singleton.StartClient();
    }

    public void SetPort()
    {
        try
        {
            singleton.networkPort = Convert.ToInt16(IPText.text);
        }
        catch
        {

        }
    }

    public void SetIP()
    {
        singleton.networkAddress = IPText.text;
    }
}
