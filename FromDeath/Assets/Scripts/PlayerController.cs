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

    [Header("Light Barrier")]
    public bool HasLightBarrier { get; private set; }
    private Coroutine barrierCoroutine;

    [SerializeField] private GameObject lightBarrierVFX; // Efecto visual de la barrera
    [SerializeField] private float barrierDuration = 10f;
    
    private Rigidbody2D rb;
    private bool isGrounded;
    private float horizontalInput;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (lightBarrierVFX != null)
            lightBarrierVFX.SetActive(false); // Asegurar que empiece desactivado
    }

    void Update()
    {
        HandleInput();
        CheckGround();
        HandleJump();
        FlipSprite();
        
    }

    void FixedUpdate() => HandleMovement();

    public void UnlockLightBarrier()
    {
        HasLightBarrier = true;
        if (lightBarrierVFX != null)
            lightBarrierVFX.SetActive(true); // Activar efecto visual
        
        Debug.Log("¡Light Barrier activada!");
        // Aquí podrías añadir también un sonido: AudioSource.PlayClipAtPoint(...)
    }

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

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }

    

    private IEnumerator ActivateBarrier()
    {
        HasLightBarrier = true;
        
        if (lightBarrierVFX != null)
        {
            lightBarrierVFX.SetActive(true);
            Debug.Log("Barrera luminosa ACTIVADA");
        }
        
        yield return new WaitForSeconds(barrierDuration);
        
        DeactivateBarrier();
    }

    private void DeactivateBarrier()
    {
        HasLightBarrier = false;
        
        if (lightBarrierVFX != null)
        {
            lightBarrierVFX.SetActive(false);
            Debug.Log("Barrera luminosa DESACTIVADA");
        }
    }
    
}