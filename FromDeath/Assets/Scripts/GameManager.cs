using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class GameManager : MonoBehaviour
{
    #region Singleton
    public static GameManager Instance;
    private void Awake() => InitializeSingleton();
    
    private void InitializeSingleton()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    #region Scene Management
    [Header("Scene Configuration")]
    [SerializeField] private string[] levelNames = { "Opening", "Level1", "Level2", "Level3", "Ending" };
    
    [Header("Scene Transitions")]
    [SerializeField] private Animator fadeAnimator;
    [SerializeField] private float fadeDuration = 1f;

    public void LoadNextLevel()
    {
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentIndex < levelNames.Length - 1 ? currentIndex + 1 : 0);
    }

    public void LoadSceneByName(string sceneName)
    {
        if (System.Array.Exists(levelNames, name => name == sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError($"Scene {sceneName} not registered in levelNames array");
        }
    }

    public void LoadSceneWithFade(string sceneName)
    {
        StartCoroutine(TransitionRoutine(sceneName));
    }

    private IEnumerator TransitionRoutine(string sceneName)
    {
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
    #endregion

    #region Pause System
    [Header("Pause Configuration")]
    [SerializeField] private KeyCode pauseKey = KeyCode.P;
    [SerializeField] private GameObject pauseMenu;

    private void Update() => CheckPauseInput();

    private void CheckPauseInput()
    {
        if (Input.GetKeyDown(pauseKey))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        Time.timeScale = Time.timeScale == 0 ? 1 : 0;
        if (pauseMenu) pauseMenu.SetActive(!pauseMenu.activeSelf);
    }
    #endregion

    #region Game Flow
    [Header("Game Over Configuration")]
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private float gameOverDelay = 1.5f;

    public void TriggerGameOver()
    {
        StartCoroutine(ShowGameOverScreen());
    }

    private IEnumerator ShowGameOverScreen()
    {
        yield return new WaitForSeconds(gameOverDelay);
        if (gameOverScreen) gameOverScreen.SetActive(true);
        Time.timeScale = 0;
    }

    public void RestartGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    #endregion

    #region Button Actions
    // Panel de Pausa
    public void ResumeGame() => TogglePause();
    public void RestartLevel() => RestartGame();
    public void QuitToMainMenu() => LoadSceneByName("Opening");

    // Panel Game Over
    public void RetryLevel() => RestartGame();
    public void MainMenu() => LoadSceneByName("Opening");
    #endregion

    #region Player Events
    public void OnPlayerDeath() => TriggerGameOver();
    #endregion
}