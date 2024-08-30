using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f; // 플레이어 이동 속도
    public float jumpForce = 10f; // 점프 힘
    public Transform groundCheck; // 바닥 체크를 위한 위치
    public float groundCheckRadius = 0.2f; // 바닥 체크를 위한 원의 반지름
    public LayerMask groundLayer; // 바닥 체크를 위한 레이어
    public float fallMultiplier = 2.5f; // 하강 시 추가 힘

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
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

        // 하강 시 추가적인 힘을 가함
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.down * fallMultiplier * Time.deltaTime;
        }
    }

    void FixedUpdate()
    {
        rb.velocity = new Vector2(horizontalMove, rb.velocity.y);
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
