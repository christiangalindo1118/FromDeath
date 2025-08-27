using UnityEngine;

// RECOMENDACIÓN: Renombrar este archivo a "ProjectileMovement.cs"
public class ProjectileMovement : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifetime = 3f;
    [SerializeField] private int damage = 20;

    void Start()
    {
        // Destruye el proyectil después de un tiempo para que no se acumulen en la escena.
        Destroy(gameObject, lifetime);
        // Asigna la velocidad inicial. El motor de física se encarga del resto.
        GetComponent<Rigidbody2D>().linearVelocity = transform.right * speed;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Si colisiona con el jugador, intenta hacerle daño.
        if(other.CompareTag("Player"))
        {
            // Optimizacion: Usamos TryGetComponent para más seguridad.
            if (other.TryGetComponent<PlayerHealth>(out PlayerHealth playerHealth))
            {
                playerHealth.TakeDamage(damage);
            }
            // El proyectil se destruye al impactar con el jugador.
            Destroy(gameObject);
        }
        // Podrías añadir lógica para que se destruya al chocar con el escenario también.
        // else if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        // {
        //     Destroy(gameObject);
        // }
    }
}