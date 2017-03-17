using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIButtonMainMenu : MonoBehaviour
{

    RectTransform rect;
    Text text;
    Vector3 originalPosition;
    public Vector3 offset;
    public float Speed;

    public bool AnimateMovement = true;
    public bool AnimateColor = true;

    public Buttons button;

    private UIScreens ScreensManager;

    // Use this for initialization
    void Start ()
    {
        rect = GetComponent<RectTransform>();
        text = GetComponent<Text>();
        originalPosition = rect.anchoredPosition;

        ScreensManager = transform.parent.parent.GetComponent<UIScreens>();
    }
	
	// Update is called once per frame
	void Update ()
    {
	    
	}

    public void StartHover()
    {
        if(AnimateMovement)
            StartCoroutine(SmoothMove(originalPosition, add(rect.anchoredPosition, offset), Speed));

        if(AnimateColor)
            StartCoroutine(SmoothColor(Color.white, Color.yellow, Speed));
    }

    public void EndHover()
    {
        StopAllCoroutines();
        rect.anchoredPosition = originalPosition;
        text.color = Color.white;
    }

    public void Click()
    {
        if(button == Buttons.Singleplayer)
        {
            ScreensManager.Lobby.NLStartSingleplayer();
            SceneManager.LoadScene("LoadingGlen");
        }
        else if (button == Buttons.Multiplayer)
        {
            ScreensManager.SwitchScreen(3);
        }
        else if (button == Buttons.HostAGame)
        {
            ScreensManager.SwitchScreen(5);
        }
        else if (button == Buttons.JoinAGame)
        {
            ScreensManager.SwitchScreen(1);
        }
        else if (button == Buttons.BackToMain)
        {
            ScreensManager.SwitchScreen(0);
        }
        else if (button == Buttons.BackToHJ)
        {
            ScreensManager.SwitchScreen(3);
        }
        else if (button == Buttons.CreateLobby)
        {
            ScreensManager.Lobby.SetScreens(ScreensManager);

            ScreensManager.Lobby.NLStartAsHost();
        }
        else if (button == Buttons.Connect)
        {
            ScreensManager.Lobby.SetScreens(ScreensManager);

            ScreensManager.SwitchScreen(2);

            ScreensManager.Lobby.NLConnect();
        }
        else if (button == Buttons.StartGame)
        {
            ScreensManager.Lobby.LocalPlayer.HandleLevelChange();
        }
    }

    IEnumerator SmoothMove(Vector3 startpos, Vector3 endpos, float seconds)
    {
        float t = 0.0f;
        while (t <= 1.0)
        {
            t += Time.deltaTime / seconds;
            rect.anchoredPosition = Vector3.Lerp(startpos, endpos, Mathf.SmoothStep(0.0f, 1.0f, t));
            yield return null;
        }
    }
    IEnumerator SmoothColor(Color start, Color end, float seconds)
    {
        float t = 0.0f;
        while (t <= 1.0)
        {
            t += Time.deltaTime / seconds;
            text.color = Color.Lerp(start, end, Mathf.SmoothStep(0.0f, 1.0f, t));
            yield return null;
        }
    }

    Vector3 add(Vector3 a, Vector3 b)
    {
        return new Vector3(a.x + b.x, a.y + b.y, a.z + b.z);
    }

    public enum Buttons
    {
        Singleplayer,
        Multiplayer,
        Options,
        Quit,
        BackToMain,
        BackToHJ,
        HostAGame,
        JoinAGame,
        Connect,
        StartGame,
        MapButtonLeft,
        MapButtonRight,
        ShowScorecard,
        CreateLobby
    }
}
