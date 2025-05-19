using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private Image[] hearts;
    [SerializeField] private Sprite fullHeart, emptyHeart;
    
    [Header("References")]
    [SerializeField] private PlayerHealth playerHealth;

    private void OnEnable() => playerHealth.OnHealthChanged.AddListener(UpdateHearts);
    private void OnDisable() => playerHealth.OnHealthChanged.RemoveListener(UpdateHearts);

    private void Start()
    {
        InitializeHearts();
        UpdateHearts(playerHealth.CurrentHealth);
    }

    private void InitializeHearts()
    {
        for (int i = 0; i < hearts.Length; i++)
            hearts[i].gameObject.SetActive(i < playerHealth.MaxHealth);
    }

    private void UpdateHearts(int currentHealth)
    {
        for (int i = 0; i < hearts.Length; i++)
            hearts[i].sprite = i < currentHealth ? fullHeart : emptyHeart;
    }
}