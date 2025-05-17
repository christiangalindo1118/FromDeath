using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Config")]
    [SerializeField, Min(1)] private int maxHealth = 3;
    [SerializeField, Min(0)] private float invincibilityTime = 1f;

    #if UNITY_EDITOR
    [Header("Debug Settings")]
    [SerializeField] private bool showCheckpointDebug = true;
    #endif

    private int currentHealth;
    private bool isInvincible;
    private Vector3 checkpointPosition;

    public int CurrentHealth => currentHealth;

    private void Start()
    {
        InitializeHealthSystem();
    }

    #region Initialization
    private void InitializeHealthSystem()
    {
        currentHealth = maxHealth;
        UpdateCheckpoint(transform.position);
        
        #if UNITY_EDITOR
        Debug.Log("[SYSTEM] Health initialized: " + currentHealth + "/" + maxHealth);
        #endif
    }
    #endregion

    #region Damage System
    public void TakeDamage(int damage)
    {
        if (isInvincible || damage <= 0) return;

        currentHealth = Mathf.Max(currentHealth - damage, 0);
        
        #if UNITY_EDITOR
        Debug.Log("[DAMAGE] Took " + damage + " damage | Remaining: " + currentHealth);
        #endif

        StartCoroutine(InvincibilityFrames());
        CheckRespawn();
    }

    private IEnumerator InvincibilityFrames()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibilityTime);
        isInvincible = false;
    }
    #endregion

    #region Respawn System
    public void ForceDeath()
    {
        currentHealth = 0;
        Respawn();
    }

    private void CheckRespawn()
    {
        if (currentHealth > 0) return;
        Respawn();
    }

    private void Respawn()
    {
        transform.position = checkpointPosition;
        currentHealth = maxHealth;
        
        #if UNITY_EDITOR
        Debug.LogWarning("[RESPAWN] Player respawned at checkpoint");
        #endif
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
            Debug.Log("[CHECKPOINT] New position: " + newPosition);
            Debug.DrawLine(transform.position, newPosition, Color.cyan, 2f);
        }
        #endif
    }
    #endregion

    #region Editor Visualizations
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