using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoinManager : MonoBehaviour
{
    //public Text textOfCoin; // text object
    private int numOfCoin = 0; // number of coins
    public ScoreManager scoreManager;

    void Start()
    {
        //DisplayCoins();
        //scoreManager = FindObjectOfType<ScoreManager>();
    }

    // returns how many coins there are
    public int GetCoins()
    {
        return numOfCoin;
    }

    // add an amount of coins
    public void AddCoins(int amount)
    {
        scoreManager.AddScore("coin");
        numOfCoin += amount;
        //DisplayCoins();
    }

    // resets the coina
    public void ResetCoins()
    {
        numOfCoin = 0;
        //DisplayCoins();
    }

    //// displays the current number if coins
    //private void DisplayCoins()
    //{
    //    textOfCoin.text = "x" + numOfCoin.ToString();
    //}
}
