using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakablePlatform : MonoBehaviour
{
    public float breakDelay = 2f; // �÷��̾ ���� �� �÷����� �ν���������� �ð�
    public bool respawn = true;   // �÷����� �ν��� �� �ٽ� ������ ����
    public float respawnTime = 5f; // �ٽ� ���� ���, ��������� �ɸ��� �ð�

    private Collider2D platformCollider;
    private SpriteRenderer platformRenderer;

    void Start()
    {
        platformCollider = GetComponent<Collider2D>();
        platformRenderer = GetComponent<SpriteRenderer>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // �÷��̾ ������ �浹�ߴ��� Ȯ��
            if (collision.contacts[0].point.y > transform.position.y)
            {
                // �÷��̾ �÷��� ������ �浹���� ���� �ν����� ����
                StartCoroutine(BreakPlatform());
            }
        }
    }

    IEnumerator BreakPlatform()
    {
        // �ν����� �������� ��� �ð�
        yield return new WaitForSeconds(breakDelay);

        // �÷����� ��Ȱ��ȭ�Ͽ� �ν����� ȿ���� ��
        platformCollider.enabled = false;
        platformRenderer.enabled = false;

        // �ٽ� ���⵵�� ������ ���
        if (respawn)
        {
            yield return new WaitForSeconds(respawnTime);
            RespawnPlatform();
        }
        else
        {
            // �ٽ� ������ �ʴ� ��� �÷����� �ı�
            Destroy(gameObject);
        }
    }

    void RespawnPlatform()
    {
        // �÷����� �ٽ� Ȱ��ȭ�Ͽ� �����
        platformCollider.enabled = true;
        platformRenderer.enabled = true;
    }
}
