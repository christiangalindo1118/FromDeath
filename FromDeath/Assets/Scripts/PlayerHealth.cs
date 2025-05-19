using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    #region Health Configuration
    [Header("Health Config")]
    [SerializeField, Min(1)] private int maxHealth = 3;
    [SerializeField, Min(0)] private float invincibilityTime = 1f;

    public int MaxHealth => maxHealth;
    public int CurrentHealth => currentHealth;
    #endregion

    #region Events
    [Header("Health Events")]
    public UnityEvent<int> OnHealthChanged = new UnityEvent<int>();
    public UnityEvent OnPlayerDeath = new UnityEvent();
    #endregion

    #region Debug Settings
    #if UNITY_EDITOR
    [Header("Debug Settings")]
    [SerializeField] private bool showCheckpointDebug = true;
    #endif
    #endregion

    #region Private State
    private int currentHealth;
    private bool isInvincible;
    private Vector3 checkpointPosition;
    #endregion

    #region Initialization
    private void Start() => InitializeHealthSystem();

    private void InitializeHealthSystem()
    {
        currentHealth = maxHealth;
        UpdateCheckpoint(transform.position);
        OnHealthChanged?.Invoke(currentHealth);
        
        #if UNITY_EDITOR
        Debug.Log($"[SYSTEM] Health initialized: {currentHealth}/{maxHealth}");
        #endif
    }
    #endregion

    #region Health Management
    public void TakeDamage(int damage)
    {
        if (isInvincible || damage <= 0) return;

        currentHealth = Mathf.Max(currentHealth - damage, 0);
        OnHealthChanged?.Invoke(currentHealth);
        
        #if UNITY_EDITOR
        Debug.Log($"[DAMAGE] Took {damage} damage | Remaining: {currentHealth}");
        #endif

        if (currentHealth > 0)
        {
            StartCoroutine(InvincibilityFrames());
        }
        else
        {
            OnPlayerDeath?.Invoke();
        }
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        OnHealthChanged?.Invoke(currentHealth);
    }
    #endregion

    #region Invincibility System
    private IEnumerator InvincibilityFrames()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibilityTime);
        isInvincible = false;
    }
    #endregion

    #region Checkpoint System
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Checkpoint")) return;

        UpdateCheckpoint(other.transform.position);
        
        #if UNITY_EDITOR
        VisualizeCheckpoint(other.transform.position);
        #endif
    }

    private void UpdateCheckpoint(Vector3 newPosition)
    {
        checkpointPosition = newPosition;
        
        #if UNITY_EDITOR
        if (showCheckpointDebug)
        {
            Debug.Log($"[CHECKPOINT] New position: {newPosition}");
            Debug.DrawLine(transform.position, newPosition, Color.cyan, 2f);
        }
        #endif
    }
    #endregion

    #region Debug Visualization
    #if UNITY_EDITOR
    private void VisualizeCheckpoint(Vector3 position)
    {
        if (!showCheckpointDebug) return;
        
        Debug.DrawLine(transform.position, position, Color.cyan, 2f);
    }

    private void OnDrawGizmosSelected()
    {
        if (!showCheckpointDebug) return;
        
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(checkpointPosition, 0.5f);
        Gizmos.DrawLine(transform.position, checkpointPosition);
    }
    #endif
    #endregion
}