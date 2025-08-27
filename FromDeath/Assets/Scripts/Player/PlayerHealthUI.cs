using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private Image[] hearts;
    [SerializeField] private Sprite fullHeart, emptyHeart;
    
    // ¡Ya no necesitamos una referencia directa al jugador!

    private void OnEnable()
    {
        // Nos suscribimos al evento estático de PlayerHealth.
        PlayerHealth.OnHealthChanged.AddListener(UpdateHearts);
    }

    private void OnDisable()
    {
        // Es crucial desuscribirse para evitar errores.
        PlayerHealth.OnHealthChanged.RemoveListener(UpdateHearts);
    }

    // Ya no necesitamos el método Start, la UI se actualizará por el evento.

    private void UpdateHearts(int currentHealth)
    {
        // La lógica para actualizar los corazones es la misma.
        // Primero, nos aseguramos de que el número de corazones coincida con la vida máxima.
        // Esto es opcional, pero hace la UI más adaptable si cambias la vida máxima.
        for (int i = 0; i < hearts.Length; i++)
        {
            // Asumiendo que maxHealth es el length del array, o podrías necesitar pasar maxHealth también.
            // Por ahora, lo mantenemos simple.
        }

        // Actualizamos el sprite de cada corazón.
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].sprite = i < currentHealth ? fullHeart : emptyHeart;
        }
    }
}
