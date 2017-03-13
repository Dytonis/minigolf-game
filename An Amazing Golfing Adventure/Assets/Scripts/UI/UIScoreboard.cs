using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class UIScoreboard : MonoBehaviour
{

    public GameObject LinePrefab;
    public RectTransform TopObject;
    public RectTransform BottomObject;

    private Vector3 anchoredTop;
    private Vector3 anchoredBot;

    public float LineSize;
    public float TopOffset;
    public float BotOffset;

    public bool isIntialized = false;

    public List<UIScoreboardLine> Lines = new List<UIScoreboardLine>();

    public void Initialize(string[] PlayerNamesInit)
    {
        anchoredTop = TopObject.anchoredPosition;
        anchoredBot = BottomObject.anchoredPosition;

        int index = 0;
        foreach(string s in PlayerNamesInit)
        {
            print("creating line " + s);
            GameObject line = Instantiate(LinePrefab) as GameObject;
            Lines.Add(line.GetComponent<UIScoreboardLine>());
            line.GetComponent<UIScoreboardLine>().Name.text = s;
            line.transform.SetParent(transform);
            line.transform.SetAsFirstSibling();

            if(index % 2 == 1)
            {
                line.GetComponent<Image>().color = new Color(
                    LinePrefab.GetComponent<Image>().color.r + 0.2f,
                    LinePrefab.GetComponent<Image>().color.g + 0.2f,
                    LinePrefab.GetComponent<Image>().color.b + 0.2f,
                    LinePrefab.GetComponent<Image>().color.a);
            }

            RectTransform rect = line.GetComponent<RectTransform>();

            rect.anchoredPosition = new Vector3(0, (float)(((LineSize * (PlayerNamesInit.Length - 1)) - (LineSize * index)) - ((LineSize * (PlayerNamesInit.Length - 1)) * 0.5)));
            rect.localScale = Vector3.one;

            index++;
        }

        TopObject.anchoredPosition = new Vector3(TopObject.anchoredPosition.x, TopObject.anchoredPosition.y + ((PlayerNamesInit.Length * 60) / 2) + TopOffset);
        BottomObject.anchoredPosition = new Vector3(BottomObject.anchoredPosition.x, BottomObject.anchoredPosition.y + ((PlayerNamesInit.Length * -60) / 2) + BotOffset);

        isIntialized = true;
    }

    /// <summary>
    /// Handles the deletion and recreation of the scoreboard lines.
    /// </summary>
    /// <param name="PlayerNamesInit"></param>
    public void UpdatePlayers(string[] PlayerNamesInit)
    {
        //loop through lines, deleting objects
        for(int i = 0; i < Lines.Count; i++)
        {
            Destroy(Lines[i].gameObject);
        }

        Lines.Clear();

        TopObject.anchoredPosition = anchoredTop;
        BottomObject.anchoredPosition = anchoredBot;

        Initialize(PlayerNamesInit);
    }

    public void ApplyScores(int hole, int player, int score)
    {
        Lines[player].Scores[hole].text = score.ToString();
        Lines[player].ScoresRaw[hole] = score;
        Lines[player].Tot.text = Lines[player].ScoresRaw.Sum().ToString();
    }

	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}
}
