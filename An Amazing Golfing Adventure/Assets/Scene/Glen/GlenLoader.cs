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

    public NetworkLobby lobby;

	// Use this for initialization
	void Start ()
    {
        lobby = GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<NetworkLobby>();

        lobby.NLChangeLevel("Glen");
        loading = true;
    }
	
	// Update is called once per frame
	void Update ()
    {
        delay += Time.deltaTime;
	}

    void OnGUI()
    {
        progress.localScale = new Vector3((delay / (WaitTime / 1.33f) > 1) ? 1 : (delay / (WaitTime / 1.33f)), 1, 1);        
    }
}
