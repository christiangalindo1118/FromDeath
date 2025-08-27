using UnityEngine;
using UnityEngine.UI;

// Este script se coloca en el objeto del Canvas que contiene la barra de vida del jefe.
public class BossHealthUI : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private EnemyHealth bossHealth; // Referencia al script de vida del jefe
    [SerializeField] private Slider healthSlider;    // El slider de la UI
    [SerializeField] private GameObject bossHealthBarPanel; // El panel contenedor

    private void Awake()
    {
        if (bossHealth == null || healthSlider == null || bossHealthBarPanel == null)
        {
            Debug.LogError("Faltan referencias en BossHealthUI. Desactivando.", this);
            gameObject.SetActive(false);
            return;
        }

        // Ocultar la barra de vida al principio.
        bossHealthBarPanel.SetActive(false);
    }

    private void OnEnable()
    {
        // Nos suscribimos a los eventos del jefe.
        bossHealth.OnHealthChanged.AddListener(UpdateHealthSlider);
        bossHealth.OnDeath.AddListener(HideHealthBar);

        // Podríamos tener un evento OnBossFightStart para mostrar la barra.
        // Por ahora, la mostraremos con el primer cambio de vida.
    }

    private void OnDisable()
    {
        // Nos desuscribimos para evitar errores.
        bossHealth.OnHealthChanged.RemoveListener(UpdateHealthSlider);
        bossHealth.OnDeath.RemoveListener(HideHealthBar);
    }

    private void UpdateHealthSlider(float healthPercentage)
    {
        // Si la barra no está visible, la mostramos la primera vez que el jefe recibe daño.
        if (!bossHealthBarPanel.activeSelf)
        {
            bossHealthBarPanel.SetActive(true);
        }
        healthSlider.value = healthPercentage;
    }

    private void HideHealthBar()
    {
        bossHealthBarPanel.SetActive(false);
    }
}
