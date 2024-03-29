﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class MarioAgent : Agent
{
    public float movementSpeed; // how fast Mario moves
    public float jumpForce; // Mario's jump force
    public float fallMultiplier;
    public float lowJumpMultiplier;
    public Camera camera;

    public float coinReward;
    public float killGoombaReward;
    public float hitByGoombaReward;
    public float hitByKillboxReward;
    public float flagReward;
    public float mushroomReward;
    public float flowerReward;
    public float mysteryBlockReward;
    public float brickBlockReward;
    public float moveRightReward;
    public float moveCameraRightReward; //rewards when camera moves right
    public float moveLeftReward; // reward for moving too far left (out of camera bounds)
    public float timeLeftReward; // multiplies by timeLeft

    public Sprite smallMarioSprite;
    public Sprite bigMarioSprite;
    public Vector2 smallMarioColliderSize;
    public Vector2 bigMarioColliderSize;

    public GameObject allMysteryBlocks;
    public GameObject allBrickBlocks;
    public GameObject allGoombas;

    private Vector3 originalCamPosition;
    private float raycastDistance = 0.6f;
    private bool isHit = false; // invicibility after being hit
    private bool isBig = false; // size after collecting mushroom
    private bool isGrounded = true;
    private int score = 0;
    private int coins = 0;
    private float timeLeft;
    private Vector3 agentStartPosition; // starting position of agent
    private Rigidbody2D playerRigidbody;
    private RigidbodyConstraints2D previousConstraints;
    private Animator animator;
    public CoinManager coinManager;
    public ScoreManager scoreManager;
    public GameObject academy;
    public TimeManager timeManager;
    private float cameraXStart; // the starting X bound of the camera - changes as Mario moves
    private float cameraCurrPos;

    // what agent does when an action is taken - moves player
    public override void OnActionReceived(float[] vectorAction)
    {
        // get the action index for movement and jumping
        float leftmovement = vectorAction[0];
        float rightmovement = vectorAction[1];
        float jump = Mathf.Round(vectorAction[2]);
        int movement = 0; // direction of movement L or Rx

        // the x start bound of the camera
        cameraCurrPos = camera.transform.position.x;
        cameraXStart = cameraCurrPos - (camera.aspect * camera.orthographicSize);

        // determine whether player is moving L or R or both
        if (leftmovement >= 1f && rightmovement >= 1f) { movement = 0; }
        else
        {
            // only move left if Mario will not go out of bounds
            if (leftmovement >= 1f)
            {
                if (transform.position.x > cameraXStart)
                {
                    movement = -1;
                    transform.localRotation = Quaternion.Euler(0, 180, 0);
                    AddReward(moveLeftReward);
                }
            }
            if (rightmovement >= 1f)
            {
                movement = 1;
                transform.localRotation = Quaternion.Euler(0, 0, 0);
                AddReward(moveRightReward);
            }
        }

        if (!isHit)
        {
            // move the player
            playerRigidbody.velocity = new Vector2(movement * movementSpeed, playerRigidbody.velocity.y);

            // player jump
            if (jump >= 1 && isGrounded)
            {
                //TODO -- Add jump sound
                playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x, jumpForce);
            }
        
        }

        //TODO - maybe change the location of the animator changes - to test
        if (movement != 0 && isGrounded) { ChangeAnimatorState("running"); }
        else if (movement == 0 && isGrounded) { ChangeAnimatorState("idle"); }
        if (!isGrounded) { ChangeAnimatorState("jumping"); }

        if (playerRigidbody.velocity.y < 0)
        {
            playerRigidbody.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (playerRigidbody.velocity.y > 0 && jump <= 0)
        {
            playerRigidbody.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }

        // penalty given each step to encourage agent to finish task quickly
        AddReward(-1f / MaxStep);
    }

    // controls for testing purposes
    public override void Heuristic(float[] actionsOut)
    {
        actionsOut[0] = Input.GetKey(KeyCode.A) ? 1.0f : 0.0f;
        actionsOut[1] = Input.GetKey(KeyCode.D) ? 1.0f : 0.0f;
        actionsOut[2] = Input.GetKey(KeyCode.Space) ? 1.0f : 0.0f;
    }

    public override void OnEpisodeBegin()
    {
        this.transform.position = agentStartPosition; // reset agent's position
        ChangeToSmallMario();
        camera.transform.position = originalCamPosition;
        scoreManager.ResetScore();
        coinManager.ResetCoins();
        score = scoreManager.GetScore();
        coins = coinManager.GetCoins();
        //call reset functions of all the blocks - mystery, brick and goomba!
        ResetAllObjects(allMysteryBlocks, "mysteryBlock");
        ResetAllObjects(allBrickBlocks, "brickBlock");
        ResetAllObjects(allGoombas, "goomba");
        DestroyAll("mushroom");
        DestroyAll("flower");
        timeManager.ResetTime();
        timeLeft = timeManager.GetTimeLeft();
        timeManager.StartTimer();
        //TODO - reset all goombas!
    }

    // destroy all objects with a specific tag in an academy
    private void DestroyAll(string currTag)
    {
        Transform t = academy.transform;
        foreach (Transform tr in t)
        {
            if (tr.tag == currTag)
            {
                Destroy(tr.gameObject);
            }
        }
    }

    private void ResetAllObjects(GameObject currGameObject, string tagName)
    {
        // for each child in Gameobject reset them!
        foreach (Transform child in currGameObject.transform)
        {
            if (tagName == "mysteryBlock") { child.GetComponent<MysteryBlockManager>().ResetMysteryBlock(); }
            if (tagName == "brickBlock") { child.GetComponent<BrickBlockManager>().ResetBrickBlock(); }
            if (tagName == "goomba") { child.GetComponent<GoombaManager>().ResetGoomba(); }
        }
    }

    // changes Mario's animation states
    private void ChangeAnimatorState(string action)
    {
        switch (action)
        {
            case "running":
                if (isBig) { animator.SetInteger("movementState", 4); }
                else { animator.SetInteger("movementState", 1); }
                break;
            case "jumping":
                if (isBig) { animator.SetInteger("movementState", 5); }
                else { animator.SetInteger("movementState", 2); }
                break;
            default:
                if (isBig) { animator.SetInteger("movementState", 3); }
                else { animator.SetInteger("movementState", 0); }
                break;
        }
    }

    // the observations of the agent
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(this.transform.localPosition); //agent position
        sensor.AddObservation(isBig);
        sensor.AddObservation(isHit);
        sensor.AddObservation(isGrounded);
        sensor.AddObservation(timeLeft);
        //sensor.AddObservation(cameraXStart);
        //sensor.AddObservation(cameraCurrPos);

        sensor.AddObservation(coins);
        sensor.AddObservation(score);

        //TODO - add RayPerceptionSensorComponent2D to agent with necessary tags.
    }

    private void Start()
    {
        Physics2D.IgnoreLayerCollision( 9, 11, true); // make goombas and collectibles ignore each other
        agentStartPosition = this.transform.position; // collect position of agent at start
        playerRigidbody = GetComponent<Rigidbody2D>();
        //coinManager = FindObjectOfType<CoinManager>();
        //scoreManager = FindObjectOfType<ScoreManager>();
        previousConstraints = playerRigidbody.constraints;
        animator = GetComponent<Animator>();
        originalCamPosition = camera.transform.position;
        timeManager.StartTimer();
    }

    private void Update()
    {
        isGrounded = IsGrounded();
        timeLeft = timeManager.GetTimeLeft();
        if (timeLeft <= 0) {
            EndEpisode();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        string collisionTag = collision.gameObject.tag;

        switch (collisionTag)
        {
            case "mushroom":
                //TODO - play sound effect, animation
                Destroy(collision.gameObject);
                ChangeToBigMario();
                scoreManager.AddScore(collisionTag);
                score = scoreManager.GetScore();
                AddReward(mushroomReward);
                break;
            case "flower":
                //TODO - play a sound effect, animation
                Destroy(collision.gameObject);
                scoreManager.AddScore(collisionTag);
                score = scoreManager.GetScore();
                AddReward(flowerReward);
                break;
            //case "goombaTop":
            //collision.gameObject.GetComponentInParent<GoombaManager>().HitGoomba();
            //score = scoreManager.GetScore();
            //AddReward(killGoombaReward);
            //Debug.Log("Boing");
            //break;
            case "goomba":
                StartCoroutine(CheckValidHit(collision));
                break;
            case "killbox":
                AddReward(hitByKillboxReward);
                EndEpisode();
                break;
            case "brickBlockUnder":
                collision.gameObject.GetComponentInParent<BrickBlockManager>().HitBrickBlock(isBig);
                score = scoreManager.GetScore();
                coins = coinManager.GetCoins();
                AddReward(brickBlockReward);
                break;
            case "mysteryBlockUnder":
                collision.gameObject.GetComponentInParent<MysteryBlockManager>().HitMysteryBlock(isBig);
                score = scoreManager.GetScore();
                coins = coinManager.GetCoins();
                AddReward(mysteryBlockReward);
                break;
            case "endFlag":
                //TODO - play end flag animation, sounds then end episode?
                AddReward(flagReward);
                AddReward(timeLeft * timeLeftReward); //add reward for doing it before timeLeft
                EndEpisode();
                break;
            default:
                break;
        }
    }

    // activated to reward Mario for killing Goomba
    public void RewardGoombaKill()
    {
        AddReward(killGoombaReward);
        score = scoreManager.GetScore();
    }


    private void ChangeToBigMario()
    {
        if (!isBig)
        {
            StartCoroutine(PlayOtherAnimations("becomeBig"));
            this.GetComponent<SpriteRenderer>().sprite = bigMarioSprite;
            this.GetComponent<BoxCollider2D>().size = bigMarioColliderSize;
            isBig = true;
            raycastDistance = 1.1f;
        }

    }

    private void ChangeToSmallMario()
    {
        if (isBig)
        {
            this.GetComponent<SpriteRenderer>().sprite = smallMarioSprite;
            this.GetComponent<BoxCollider2D>().size = smallMarioColliderSize;
            isBig = false;
            raycastDistance = 0.6f;
        }

    }

    // makes Mario immune to damage temporarily
    IEnumerator GetHit()
    {
        isHit = true;
        Physics2D.IgnoreLayerCollision(10, 9, true);
        //playerRigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
        playerRigidbody.velocity = new Vector2(0f, 0f); // make him stop moving
        ChangeToSmallMario();
        animator.SetInteger("otherState", 2);
        yield return new WaitForSeconds(1f);
        isHit = false;
        //playerRigidbody.constraints = previousConstraints; // set to previous state
        yield return new WaitForSeconds(0.3f);
        Physics2D.IgnoreLayerCollision(10, 9, false);
        animator.SetInteger("otherState", 0);

    }

    IEnumerator PlayOtherAnimations(string action) {
        switch (action) {
            case "becomeBig":
                animator.SetInteger("otherState", 1);
                yield return new WaitForSeconds(0.6f);
                animator.SetInteger("otherState", 0);
                break;
            case "death":
                isHit = true;
                playerRigidbody.velocity = new Vector2(0f, 5f); // make him stop moving
                Physics2D.IgnoreLayerCollision(10, 9, true);
                this.GetComponent<BoxCollider2D>().enabled = false;
                animator.SetInteger("otherState", 3);
                yield return new WaitForSeconds(1f);
                Physics2D.IgnoreLayerCollision(10, 9, false);
                animator.SetInteger("otherState", 0);
                this.GetComponent<BoxCollider2D>().enabled = true;
                isHit = false;
                EndEpisode();
                break;
        }
    }

    // makes Mario immune to damage temporarily
    IEnumerator CheckValidHit(Collision2D collision)
    {
        //playerRigidbody.velocity = new Vector2(0f, 0f);
        yield return new WaitForSeconds(0f);
        if (collision != null)
        {
            GoombaManager hit = collision.gameObject.GetComponent<GoombaManager>();
            if (hit != null && !hit.IsHit())
            {
                AddReward(hitByGoombaReward);
                if (isBig) { StartCoroutine(GetHit()); }
                else
                {
                    StartCoroutine(PlayOtherAnimations("death"));
                }
            }
        }
    
    }

    // check if Mario is grounded
    private bool IsGrounded()
    {
        int mask = 1 << LayerMask.NameToLayer("Ground");
        float castDist = 0.4125f;
        if (isBig) { castDist = 0.5f; }

        Vector3 leftCastPos = transform.position + new Vector3(-castDist, 0f, 0f);
        Vector3 rightCastPos = transform.position + new Vector3(castDist, 0f, 0f);
        Debug.DrawRay(leftCastPos, Vector2.down * raycastDistance, Color.green);
        Debug.DrawRay(rightCastPos, Vector2.down * raycastDistance, Color.green);
        RaycastHit2D leftCast = Physics2D.Raycast(leftCastPos, Vector2.down, raycastDistance, mask);
        RaycastHit2D rightCast = Physics2D.Raycast(rightCastPos, Vector2.down, raycastDistance, mask);
        if (leftCast || rightCast)
        {
            return true;
        }
        return false;
    }

    public void RewardMovement()
    {
        AddReward(moveCameraRightReward);
    }

}
