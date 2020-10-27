using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    //public Text textOfScore; // text object
    public int coinScore;
    public int goombaScore;
    public int powerupScore;

    private int numOfScore = 0; // score number

    void Start()
    {
        //DisplayScore();
    }

    public void ResetScore()
    {
        numOfScore = 0;
        //DisplayScore();
    }

    public void AddScore(string tagName)
    {
        switch (tagName)
        {
            case "goomba":
                numOfScore += goombaScore;
                break;
            case "coin":
                numOfScore += coinScore;
                break;
            case "mushroom":
            case "flower":
                numOfScore += powerupScore;
                break;
            default:
                break;
        }
        //DisplayScore();
    }

    public int GetScore()
    {
        return numOfScore;
    }

    private void DisplayScore()
    {
        string toDisplay = "";

        if (numOfScore <= 9) { toDisplay += "00000"; }
        else if (numOfScore <= 99) { toDisplay += "0000"; }
        else if (numOfScore <= 999) { toDisplay += "000"; }
        else if (numOfScore <= 9999) { toDisplay += "00"; }
        else if (numOfScore <= 99999) { toDisplay += "0"; }
        toDisplay += numOfScore.ToString();

        //textOfScore.text = toDisplay;
    }
}
