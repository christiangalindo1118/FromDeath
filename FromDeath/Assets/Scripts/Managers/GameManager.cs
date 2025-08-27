using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                // CORRECCIÓN: Usamos el método moderno recomendado por Unity.
                _instance = FindFirstObjectByType<GameManager>();

                if (_instance == null)
                {
                    GameObject managerPrefab = Resources.Load<GameObject>("GameManager");
                    if (managerPrefab != null)
                    {
                        GameObject managerObject = Instantiate(managerPrefab);
                        _instance = managerObject.GetComponent<GameManager>();
                        _instance.name = "GameManager (Auto-Instantiated)";
                    }
                    else
                    {
                        Debug.LogError("FATAL ERROR: El Prefab 'GameManager' no se encontró en la carpeta 'Resources'.");
                    }
                }
            }
            return _instance;
        }
    }

    #region State & Events
    public bool IsPaused { get; private set; }
    public static event Action OnGamePause;
    public static event Action OnGameResume;
    public static event Action OnGameOver;
    public static event Action<string> OnLoadSceneStart;
    #endregion

    #region Scene Management
    [Header("Scene Settings")]
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private string menuSceneName = "MainMenu";
    #endregion

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        IsPaused = false;
        Time.timeScale = 1;
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(TransitionRoutine(sceneName));
    }

    private System.Collections.IEnumerator TransitionRoutine(string sceneName)
    {
        OnLoadSceneStart?.Invoke(sceneName);
        yield return new WaitForSecondsRealtime(fadeDuration);
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    #region Game Flow Control
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        Debug.Log($"[GameManager] TogglePause called. Current IsPaused: {IsPaused}");
        IsPaused = !IsPaused;
        if (IsPaused)
        { 
            Time.timeScale = 0;
            Debug.Log("[GameManager] Game Paused. Time.timeScale set to 0.");
            OnGamePause?.Invoke();
            Debug.Log("[GameManager] OnGamePause event invoked.");
        }
        else
        { 
            Time.timeScale = 1;
            Debug.Log("[GameManager] Game Resumed. Time.timeScale set to 1.");
            OnGameResume?.Invoke();
            Debug.Log("[GameManager] OnGameResume event invoked.");
        }
    }

    public void TriggerGameOver()
    {
        Debug.Log("[GameManager] TriggerGameOver called. Freezing time and invoking OnGameOver event.");
        Time.timeScale = 0;
        OnGameOver?.Invoke();
    }

    public void RestartGame()
    {
        LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ReturnToMenu()
    {
        LoadScene(menuSceneName);
    }
    #endregion

    #region Player Preferences
    public void SaveVolumeSettings(float masterVolume, float sfxVolume)
    {
        PlayerPrefs.SetFloat("MasterVolume", masterVolume);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        PlayerPrefs.Save();
    }

    public float GetMasterVolume()
    {
        return PlayerPrefs.GetFloat("MasterVolume", 1f);
    }

    public float GetSFXVolume()
    {
        return PlayerPrefs.GetFloat("SFXVolume", 1f);
    }
    #endregion

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}