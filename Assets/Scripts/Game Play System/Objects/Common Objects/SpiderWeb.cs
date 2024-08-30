// using System.Collections;
// using UnityEngine;

// public class SpiderWeb : MonoBehaviour
// {
//     public float slowDownFactor = 0.5f;  // �ӵ��� �󸶳� ������ ���� ������ ����
//     public float gravityScaleFactor = 0.5f;  // �߷� �������� �󸶳� ������ ����
//     private float originalGravityScale;
//     private float originalMoveSpeed;

//     void OnTriggerEnter2D(Collider2D collision)
//     {
//         if (collision.gameObject.CompareTag("Player"))
//         {
//             Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
//             PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();

//             if (rb != null && playerController != null)
//             {
//                 // ������ �߷� �����ϰ� �̵� �ӵ��� ����
//                 originalGravityScale = rb.gravityScale;
//                 originalMoveSpeed = playerController.moveSpeed;

//                 // �߷� �����ϰ� �̵� �ӵ��� �ٿ� �Ź����� ������ ��
//                 rb.gravityScale *= gravityScaleFactor;
//                 playerController.moveSpeed *= slowDownFactor;

//                 // ���� �� �߷� �������� ������������ �������� �ʵ��� �߷� ������ ����
//                 playerController.SetGravityScale(originalGravityScale);
//             }
//         }
//     }

//     void OnTriggerExit2D(Collider2D collision)
//     {
//         if (collision.gameObject.CompareTag("Player"))
//         {
//             Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
//             PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();

//             if (rb != null && playerController != null)
//             {
//                 // �߷� �������� ������� ���� (����������)
//                 StartCoroutine(RestoreGravity(rb));

//                 // �̵� �ӵ��� ��� ���� �ӵ��� ����
//                 playerController.moveSpeed = originalMoveSpeed;

//                 // �÷��̾��� ���� �� �߷� �����ϵ� ���� ������ ����
//                 playerController.SetGravityScale(originalGravityScale);
//             }
//         }
//     }

//     private IEnumerator RestoreGravity(Rigidbody2D rb)
//     {
//         float elapsedTime = 0f;
//         float restoreDuration = 1f;  // �߷� ������ �ɸ��� �ð�

//         while (elapsedTime < restoreDuration)
//         {
//             rb.gravityScale = Mathf.Lerp(rb.gravityScale, originalGravityScale, elapsedTime / restoreDuration);
//             elapsedTime += Time.deltaTime;
//             yield return null;
//         }

//         rb.gravityScale = originalGravityScale;  // ������ ����
//     }
// }
