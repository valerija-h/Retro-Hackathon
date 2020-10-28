using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoombaTopManagerWithSound : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {   
        if (collision.gameObject.CompareTag("mario"))
        {
            GetComponentInParent<GoombaManagerWithSound>().HitGoomba();
        }
    }
}
