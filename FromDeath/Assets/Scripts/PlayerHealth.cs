using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] int maxHealth = 3;
    [SerializeField] float invincibilityTime = 1f;

    private int currentHealth;
    private bool isInvincible = false;
    private Vector3 checkpointPosition;

    void Start()
    {
        currentHealth = maxHealth;
        checkpointPosition = transform.position;
        Debug.Log($"Salud inicial: {currentHealth}/{maxHealth} | Checkpoint inicial: {checkpointPosition}");
    }

    public void TakeDamage(int damage)
    {
        if (!isInvincible)
        {
            currentHealth -= damage;
            Debug.Log($"Daño recibido: -{damage} | Salud actual: {currentHealth}/{maxHealth}");
            
            StartCoroutine(InvincibilityFrames());

            if (currentHealth <= 0)
            {
                Debug.LogWarning("¡Salud agotada! Respawneando...");
                Respawn();
            }
        }
        else
        {
            Debug.Log("Invencibilidad activa: Daño bloqueado");
        }
    }

    IEnumerator InvincibilityFrames()
    {
        isInvincible = true;
        Debug.Log("Invencibilidad ACTIVADA");
        
        yield return new WaitForSeconds(invincibilityTime);
        
        isInvincible = false;
        Debug.Log("Invencibilidad DESACTIVADA");
    }

    void Respawn()
    {
        transform.position = checkpointPosition;
        currentHealth = maxHealth;
        Debug.LogWarning($"Respawn completado. Nueva posición: {checkpointPosition} | Salud restaurada: {currentHealth}/{maxHealth}");
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Checkpoint"))
        {
            checkpointPosition = other.transform.position;
            Debug.Log($"Checkpoint actualizado! Nueva posición: {checkpointPosition}");
            
            // Opcional: Destacar checkpoints activos
            Debug.DrawLine(transform.position, checkpointPosition, Color.cyan, 2f);
        }
    }
}