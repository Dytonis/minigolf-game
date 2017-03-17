using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class UIScreens : MonoBehaviour
{
    public List<GameObject> Screens = new List<GameObject>();
    public NetworkLobby Lobby;

    public Text MapName;
    public Text Lobbyname;
    public Text Playernames;

    public void SwitchScreen(int ScreenNumber)
    {
        foreach (GameObject obj in Screens)
        {
            obj.SetActive(false);
        }

        Screens[ScreenNumber].SetActive(true);
    }
}
