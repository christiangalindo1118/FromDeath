using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Combat Settings")]
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private int contactDamage = 1;
    [SerializeField] private float attackCooldown = 1f;
    
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private bool showDetectionGizmo = true;

    [Header("Visual Settings")]
    [SerializeField] private bool flipSprite = true; // Activar/desactivar volteo de sprite
    private SpriteRenderer spriteRenderer;

    private Transform player;
    private Rigidbody2D rb;
    private int currentHealth;
    private float lastAttackTime;
    private Vector2 directionToPlayer;

    void Start()
    {
        InitializeComponents();
        currentHealth = maxHealth;
    }

    void FixedUpdate()
    {
        HandlePlayerDetection();
        HandleMovement();
    }

    private void InitializeComponents()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindWithTag("Player")?.transform;
        spriteRenderer = GetComponent<SpriteRenderer>(); // Nueva línea
        
        if (!player) Debug.LogWarning("Player not found in scene!");
        if (!rb) Debug.LogError("Rigidbody2D component missing!");
        if (flipSprite && !spriteRenderer) Debug.LogError("SpriteRenderer component missing!");
    }

    private void HandlePlayerDetection()
    {
        if (!player) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        directionToPlayer = (player.position - transform.position).normalized;

        if (distanceToPlayer <= detectionRange)
        {
            Debug.DrawLine(transform.position, player.position, Color.red);
        }
    }

    private void HandleMovement()
    {
        if (!player || Vector2.Distance(transform.position, player.position) > detectionRange)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        rb.linearVelocity = new Vector2(directionToPlayer.x * moveSpeed, rb.linearVelocity.y);

        FlipSprite();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"Enemy took {damage} damage. Remaining health: {currentHealth}");

        if (currentHealth <= 0)
        {
            DestroyEnemy();
        }
    }

    private void DestroyEnemy()
    {
        Debug.Log("Enemy destroyed");
        Destroy(gameObject);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;
        if (Time.time - lastAttackTime < attackCooldown) return;

        PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
        if (playerHealth)
        {
            playerHealth.TakeDamage(contactDamage);
            lastAttackTime = Time.time;
        }
    }

    void OnDrawGizmosSelected()
    {
        if (showDetectionGizmo && Application.isPlaying)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRange);
        }
    }

    private void FlipSprite()
    {
        if (!flipSprite || !spriteRenderer) return;

        // Voltear el sprite según la dirección del jugador
        if (directionToPlayer.x > 0) // Jugador a la derecha
        {
            spriteRenderer.flipX = false;
        }
        else if (directionToPlayer.x < 0) // Jugador a la izquierda
        {
            spriteRenderer.flipX = true;
        }
    }
}