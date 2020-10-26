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
    public float hitByGoombaReward;
    public float hitByKillboxReward;
    public float flagReward;
    public float mushroomReward;
    public float flowerReward;
    public float mysteryBlockReward;
    public float brickBlockReward;

    public Sprite smallMarioSprite;
    public Sprite bigMarioSprite;
    public Vector2 smallMarioColliderSize;
    public Vector2 bigMarioColliderSize;

    public GameObject allMysteryBlocks;
    public GameObject allBrickBlocks;
    private Vector3 originalCamPosition;
    private float raycastDistance = 0.6f;
    private bool isHit = false; // invicibility after being hit
    private bool isBig = false; // size after collecting mushroom
    private bool isGrounded = true;
    private int score = 0;
    private int coins = 0;
    private Vector3 agentStartPosition; // starting position of agent
    private Rigidbody2D playerRigidbody;
    private RigidbodyConstraints2D previousConstraints;
    private Animator animator;
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

        //TODO - maybe change the location of the animator changes - to test
        if (movement != 0 && isGrounded) { ChangeAnimatorState("running"); }
        else if (movement == 0 && isGrounded) { ChangeAnimatorState("idle"); }

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
        //TODO - reset all goombas!
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
                if (isBig) { animator.SetInteger("movementState", 1); }
                else { animator.SetInteger("movementState", 1); }
                break;
            case "jumping":
                if (isBig) { animator.SetInteger("movementState", 2); }
                else { animator.SetInteger("movementState", 2); }
                break;
            default:
                if (isBig) { animator.SetInteger("movementState", 0); }
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
        animator = GetComponent<Animator>();
        originalCamPosition = camera.transform.position;
    }

    private void Update()
    {
        isGrounded = IsGrounded();
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
            case "goombaTop":
                collision.gameObject.GetComponentInParent<GoombaManager>().HitGoomba();
                score = scoreManager.GetScore();
                AddReward(killGoombaReward);
                break;
            case "goomba":
                if (isBig)
                {
                    StartCoroutine(GetHit());
                }
                else
                {
                    AddReward(hitByGoombaReward);
                    //TODO - play death animation with coroutine then end episode
                    EndEpisode();
                }
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
                EndEpisode();
                break;
            default:
                break;
        }
    }

    private void ChangeToBigMario()
    {
        if (!isBig)
        {
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
        ChangeAnimatorState("jumping");
        return false;
    }

}
