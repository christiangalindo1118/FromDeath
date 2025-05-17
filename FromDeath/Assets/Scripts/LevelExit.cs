using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;


public class LevelExit : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private string nextLevel = "Level2";
    [SerializeField] private float transitionDelay = 1f;
    [SerializeField] private AudioClip transitionSound;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(!other.CompareTag("Player")) return;

        StartCoroutine(LoadNextLevel());
    }

    private IEnumerator LoadNextLevel()
    {
        // Reproducir sonido de transición
        if(transitionSound != null)
            AudioSource.PlayClipAtPoint(transitionSound, transform.position);

        // Esperar el tiempo de transición
        yield return new WaitForSeconds(transitionDelay);

        // Cargar escena con transición
        TransitionManager.Instance.LoadSceneWithFade(nextLevel);
    }

    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Collider2D collider = GetComponent<Collider2D>();
        if(collider != null)
        {
            Gizmos.DrawWireCube(transform.position, collider.bounds.size);
        }
    }
    #endif
}