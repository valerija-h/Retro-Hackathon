using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoombaTopManager : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {   
        if (collision.gameObject.CompareTag("mario"))
        {
            GetComponentInParent<GoombaManager>().HitGoomba();
        }
    }
}
