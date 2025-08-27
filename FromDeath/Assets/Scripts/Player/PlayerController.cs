using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 5f;
    
    [Header("Ground Detection")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;

    [Header("Audio")]
    [SerializeField] private AudioClip jumpSound;

    private bool isGrounded;
    private float horizontalInput;

    void Update()
    {
        HandleInput();
        CheckGround();
        HandleJump();
        FlipSprite();

        if (transform.position.y < -5f)
        {
            Player.Instance.Health.TakeDamage(999);
        }
    }

    void FixedUpdate() => HandleMovement();

    private void HandleInput() => horizontalInput = Input.GetAxisRaw("Horizontal");

    private void CheckGround() => 
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

    private void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Player.Instance.RB.linearVelocity = new Vector2(Player.Instance.RB.linearVelocity.x, jumpForce);
            // Reproducimos el sonido de salto
            AudioManager.Instance.PlaySFX(jumpSound);
        }
    }

    private void FlipSprite()
    {
        if (horizontalInput != 0)
            transform.localScale = new Vector3(Mathf.Sign(horizontalInput), 1, 1);
    }

    private void HandleMovement() => 
        Player.Instance.RB.linearVelocity = new Vector2(horizontalInput * moveSpeed, Player.Instance.RB.linearVelocity.y);

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}