using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 5f;
    
    [Header("Ground Detection")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck; 
    [SerializeField] private float groundCheckRadius = 0.2f;

    [Header("Light Barrier System")]
    [SerializeField] private GameObject barrierEffectPrefab; // Prefab con VFX + Luz
    [SerializeField] private float barrierDuration = 10f;
    [SerializeField] private Light barrierLight; // Luz integrada
    

    private Rigidbody2D rb;
    private bool isGrounded;
    private float horizontalInput;
    private GameObject currentBarrier;
    private Coroutine activeBarrierCoroutine;
    public bool HasLightBarrier { get; private set; }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        InitializeBarrier();
    }

    void Update()
    {
        HandleInput();
        CheckGround();
        HandleJump();
        FlipSprite();
    }

    void FixedUpdate() => HandleMovement();

    #region Sistema de Barrera Luminosa
     public void ActivateLightBarrier(GameObject barrierInstance, float duration)
    {
        if (currentBarrier != null)
            Destroy(currentBarrier);

        currentBarrier = barrierInstance;

        if (duration > 0)
            Destroy(currentBarrier, duration);
    }

    private IEnumerator DeactivateBarrierAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(currentBarrier);
        currentBarrier = null;
        
    }

    private IEnumerator BarrierCountdown()
    {
        yield return new WaitForSeconds(barrierDuration);
        DeactivateLightBarrier();
    }

    private void DeactivateLightBarrier()
    {
        HasLightBarrier = false;
        Destroy(currentBarrier);
        if (barrierLight != null) barrierLight.enabled = false;
    }

    private void InitializeBarrier()
    {
        if (barrierLight != null) barrierLight.enabled = false;
    }
    #endregion

    #region Movimiento Básico
    private void HandleInput() => horizontalInput = Input.GetAxisRaw("Horizontal");

    private void CheckGround() => 
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

    private void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }

    private void FlipSprite()
    {
        if (horizontalInput != 0)
            transform.localScale = new Vector3(Mathf.Sign(horizontalInput), 1, 1);
    }

    private void HandleMovement() => 
        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
    #endregion

    #region Debug
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
    #endregion
}