using UnityEngine;

public class DarkZoneController : MonoBehaviour
{
    [Header("Configuración Mortal")]
    [SerializeField] private GameObject warningUI;
    [SerializeField] private int darknessDamage = 1;
    [SerializeField] private float damageInterval = 1f;

    private bool isPlayerInDarkness;
    private PlayerHealth playerHealth;
    private float nextDamageTime;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // Usamos el Player.Instance para acceder a sus componentes de forma centralizada.
        // Esto es más robusto que GetComponent.
        playerHealth = Player.Instance.Health;
        
        // CORRECCIÓN: Preguntamos al componente correcto (LightBarrier) a través del Player.Instance.
        if (!Player.Instance.LightBarrier.IsBarrierActive)
        {
            ActivateDarkZoneEffects();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        
        DeactivateDarkZoneEffects();
        playerHealth = null; // Limpiamos la referencia al salir.
    }

    private void Update()
    {
        if (!isPlayerInDarkness) return;
        
        // Si el jugador activa la barrera MIENTRAS está dentro, lo detectamos.
        if (Player.Instance.LightBarrier.IsBarrierActive)
        {
            DeactivateDarkZoneEffects();
            return;
        }

        if (Time.time >= nextDamageTime)
        {
            ApplyDamage();
            nextDamageTime = Time.time + damageInterval;
        }
    }

    private void ActivateDarkZoneEffects()
    {
        isPlayerInDarkness = true;
        if (warningUI != null) warningUI.SetActive(true);
    }

    private void DeactivateDarkZoneEffects()
    {
        isPlayerInDarkness = false;
        if (warningUI != null) warningUI.SetActive(false);
    }

    private void ApplyDamage()
    {
        if (playerHealth != null)
        { 
            playerHealth.TakeDamage(darknessDamage);
        }
    }
}