using System.Collections;
using UnityEngine;

public class SpiderWeb : MonoBehaviour
{
    public float slowDownFactor = 0.5f;  // 속도를 얼마나 느리게 할지 비율로 설정
    public float gravityScaleFactor = 0.5f;  // 중력 스케일을 얼마나 줄일지 설정
    private float originalGravityScale;
    private float originalMoveSpeed;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
            PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();

            if (rb != null && playerController != null)
            {
                // 원래의 중력 스케일과 이동 속도를 저장
                originalGravityScale = rb.gravityScale;
                originalMoveSpeed = playerController.moveSpeed;

                // 중력 스케일과 이동 속도를 줄여 거미줄의 영향을 줌
                rb.gravityScale *= gravityScaleFactor;
                playerController.moveSpeed *= slowDownFactor;

                // 점프 중 중력 스케일이 비정상적으로 낮아지지 않도록 중력 스케일 조정
                playerController.SetGravityScale(originalGravityScale);
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
            PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();

            if (rb != null && playerController != null)
            {
                // 중력 스케일을 원래대로 복구 (점진적으로)
                StartCoroutine(RestoreGravity(rb));

                // 이동 속도를 즉시 원래 속도로 복구
                playerController.moveSpeed = originalMoveSpeed;

                // 플레이어의 점프 중 중력 스케일도 원래 값으로 복구
                playerController.SetGravityScale(originalGravityScale);
            }
        }
    }

    private IEnumerator RestoreGravity(Rigidbody2D rb)
    {
        float elapsedTime = 0f;
        float restoreDuration = 1f;  // 중력 복구에 걸리는 시간

        while (elapsedTime < restoreDuration)
        {
            rb.gravityScale = Mathf.Lerp(rb.gravityScale, originalGravityScale, elapsedTime / restoreDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        rb.gravityScale = originalGravityScale;  // 완전히 복구
    }
}
