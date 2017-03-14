using UnityEngine;
using System.Collections.Generic;

public class UIScreens : MonoBehaviour
{
    public List<GameObject> Screens = new List<GameObject>();
    public NetworkLobby Lobby;

    public void SwitchScreen(int ScreenNumber)
    {
        foreach (GameObject obj in Screens)
        {
            obj.SetActive(false);
        }

        Screens[ScreenNumber].SetActive(true);
    }
}
