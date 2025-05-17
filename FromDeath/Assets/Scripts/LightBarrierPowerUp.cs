using UnityEngine;

public class LightBarrierPowerUp : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private GameObject pickupEffect; // Efecto al recolectar (partículas, sonido, etc.)

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.UnlockLightBarrier(); // Desbloquea la habilidad
                if (pickupEffect != null)
                {
                    Instantiate(pickupEffect, transform.position, Quaternion.identity);
                }
                Destroy(gameObject); // Destruye el power-up
            }
        }
    }
}
