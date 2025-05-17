using UnityEngine;
using System.Collections;

public class BreakablePlatform : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private float shakeDuration = 1f;
    [SerializeField] private float shakeIntensity = 0.1f;
    [SerializeField] private float destroyDelay = 2f;

    [Header("Referencias")]
    [SerializeField] private GameObject breakParticlesPrefab;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private Rigidbody2D rb;
    private Vector3 initialPosition;
    private bool isShaking = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Kinematic; // Configuración correcta del Body Type
        }
        initialPosition = transform.position;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !isShaking)
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (contact.normal.y < -0.5f)
                {
                    StartCoroutine(ShakeAndFall());
                    break;
                }
            }
        }
    }

    private IEnumerator ShakeAndFall()
    {
        isShaking = true;
        float elapsed = 0f;

        // Fase de temblor
        while (elapsed < shakeDuration)
        {
            transform.position = initialPosition + new Vector3(
                Random.Range(-shakeIntensity, shakeIntensity),
                Random.Range(-shakeIntensity, shakeIntensity),
                0
            );
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = initialPosition;

        // Activar física
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic; // Cambio correcto a cuerpo dinámico
        }

        // Instanciar partículas
        if (breakParticlesPrefab != null)
        {
            Instantiate(breakParticlesPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject, destroyDelay);
    }
}