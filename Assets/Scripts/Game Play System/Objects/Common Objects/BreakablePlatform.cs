using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakablePlatform : MonoBehaviour
{
    public float breakDelay = 2f; // 플레이어가 밟은 후 플랫폼이 부숴지기까지의 시간
    public bool respawn = true;   // 플랫폼이 부숴진 후 다시 생길지 여부
    public float respawnTime = 5f; // 다시 생길 경우, 재생성까지 걸리는 시간

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
            // 플레이어가 위에서 충돌했는지 확인
            if (collision.contacts[0].point.y > transform.position.y)
            {
                // 플레이어가 플랫폼 위에서 충돌했을 때만 부숴지기 시작
                StartCoroutine(BreakPlatform());
            }
        }
    }

    IEnumerator BreakPlatform()
    {
        // 부숴지기 전까지의 대기 시간
        yield return new WaitForSeconds(breakDelay);

        // 플랫폼을 비활성화하여 부숴지는 효과를 줌
        platformCollider.enabled = false;
        platformRenderer.enabled = false;

        // 다시 생기도록 설정된 경우
        if (respawn)
        {
            yield return new WaitForSeconds(respawnTime);
            RespawnPlatform();
        }
        else
        {
            // 다시 생기지 않는 경우 플랫폼을 파괴
            Destroy(gameObject);
        }
    }

    void RespawnPlatform()
    {
        // 플랫폼을 다시 활성화하여 재생성
        platformCollider.enabled = true;
        platformRenderer.enabled = true;
    }
}
