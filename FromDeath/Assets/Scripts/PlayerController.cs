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

    private Rigidbody2D rb;
    private bool isGrounded;
    private float horizontalInput;

    void Start() => rb = GetComponent<Rigidbody2D>();

    void Update()
    {
        HandleInput();
        CheckGround();
        HandleJump();
        FlipSprite();
    }

    void FixedUpdate() => HandleMovement();

    private void HandleInput() => horizontalInput = Input.GetAxisRaw("Horizontal");

    private void CheckGround()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        Debug.Log($"Grounded: {isGrounded} | Jump Pressed: {Input.GetButtonDown("Jump")}");
    }

    private void HandleJump()
    {
        if(Input.GetButtonDown("Jump") && isGrounded)
        {
            Debug.Log("Jump triggered!");
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    private void FlipSprite()
    {
        if(horizontalInput != 0)
            transform.localScale = new Vector3(Mathf.Sign(horizontalInput), 1, 1);
    }

    private void HandleMovement() => 
        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}