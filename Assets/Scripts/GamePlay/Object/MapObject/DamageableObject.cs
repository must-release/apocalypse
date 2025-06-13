using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct TagDamagePair
{
    public string tag;         // 충돌하는 오브젝트의 태그
    public int damageAmount;   // 해당 태그로 인해 입는 데미지
}

public class DamageableObject : MonoBehaviour
{
    public int maxHealth = 100;  // 오브젝트의 초기 체력
    public List<TagDamagePair> damageSources;  // 태그와 데미지 정보를 담는 리스트

    private int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;  // 시작할 때 최대 체력을 현재 체력으로 설정
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        foreach (var damageSource in damageSources)
        {
            if (collision.gameObject.CompareTag(damageSource.tag))
            {
                ApplyDamage(damageSource.tag, damageSource.damageAmount);
            }
        }
    }

    int FindDamageAmount(string tag)
    {
        foreach (var source in damageSources)
        {
            if (source.tag == tag)
            {
                return source.damageAmount;
            }
        }
        return 0; // 태그에 해당하는 데미지가 없을 경우 0 리턴
    }

    void ApplyDamage(string tag, int damage)
    {
        currentHealth -= damage;
        Debug.Log($"{tag} hit, dealt {damage} damage, remaining health: {currentHealth}");
        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }
}
