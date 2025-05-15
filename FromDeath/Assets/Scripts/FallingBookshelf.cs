using UnityEngine;

public class FallingBookshelf : MonoBehaviour
{
    [SerializeField] private Sprite fallenSprite; // Sprite caído
    [SerializeField] private float fallDelay = 0.3f; // Tiempo antes de caer
    [SerializeField] private float destroyDelay = 2f; // Tiempo antes de desaparecer

    private SpriteRenderer _spriteRenderer;
    private bool _isFalling = false;

    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!_isFalling && other.CompareTag("Player"))
        {
            _isFalling = true;
            Invoke(nameof(Fall), fallDelay);
        }
    }

    private void Fall()
    {
        _spriteRenderer.sprite = fallenSprite; // Cambia el sprite
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic; // Activa gravedad
        Destroy(gameObject, destroyDelay); // Destruye después de un tiempo
    }
}
