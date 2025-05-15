using UnityEngine;

public class BreakablePlatform : MonoBehaviour
{
    [SerializeField] private float breakDelay = 0.5f; // Tiempo antes de romperse
    [SerializeField] private Sprite brokenSprite; // Sprite de la plataforma rota
    [SerializeField] private GameObject debrisParticles; // Efecto visual opcional

    private SpriteRenderer _spriteRenderer;
    private BoxCollider2D _collider;

    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _collider = GetComponent<BoxCollider2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Invoke(nameof(BreakPlatform), breakDelay);
        }
    }

    private void BreakPlatform()
    {
        _spriteRenderer.sprite = brokenSprite; // Cambia el sprite
        _collider.enabled = false; // Desactiva la colisión

        if (debrisParticles != null)
        {
            Instantiate(debrisParticles, transform.position, Quaternion.identity);
        }
    }
}
