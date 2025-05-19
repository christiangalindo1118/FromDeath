using UnityEngine;

public class LightBarrierPowerUp : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private GameObject barrierEffectPrefab; 
    [SerializeField] private float powerUpDuration = 10f; 

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerController player = other.GetComponent<PlayerController>();
        if (player == null) return;

        GameObject barrierInstance = Instantiate(
            barrierEffectPrefab, 
            player.transform.position, 
            Quaternion.identity, 
            player.transform
        );

        // Pasa ambos parámetros al método
        player.ActivateLightBarrier(barrierInstance, powerUpDuration); // ✅

        Destroy(gameObject);
    }
}
