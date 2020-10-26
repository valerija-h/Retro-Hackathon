using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class MarioAgent : Agent
{
    public float movementSpeed; // how fast Mario moves
    public float jumpForce; // Mario's jump force
    public Camera camera;

    public float coinReward;
    public float killGoombaReward;
    public float mushroomReward;
    public float flowerReward;
    public float mysteryBlockReward;
    public float brickBlockReward;

    public Sprite smallMarioSprite;
    public Sprite bigMarioSprite;
    public Animator smallMarioAnimator;
    public Animator bigMarioAnimator;
    public Vector2 smallMarioColliderSize;
    public Vector2 bigMarioColliderSize;
    private float raycastDistance = 0.6f;

    private bool isHit = false; // invicibility after being hit
    private bool isBig = false; // size after collecting mushroom
    private bool isGrounded = true;
    private int score = 0;
    private int coins = 0;
    private Vector3 agentStartPosition; // starting position of agent
    private Rigidbody2D playerRigidbody;
    private RigidbodyConstraints2D previousConstraints;

    private CoinManager coinManager;
    private ScoreManager scoreManager;

    // what agent does when an action is taken - moves player
    public override void OnActionReceived(float[] vectorAction)
    {
        // get the action index for movement and jumping
        float leftmovement = vectorAction[0];
        float rightmovement = vectorAction[1];
        int jump = Mathf.FloorToInt(vectorAction[2]);
        int movement = 0; // direction of movement L or R

        // the x start bound of the camera
        float horizontalMin = camera.transform.position.x - (camera.aspect * camera.orthographicSize);

        // determine whether player is moving L or R or both
        if (leftmovement >= 1f && rightmovement >= 1f) { movement = 0; }
        else
        {
            // only move left if Mario will not go out of bounds
            if (leftmovement >= 1f && transform.position.x > horizontalMin) { movement = -1; }
            if (rightmovement >= 1f) { movement = 1; }
        }

        // move the player
        playerRigidbody.velocity = new Vector2(movement * movementSpeed, playerRigidbody.velocity.y);

        // player jump
        if (jump == 1 && isGrounded)
        {
            //TODO -- Add jump sound
            playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x, jumpForce);
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
        //TODO - call reset functions of all the blocks - mystery, brick and goomba!
    }

    // the observations of the agent
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(this.transform.localPosition); //agent position
        sensor.AddObservation(isBig);
        sensor.AddObservation(isHit);
        sensor.AddObservation(isGrounded);

        sensor.AddObservation(coins);
        sensor.AddObservation(score);

        //TODO - add RayPerceptionSensorComponent2D to agent with necessary tags.
    }

    private void Start()
    {
        agentStartPosition = this.transform.position; // collect position of agent at start
        playerRigidbody = GetComponent<Rigidbody2D>();
        coinManager = FindObjectOfType<CoinManager>();
        scoreManager = FindObjectOfType<ScoreManager>();
        previousConstraints = playerRigidbody.constraints;
    }

    private void Update()
    {
        isGrounded = IsGrounded();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        string collisionTag = collision.gameObject.tag;

        //TODO - program the collisions
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
            case "goombaTop":
                collision.gameObject.GetComponentInParent<GoombaManager>().HitGoomba();
                score = scoreManager.GetScore();
                AddReward(killGoombaReward);
                break;
            case "goomba":
                if (isBig) {
                    StartCoroutine(GetHit());
                } else {
                    //TODO - kill Mario - reset agent
                    //TODO - Add negative reward
                }
                break;
            case "killbox":
                //TODO - kill Mario - reset agent
                //TODO - Add negative reward
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
                //TODO - calls function of the end flag (if any)
                //TODO - add HIGH reward
                //TODO - mark as done
                break;
            default:
                break;
        }
    }

    private void ChangeToBigMario()
    {
        if (!isBig)
        {
            //TODO - change animator
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
            //TODO - change animator
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
        playerRigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
        gameObject.GetComponent<Collider2D>().enabled = false;
        ChangeToSmallMario();

        // TODO - play any sounds or animation
        yield return new WaitForSeconds(0.75f);

        // TODO - check if he can move after he is hit during invincibility
        gameObject.GetComponent<Collider2D>().enabled = true;
        playerRigidbody.constraints = previousConstraints; // set to previous state
        isHit = false;
    }

    // check if Mario is grounded
    private bool IsGrounded()
    {
        int mask = 1 << LayerMask.NameToLayer("Ground");
        //Debug.DrawRay(transform.position, Vector2.down * raycastDistance, Color.green);
        if (Physics2D.Raycast(transform.position, Vector2.down, raycastDistance, mask))
        {
            return true;
        }
        return false;
    }

}
