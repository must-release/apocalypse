using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovableObject : MonoBehaviour
{
    private Rigidbody2D rb;
    public float pushForce;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.isKinematic = false;
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Vector2 playerPosition = collision.gameObject.transform.position;
            Vector2 objectPosition = transform.position;

            float verticalDifference = Mathf.Abs(playerPosition.y - objectPosition.y);

            if (verticalDifference < 0.5f)
            {

                if (Mathf.Abs(playerPosition.x - objectPosition.x) > 0.1f)
                {

                    Vector2 pushDirection = (objectPosition - new Vector2(playerPosition.x, playerPosition.y)).normalized;

                    pushDirection = new Vector2(pushDirection.x, 0).normalized;

                    rb.velocity = pushDirection * pushForce;
                }
                else
                {
                    rb.velocity = Vector2.zero;
                    rb.angularVelocity = 0f;
                }
            }
            else
            {
                rb.velocity = Vector2.zero;
                rb.angularVelocity = 0f;
            }
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
    }
}
