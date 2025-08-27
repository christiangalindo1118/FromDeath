using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(EnemyHealth))]
public class EnemyController : MonoBehaviour
{
    // Definición de los estados del enemigo
    public enum EnemyState
    {
        Patrolling,
        Chasing
    }

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float detectionRange = 5f;

    [Header("Combat Settings")]
    [SerializeField] private int contactDamage = 1;

    [Header("Patrol Settings")]
    [SerializeField] private float patrolSpeed = 1f;
    [SerializeField] private float patrolDistance = 3f; // Distancia a patrullar izquierda/derecha

    [Header("Chase Settings")]
    [SerializeField] private float loseSightRange = 7f; // Rango para perder de vista al jugador

    private Transform playerTransform;
    private Rigidbody2D rb;
    private EnemyHealth enemyHealth;
    private SpriteRenderer spriteRenderer;

    private EnemyState currentState; // Estado actual del enemigo
    private Vector2 patrolStartPoint;
    private bool movingRight = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        enemyHealth = GetComponent<EnemyHealth>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Nos suscribimos al evento de muerte para destruir el objeto.
        enemyHealth.OnDeath.AddListener(DestroyEnemy);

        currentState = EnemyState.Patrolling; // Estado inicial
        patrolStartPoint = transform.position; // Punto de inicio de la patrulla
    }

    void FixedUpdate()
    {
        // Ejecuta la lógica según el estado actual del enemigo
        switch (currentState)
        {
            case EnemyState.Patrolling:
                Patrol();
                break;
            case EnemyState.Chasing:
                Chase();
                break;
        }
    }

    private void Patrol()
    {
        // 1. Comprobar si el jugador está en rango de detección
        Collider2D playerCollider = Physics2D.OverlapCircle(transform.position, detectionRange, LayerMask.GetMask("Player"));
        if (playerCollider != null)
        {
            playerTransform = playerCollider.transform;
            Debug.Log("Enemy: Jugador detectado mientras patrullaba. Cambiando a Perseguir.");
            currentState = EnemyState.Chasing; // Cambiar a estado de persecución
            return; // Salir de Patrol para ir directamente a Chase en el siguiente FixedUpdate
        }

        // 2. Si no hay jugador detectado, continuar patrullando
        Debug.Log("Enemy: Patrullando...");
        float currentX = transform.position.x;

        if (movingRight)
        {
            rb.linearVelocity = new Vector2(patrolSpeed, rb.linearVelocity.y);
            if (spriteRenderer != null) spriteRenderer.flipX = false; // Mirar a la derecha
            if (currentX >= patrolStartPoint.x + patrolDistance)
            {
                movingRight = false; // Cambiar de dirección
            }
        }
        else
        {
            rb.linearVelocity = new Vector2(-patrolSpeed, rb.linearVelocity.y);
            if (spriteRenderer != null) spriteRenderer.flipX = true; // Mirar a la izquierda
            if (currentX <= patrolStartPoint.x - patrolDistance)
            {
                movingRight = true; // Cambiar de dirección
            }
        }
    }

    private void Chase()
    {
        // 1. Comprobar si el jugador objetivo es válido (no ha sido destruido)
        if (playerTransform == null)
        {
            Debug.Log("Enemy: Jugador objetivo destruido. Volviendo a Patrullar.");
            currentState = EnemyState.Patrolling; // Volver a patrullar
            rb.linearVelocity = Vector2.zero;
            return;
        }

        // 2. Calcular distancia al jugador
        float sqrDistanceToPlayer = (playerTransform.position - transform.position).sqrMagnitude;

        // 3. Si el jugador está dentro del rango de persecución, continuar persiguiendo
        if (sqrDistanceToPlayer <= loseSightRange * loseSightRange)
        {
            Debug.Log("Enemy: Persiguiendo al jugador. Distancia: " + Mathf.Sqrt(sqrDistanceToPlayer));
            Vector2 direction = (playerTransform.position - transform.position).normalized;
            rb.linearVelocity = new Vector2(direction.x * moveSpeed, rb.linearVelocity.y);
            if (spriteRenderer != null) spriteRenderer.flipX = direction.x < 0;
        }
        else // 4. El jugador está fuera del rango de persecución, volver a patrullar
        {
            Debug.Log("Enemy: Jugador fuera de rango de persecución. Volviendo a Patrullar.");
            currentState = EnemyState.Patrolling;
            rb.linearVelocity = Vector2.zero;
            playerTransform = null; // Olvidar al jugador
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow; // Color del círculo de detección
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red; // Color del círculo de perder de vista
        Gizmos.DrawWireSphere(transform.position, loseSightRange);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Si chocamos con el jugador, intentamos hacerle daño.
        if (collision.gameObject.CompareTag("Player"))
        { 
            if (collision.gameObject.TryGetComponent<PlayerHealth>(out PlayerHealth playerHealth))
            {
                playerHealth.TakeDamage(contactDamage);
            }
        }
    }

    // Este método se llama desde el evento OnDeath de EnemyHealth
    private void DestroyEnemy()
    {
        // Aquí podrías instanciar efectos de muerte, loot, etc.
        Destroy(gameObject);
    }
}
