using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI Configuration")]
    [SerializeField] private Text warningText;
    [SerializeField] private Animator fadeAnimator;

    [Header("Scene Transition Settings")]
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private AudioClip transitionSound;

    private AudioSource audioSource;

    void Awake()
    {
        ManageSingleton();
        InitializeComponents();
    }

    void ManageSingleton()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void InitializeComponents()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        if (fadeAnimator != null)
        {
            fadeAnimator.gameObject.SetActive(true);
        }
    }

    public void ShowWarning(string message)
    {
        if (warningText != null)
        {
            warningText.text = message;
            warningText.gameObject.SetActive(true);
        }
    }

    public void HideWarning()
    {
        if (warningText != null)
        {
            warningText.gameObject.SetActive(false);
        }
    }

    public void LoadSceneWithFade(string sceneName)
    {
        StartCoroutine(TransitionRoutine(sceneName));
    }

    private IEnumerator TransitionRoutine(string sceneName)
    {
        PlayTransitionSound();
        
        if (fadeAnimator != null)
        {
            fadeAnimator.SetTrigger("FadeOut");
            yield return new WaitForSeconds(fadeDuration);
        }
        
        SceneManager.LoadScene(sceneName);
        
        if (fadeAnimator != null)
        {
            fadeAnimator.SetTrigger("FadeIn");
        }
    }

    private void PlayTransitionSound()
    {
        if (transitionSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(transitionSound);
        }
    }

    // Método para cargar escenas directamente
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}