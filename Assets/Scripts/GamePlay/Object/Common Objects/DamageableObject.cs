using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct TagDamagePair
{
    public string tag;         // �浹�ϴ� ������Ʈ�� �±�
    public int damageAmount;   // �ش� �±׷� ���� �Դ� ������
}

public class DamageableObject : MonoBehaviour
{
    public int maxHealth = 100;  // ������Ʈ�� �ʱ� ü��
    public List<TagDamagePair> damageSources;  // �±׿� ������ ������ ��� ����Ʈ

    private int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;  // ������ �� �ִ� ü���� ���� ü������ ����
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
        return 0; // �±׿� �ش��ϴ� �������� ���� ��� 0 ����
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
