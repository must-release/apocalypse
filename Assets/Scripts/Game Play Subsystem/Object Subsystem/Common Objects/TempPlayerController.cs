using UnityEngine;

public class TempPlayerController : MonoBehaviour
{
    public float moveSpeed = 5f; // �÷��̾� �̵� �ӵ�
    public float jumpForce = 10f; // ���� ��
    public Transform groundCheck; // �ٴ� üũ�� ���� ��ġ
    public float groundCheckRadius = 0.2f; // �ٴ� üũ�� ���� ���� ������
    public LayerMask groundLayer; // �ٴ� üũ�� ���� ���̾�
    public float fallMultiplier = 2.5f; // �ϰ� �� �߰� ��

    private Rigidbody2D rb;
    private bool isGrounded;
    private float horizontalMove;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        horizontalMove = Input.GetAxis("Horizontal") * moveSpeed;

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        // �ϰ� �� �߰����� ���� ����
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector2.down * fallMultiplier * Time.deltaTime;
        }
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(horizontalMove, rb.linearVelocity.y);
    }

    public void SetGravityScale(float newGravityScale)
    {
        rb.gravityScale = newGravityScale;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
