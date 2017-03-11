using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GlenLoader : MonoBehaviour
{

    float delay;
    public float WaitTime;
    public bool loading;

    public RectTransform progress;

    AsyncOperation async = null;

	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        delay += Time.deltaTime;

        if(delay >= WaitTime && !loading)
        {
            StartCoroutine(LoadLevel("Glen"));
            loading = true;
        }
	}

    void OnGUI()
    {
        if(loading)
            progress.localScale = new Vector3(async.progress, 1, 1);
        else
        {
            progress.localScale = new Vector3((delay / (WaitTime / 1.33f) > 1) ? 1 : (delay / (WaitTime / 1.33f)), 1, 1);
        }
    }

    private IEnumerator LoadLevel(string levelName)
    {
        async = SceneManager.LoadSceneAsync(levelName);
        yield return async;
    }
}
