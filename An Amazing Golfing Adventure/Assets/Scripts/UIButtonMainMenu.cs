using UnityEngine;
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

    // Use this for initialization
    void Start ()
    {
        rect = GetComponent<RectTransform>();
        text = GetComponent<Text>();
        originalPosition = rect.anchoredPosition;
        print(rect.anchoredPosition);
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
            SceneManager.LoadScene("LoadingGlen");
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
        Quit
    }
}
