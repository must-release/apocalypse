using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleObject : MonoBehaviour
{
    public int health = 1;
    public int grenadeDamage = 50;
    public int bulletDamage = 5;

    public bool IsOnlyDamagedbyGrenade;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (IsOnlyDamagedbyGrenade)
        {
            if (collision.gameObject.CompareTag("Grenade"))
            {
                TakeExplosionDamage(grenadeDamage);
            }
        }
        else
        {
            if (collision.gameObject.CompareTag("Bullet"))
            {
                Debug.Log("Bullet hit");
                TakeBulletDamage(bulletDamage);
            }

            if (collision.gameObject.CompareTag("Grenade"))
            {
                Debug.Log("Grenade hit");
                TakeExplosionDamage(grenadeDamage);
            }
        }
    }

    public void TakeBulletDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void TakeExplosionDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
