using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Config")]
    [SerializeField, Min(1)] private int maxHealth = 3;
    [SerializeField, Min(0)] private float invincibilityDuration = 1f;

    [Header("Audio")]
    [SerializeField] private AudioClip hurtSound;
    [SerializeField] private AudioClip deathSound;

    public int CurrentHealth => currentHealth;
    public static UnityEvent<int> OnHealthChanged = new UnityEvent<int>();
    public static UnityEvent OnPlayerDeath = new UnityEvent();

    private int currentHealth;
    private bool isInvincible; // Variable restaurada
    private Vector3 checkpointPosition;
    private WaitForSeconds invincibilityWait;

    private void Awake()
    {
        invincibilityWait = new WaitForSeconds(invincibilityDuration);
    }

    private void Start()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth);
    }

    public void TakeDamage(int damage)
    { 
        if (isInvincible || damage <= 0) return;

        currentHealth = Mathf.Max(currentHealth - damage, 0);
        OnHealthChanged?.Invoke(currentHealth);

        if (currentHealth > 0)
        {
            AudioManager.Instance.PlaySFX(hurtSound);
            StartCoroutine(InvincibilityFrames()); // Esta llamada ahora funcionará
        }
        else
        {
            Debug.Log("[PlayerHealth] Player has died. Calling TriggerGameOver...");
            AudioManager.Instance.PlaySFX(deathSound);
            OnPlayerDeath?.Invoke();
            GameManager.Instance.TriggerGameOver();
        }
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        OnHealthChanged?.Invoke(currentHealth);
    }

    // Corutina de invencibilidad restaurada
    private IEnumerator InvincibilityFrames()
    {
        isInvincible = true;
        yield return invincibilityWait;
        isInvincible = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Checkpoint"))
        {
            UpdateCheckpoint(other.transform.position);
        }
    }

    private void UpdateCheckpoint(Vector3 newPosition)
    {
        checkpointPosition = newPosition;
    }
}