using UnityEngine;
using System.Collections;

public class LevelExit : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private string nextLevel = "Level2";
    [SerializeField] private float transitionDelay = 1f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(!other.CompareTag("Player")) return;

        StartCoroutine(LoadNextLevel());
    }

    private IEnumerator LoadNextLevel()
    {
        yield return new WaitForSeconds(transitionDelay);
        GameManager.Instance.LoadSceneWithFade(nextLevel);
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