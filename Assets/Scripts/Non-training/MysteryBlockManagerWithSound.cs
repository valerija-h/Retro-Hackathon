using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MysteryBlockManagerWithSound : MonoBehaviour
{
    public bool givesCoin;
    public GameObject mushroom;
    public GameObject flower;
    public GameObject nonPickupCoin; // coins without tags - don't directly interact with Mario
    public Sprite usedSprite; // sprite after it is hit

    private Sprite startSprite; // sprite before it is hit - at the start
    private bool wasHit = false;
    private Animator animator;
    private float blockHeight; // height of MysteryBlock
    private CoinManagerWithSound coinManager;
    public GameObject academy;
    private SoundManager soundManager;

    void Start()
    {
        startSprite = this.GetComponent<SpriteRenderer>().sprite;
        animator = this.GetComponent<Animator>();
        blockHeight = GetComponent<SpriteRenderer>().bounds.size.y;
        coinManager = FindObjectOfType<CoinManagerWithSound>();
        soundManager = FindObjectOfType<SoundManager>();
    }

    public void HitMysteryBlock(bool isBig) 
    {
        if (!wasHit)
        {
            soundManager.PlaySoundEffect("bumpBlock");
            // TODO - add sound effects
            transform.GetChild(0).gameObject.SetActive(false); // set hit collider to false 
            animator.enabled = false;
            wasHit = true;
            this.GetComponent<SpriteRenderer>().sprite = usedSprite;
            //TODO - add animation of brick being popped
            if (givesCoin) 
            {
                StartCoroutine(SpawnCoin());
            }
            else
            {
                soundManager.PlaySoundEffect("appearPowerUp");
                GameObject created;
                if (isBig)
                {
                    // spawn flower
                    created = Instantiate(flower, transform.position + new Vector3(0f, blockHeight, 0f), transform.rotation);

                    // TODO - add animation
                }
                else
                {
                    // spawn mushroom
                    created = Instantiate(mushroom, transform.position + new Vector3(0f, blockHeight, 0f), transform.rotation);
                    // TODO - add animation and script to mushroom so they walk
                }
                created.transform.parent = academy.transform;
            }
        }
    }

    // spawn a coin for a certain amount of time
    IEnumerator SpawnCoin()
    {
        coinManager.AddCoins(1);
        GameObject coin = Instantiate(nonPickupCoin, transform.position + new Vector3(0f, blockHeight, 0f), transform.rotation);
        // TODO - add coin animation and adjust wait time and sounds

        yield return new WaitForSeconds(0.75f);

        Destroy(coin);
    }

    // resets the current MysteryBlock
    public void ResetMysteryBlock() 
    {
        if (wasHit)
        {
            animator.enabled = true;
            wasHit = false;
            transform.GetChild(0).gameObject.SetActive(true); // set hit collider to true
            this.GetComponent<SpriteRenderer>().sprite = startSprite;
        }
    }

}
