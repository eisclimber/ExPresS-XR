using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UnityEngine.UI.Text))]
public class Score : MonoBehaviour
{

    public void AddScore(int score)
    {
        int currentScore = int.Parse(GetComponent<UnityEngine.UI.Text>().text);
        currentScore += score;
        GetComponent<UnityEngine.UI.Text>().text = currentScore.ToString();
    }

}
