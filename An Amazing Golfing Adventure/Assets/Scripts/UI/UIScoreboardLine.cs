using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class UIScoreboardLine : MonoBehaviour
{
    public List<Text> Scores = new List<Text>();
    public List<int> ScoresRaw = new List<int>();
    public Text Tot;
    public Text Name;
}
