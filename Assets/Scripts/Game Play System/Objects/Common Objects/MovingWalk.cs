using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingWalk : MonoBehaviour
{
    public float speed;
    public bool isReversed = false;

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Vector2 direction = isReversed ? -transform.right : transform.right;
            other.transform.position += (Vector3)direction * Time.fixedDeltaTime * speed;
            Debug.Log("Player is being moved.");
        }

        if (other.CompareTag("Monster"))
        {
            Vector2 direction = isReversed ? -transform.right : transform.right;
            other.transform.position += (Vector3)direction * Time.fixedDeltaTime * speed;
            Debug.Log("Monster is being moved.");
        }
    }

}
