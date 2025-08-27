using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(PlayerHealth))]
public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }

    // Propiedades para acceder a los componentes cacheados.
    public Rigidbody2D RB { get; private set; }
    public Animator Animator { get; private set; }
    public Collider2D Collider { get; private set; } // Añadido
    public PlayerController Controller { get; private set; }
    public PlayerHealth Health { get; private set; }
    public PlayerAttack Attack { get; private set; }
    public PlayerDash Dash { get; private set; }
    public PlayerLightBarrier LightBarrier { get; private set; }

    [Header("Game Over")]
    [SerializeField] private GameObject gameOverPanel;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Cacheamos todos los componentes del jugador una sola vez.
        RB = GetComponent<Rigidbody2D>();
        Animator = GetComponent<Animator>();
        Collider = GetComponent<Collider2D>(); // Añadido
        Controller = GetComponent<PlayerController>();
        Health = GetComponent<PlayerHealth>();
        Attack = GetComponent<PlayerAttack>();
        Dash = GetComponent<PlayerDash>();
        LightBarrier = GetComponent<PlayerLightBarrier>();
    }
}