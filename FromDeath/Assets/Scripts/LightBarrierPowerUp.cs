using UnityEngine;

public class LightBarrierPowerUp : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private float powerUpDuration = 10f; 

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Si el objeto que entra no es el jugador, no hacemos nada.
        if (!other.CompareTag("Player")) return;

        // Usamos el Singleton Player.Instance para acceder de forma segura y centralizada
        // al componente PlayerLightBarrier y llamar a su método público.
        Player.Instance.LightBarrier.ActivateBarrier(powerUpDuration);

        // Una vez que el power-up ha sido consumido, se destruye.
        Destroy(gameObject);
    }
}