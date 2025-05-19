using UnityEngine;

public class DarkZoneController : MonoBehaviour
{
    [Header("Configuración Mortal")]
    [SerializeField] private GameObject warningUI;
    [SerializeField] private int darknessDamage = 1;
    [SerializeField] private float damageInterval = 1f; // Daño más frecuente
    [SerializeField] private float deathCheckInterval = 0.5f;

    [Header("Debug")]
    [SerializeField] private bool debugMode = true;

    private bool isPlayerInDarkness;
    private PlayerHealth playerHealth;
    private PlayerController playerController;
    private float nextDamageTime;
    private float nextDeathCheckTime;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        LogEnterTrigger(other);
        CachePlayerComponents(other.gameObject);
        
        if (HasInvalidComponents()) return;
        
        if (!playerController.HasLightBarrier)
            ActivateDarkZone();
        else if (debugMode)
            Debug.Log("Jugador protegido por Light Barrier");
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        
        LogExitTrigger();
        DeactivateDarkZone();
    }

    private void Update()
    {
        if (!isPlayerInDarkness) return;
        
        HandleDarkZoneEffects();
        CheckContinuousDamage();
        CheckImmediateDeath();
    }

    #region Métodos Principales
    private void CachePlayerComponents(GameObject player)
    {
        playerHealth = player.GetComponent<PlayerHealth>();
        playerController = player.GetComponent<PlayerController>();
        
        if (debugMode)
        {
            if (playerHealth == null) Debug.LogError("PlayerHealth no encontrado", player);
            if (playerController == null) Debug.LogError("PlayerController no encontrado", player);
        }
    }

    private void ActivateDarkZone()
    {
        isPlayerInDarkness = true;
        ToggleWarningUI(true);
        if (debugMode) Debug.Log("Zona oscura ACTIVADA");
    }

    private void DeactivateDarkZone()
    {
        isPlayerInDarkness = false;
        ToggleWarningUI(false);
    }

    private void HandleDarkZoneEffects()
    {
        if (playerController.HasLightBarrier)
        {
            if (debugMode) Debug.Log("Light Barrier activada DENTRO de la zona");
            DeactivateDarkZone();
        }
    }
    #endregion

    #region Sistema de Daño y Muerte
    private void CheckContinuousDamage()
    {
        if (Time.time >= nextDamageTime)
        {
            ApplyDamage();
            nextDamageTime = Time.time + damageInterval;
        }
    }

    private void CheckImmediateDeath()
    {
        if (playerHealth == null) return;

        if (playerHealth.CurrentHealth <= 0 && Time.time >= nextDeathCheckTime)
        {
            Debug.LogWarning("¡Jugador MUERTO en zona oscura!");
            //playerHealth.ForceDeath();
            nextDeathCheckTime = Time.time + deathCheckInterval;
        }
    }

    private void ApplyDamage()
    {
        if (playerHealth == null) return;
    
        playerHealth.TakeDamage(darknessDamage);
    
        if (debugMode)
        {
            Debug.Log($"Daño aplicado: {darknessDamage}");
            Debug.Log($"Vida restante: {playerHealth.CurrentHealth}");
        }
    }
    #endregion

    #region Utilidades
    private bool HasInvalidComponents() => playerHealth == null || playerController == null;

    private void ToggleWarningUI(bool state)
    {
        if (warningUI != null)
            warningUI.SetActive(state);
        else
            Debug.LogError("UI de advertencia no asignada!", this);
    }

    private void LogEnterTrigger(Collider2D other)
    {
        if (!debugMode) return;
        Debug.Log($"Entró: {other.name} (Tag: {other.tag})");
        Debug.Log($"Componentes: Salud [{(playerHealth != null ? "OK" : "FALTA")}], Controlador [{(playerController != null ? "OK" : "FALTA")}]");
    }

    private void LogExitTrigger()
    {
        if (!debugMode) return;
        Debug.Log("Jugador SALIÓ de la zona");
    }
    #endregion

    #region Gizmos
    void OnDrawGizmos()
    {
        if (warningUI == null)
            Debug.LogWarning("Asignar UI de advertencia!", this);
    }
    #endregion
}