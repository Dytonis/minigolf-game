using UnityEngine;
using System.Collections.Generic;

public class Game : MonoBehaviour
{

    public List<GameObject> Players = new List<GameObject>();
    public List<int> Strokes = new List<int>();
    public List<List<int>> StrokesHistory = new List<List<int>>();
    public int HoleNumber;

    public List<GameObject> HoleSpawns = new List<GameObject>();

	public void NextHole()
    {
        print("test");
        int index = 0;
        foreach(int i in Strokes)
        {
            StrokesHistory[index][HoleNumber] = i;

            index++;
        }

        if (HoleSpawns.Count > HoleNumber + 1)
        { 
            foreach (GameObject p in Players)
            {
                p.transform.position = HoleSpawns[HoleNumber + 1].transform.position;
            }

            HoleNumber++;
        }
    }

    public void ResetHole(GameObject player)
    {
       player.transform.position = HoleSpawns[HoleNumber].transform.position;
    }

    public void ResetPosition(GameObject player, Vector3 LRTS)
    {
        player.transform.localPosition = LRTS;
    }
}
