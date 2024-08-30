using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heal_Item : MonoBehaviour
{
    public int healAmount = 20;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            heal(healAmount);
            Destroy(gameObject);
        }
    }

    void heal(int HP)
    {

    }
}
