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

    private void Start()
    {
        goombaRigidbody = this.GetComponent<Rigidbody2D>();
        scoreManager = FindObjectOfType<ScoreManager>();
        originalPosition = this.transform.position;
        originalIsRight = this.isRight;
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
        } else {
            // start moving when Mario is nearby
            float distanceFromMario =  this.transform.position.x - mario.transform.position.x;
            if (distanceFromMario < startMovingDistanceX) {
                isMoving = true;
            }
        }
    }

    // function for moving Goomba
    private void GoombaMovement() { 
        if (!isHit) {
            float movement = 1;
            if (!isRight) { movement = -1; }

            // move the player
            goombaRigidbody.velocity = new Vector2(movement * movementSpeed, goombaRigidbody.velocity.y);

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
        isMoving = false;
        isHit = false; // if he is hit by Mario
        transform.position = originalPosition;
        isRight = originalIsRight;
    }

    // activates if Mario jumps on Goomba
    public void HitGoomba() {
        scoreManager.AddScore("goomba");
        StartCoroutine(KillGoomba());
    }

    // plays animation of goomba death shortly
    IEnumerator KillGoomba()
    {
        //TODO - play animation of Goomba death

        yield return new WaitForSeconds(0.5f);

        gameObject.SetActive(false);
    }
}
