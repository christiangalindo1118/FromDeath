using UnityEngine;
using System.Collections;

public class LevelExit : MonoBehaviour
{
    [SerializeField] private string nextLevel = "Level2";
    [SerializeField] private float transitionDelay = 1f;


    // Variable para cachear la espera y optimizar el rendimiento.
    private WaitForSeconds cachedWait;

    private void Awake()
    {
        // Inicializamos la espera una sola vez para no generar basura en memoria.
        cachedWait = new WaitForSeconds(transitionDelay);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        
        // Usamos la corutina para manejar la transición.

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        StartCoroutine(LoadNextLevel());
    }

    private IEnumerator LoadNextLevel()
    {

        // Usamos la variable cacheada en lugar de crear una nueva instancia.
        yield return cachedWait;

        // Comprobamos si el GameManager existe antes de llamarlo.
        if (GameManager.Instance != null)
        {
            // CORRECCIÓN: Usamos el nombre de método correcto 'LoadScene'.
            GameManager.Instance.LoadScene(nextLevel);
        }
        else
        {
            Debug.LogError("GameManager no encontrado en la escena. No se puede cambiar de nivel.");
        }

        yield return new WaitForSeconds(transitionDelay);
        if (GameManager.Instance != null)
            GameManager.Instance.LoadSceneWithFade(nextLevel);

    }

    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {

        Gizmos.color = new Color(0.5f, 0f, 1f, 0.5f); // Un color magenta más visible
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            Gizmos.DrawCube(collider.bounds.center, collider.bounds.size);
        }
    }
    #endif
}

        Gizmos.color = Color.magenta;
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
            Gizmos.DrawWireCube(transform.position, collider.bounds.size);
    }
    #endif
}

