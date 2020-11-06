using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickBlockManager : MonoBehaviour
{
    public int coinAmount; // amount of coin blocks give if any
    public GameObject nonPickupCoin; // coins without tags - don't directly interact with Mario

    private bool wasHit = false;
    private float blockHeight; // height of MysteryBlock
    public CoinManager coinManager;

    private void Start()
    {
        blockHeight = GetComponent<SpriteRenderer>().bounds.size.y;
        //coinManager = FindObjectOfType<CoinManager>();
    }

    public void HitBrickBlock(bool isBig)
    {
        if (isBig && !wasHit)
        {
            // TODO - add sound effects where needed
            if (coinAmount > 0) { StartCoroutine(SpawnCoin()); } 
            else { DestroyBlock(); }
        }
    }

    // spawn a coin for a certain amount of time
    IEnumerator SpawnCoin()
    {
        coinManager.AddCoins(1);
        GameObject coin = Instantiate(nonPickupCoin, transform.position + new Vector3(0f, blockHeight, 0f), transform.rotation);
        // TODO - add coin animation and adjust wait time - and sounds

        yield return new WaitForSeconds(0.75f);

        Destroy(coin);

        coinAmount--;
        if (coinAmount == 0) { DestroyBlock(); }
    }

    private void DestroyBlock() {
        wasHit = true;
        // TODO - add animation of brick being popped
        gameObject.SetActive(false);
    }

    // resets the current MysteryBlock
    public void ResetBrickBlock()
    {
        if (wasHit)
        {
            wasHit = false;
            gameObject.SetActive(true);
        }
    }
}
