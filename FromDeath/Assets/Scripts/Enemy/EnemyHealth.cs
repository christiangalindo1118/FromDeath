using UnityEngine;
using UnityEngine.Events;

// Este componente puede ser añadido a cualquier enemigo o entidad que necesite tener vida.
public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;

    // Eventos de instancia. Otros scripts en el mismo enemigo pueden suscribirse a ellos.
    public UnityEvent<float> OnHealthChanged = new UnityEvent<float>(); // Envía el porcentaje de vida (0.0 a 1.0)
    public UnityEvent OnDeath = new UnityEvent();

    private int currentHealth;
    private bool isDead = false;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damageAmount)
    {
        if (isDead || damageAmount <= 0) return;

        currentHealth = Mathf.Max(currentHealth - damageAmount, 0);

        // Notifica a los listeners (como la UI) del nuevo porcentaje de vida.
        OnHealthChanged?.Invoke((float)currentHealth / maxHealth);

        if (currentHealth <= 0)
        {
            isDead = true;
            // Anuncia que la entidad ha muerto.
            OnDeath?.Invoke();
        }
    }
}
