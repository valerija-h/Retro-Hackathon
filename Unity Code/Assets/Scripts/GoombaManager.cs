using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoombaManager : MonoBehaviour
{
    public float movementSpeed;
    public bool isRight; // is goomba moving L or R
    public float startMovingDistanceX;
    public GameObject mario;

    private bool isHit = false;
    private bool isMoving = false;
    private Rigidbody2D goombaRigidbody;
    private bool originalIsRight;
    private Vector3 originalPosition; // position at start of game
    private ScoreManager scoreManager;
    private Animator animator;

    private void Start()
    {
        goombaRigidbody = this.GetComponent<Rigidbody2D>();
        scoreManager = FindObjectOfType<ScoreManager>();
        originalPosition = this.transform.position;
        originalIsRight = this.isRight;
        animator = this.GetComponent<Animator>();
    }

    private void Update()
    {
        if (isMoving)
        {
            // change direction if collision is detected
            if (IsCollisionDetected())
            {
                if (isRight) { isRight = false; }
                else { isRight = true; }
            }

            GoombaMovement();
        }
        else
        {
            // start moving when Mario is nearby
            float distanceFromMario = this.transform.position.x - mario.transform.position.x;
            if (distanceFromMario < startMovingDistanceX)
            {
                isMoving = true;
            }
        }
    }

    // function for moving Goomba
    private void GoombaMovement()
    {
        if (!isHit)
        {
            float movement = 1;
            if (!isRight) { movement = -1; }

            // move the player
            //goombaRigidbody.velocity = new Vector2(movement * movementSpeed, goombaRigidbody.velocity.y);
            this.transform.position += new Vector3(movement, 0f, 0f) * Time.deltaTime * movementSpeed;
            goombaRigidbody.velocity = new Vector2(0f, goombaRigidbody.velocity.y);

        }
    }

    // check if Goomba is going to hit something!
    private bool IsCollisionDetected()
    {
        float raycastDistance = 0.6f;
        Vector2 direction = Vector2.right;
        if (!isRight) { direction = Vector2.left; }

        int grnd = 1 << LayerMask.NameToLayer("Ground");
        int gmba = 1 << LayerMask.NameToLayer("Goomba");
        int mask = grnd | gmba;

        Debug.DrawRay(transform.position, direction * raycastDistance, Color.green);

        if (Physics2D.Raycast(transform.position, direction, raycastDistance, mask))
        {
            return true;
        }
        return false;
    }

    // if Goomba falls off platform
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("killbox"))
        {
            isMoving = false;
            gameObject.SetActive(false);
        }
    }

    public void ResetGoomba()
    {
        gameObject.SetActive(true);
        gameObject.tag = "goomba";
        animator.SetBool("isHit", false);
        isMoving = false;
        isHit = false; // if he is hit by Mario
        transform.position = originalPosition;
        isRight = originalIsRight;
    }

    // activates if Mario jumps on Goomba
    public void HitGoomba()
    {
        if (!isHit)
        {
            StartCoroutine(KillGoomba());
        }
    }

    // plays animation of goomba death shortly
    IEnumerator KillGoomba()
    {
        isHit = true;
        gameObject.tag = "Untagged"; // remove tag so Mario not detected
        scoreManager.AddScore("goomba");
        mario.GetComponent<MarioAgent>().RewardGoombaKill();
        Physics2D.IgnoreLayerCollision( 9, 10, true); // make goombas ignore mario
        //TODO - play sound effect maybe?
        animator.SetBool("isHit", true);

        yield return new WaitForSeconds(0.5f);

        gameObject.SetActive(false);
        Physics2D.IgnoreLayerCollision( 9, 10, false); // make goombas ignore mario

    }

    // tell Mario if it hit goomba!
    public bool IsHit()
    {
        return isHit;
    }
}
