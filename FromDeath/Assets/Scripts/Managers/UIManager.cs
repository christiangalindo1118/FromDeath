using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject hudPanel;              
    [SerializeField] private GameObject mainMenuPanel;         
    [SerializeField] private GameObject optionsMenuPanel;      
    [SerializeField] private GameObject pausePanel;        
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject transitionPanel;         

    [Header("Health UI")]
    [SerializeField] private Image[] heartImages; // Array para las imágenes de los corazones

    [Header("Transitions")]
    [SerializeField] private Animator transitionAnim; // Animator con trigger "transitionAnim"

    [Header("Debug")]
    [SerializeField] private bool enableDebugLogs = true;

    private string currentSceneName;
    private bool isGameScene;

    #region Unity Lifecycle
    private void Awake()
    {
        currentSceneName = SceneManager.GetActiveScene().name;
        isGameScene = !currentSceneName.Contains("Menu") && !currentSceneName.Contains("MainMenu");

        // 🔧 Autodetección de corazones si no están asignados manualmente
        if ((heartImages == null || heartImages.Length == 0) && hudPanel != null)
        {
            heartImages = hudPanel.GetComponentsInChildren<Image>(true);
            LogDebug($"Heart Images auto-asigned. Found {heartImages.Length} hearts in HUD.");
        }

        InitializePanelStates();
        LogDebug($"UIManager initialized in scene: {currentSceneName}. Is game scene: {isGameScene}");
    }

    private void OnEnable()
    {
        GameManager.OnGamePause += ShowPauseMenu;
        GameManager.OnGameResume += HidePauseMenu;
        GameManager.OnGameOver += ShowGameOverPanel;
        
        PlayerHealth.OnHealthChanged.AddListener(UpdateHealthUI);

        LogDebug("UIManager events subscribed.");
    }

    private void OnDisable()
    {
        GameManager.OnGamePause -= ShowPauseMenu;
        GameManager.OnGameResume -= HidePauseMenu;
        GameManager.OnGameOver -= ShowGameOverPanel;
        
        PlayerHealth.OnHealthChanged.RemoveListener(UpdateHealthUI);

        LogDebug("UIManager events unsubscribed.");
    }

    private void Start()
    {
        ConfigurePanelsForScene();
        LogDebug("UIManager Start completed.");
    }
    #endregion

    #region Panel Initialization
    private void InitializePanelStates()
    {
        SetPanelActive(optionsMenuPanel, false);
        SetPanelActive(pausePanel, false);
        SetPanelActive(gameOverPanel, false);
        SetPanelActive(transitionPanel, false);
    }

    private void ConfigurePanelsForScene()
    {
        if (isGameScene)
        {
            SetPanelActive(hudPanel, true);
            SetPanelActive(transitionPanel, false);
            LogDebug("Panels configured for game scene.");
        }
        else
        {
            SetPanelActive(mainMenuPanel, true);
            if (hudPanel != null && hudPanel.activeSelf)
            {
                SetPanelActive(hudPanel, false);
            }
            SetPanelActive(transitionPanel, false);
            LogDebug("Panels configured for main menu scene.");
        }
    }
    #endregion

    #region Event Handlers
    private void ShowPauseMenu()
    {
        LogDebug("ShowPauseMenu called.");
        if (pausePanel == null)
        {
            LogError("PausePanel reference is null!");
            return;
        }

        SetPanelActive(pausePanel, true);
    }

    private void HidePauseMenu()
    {
        LogDebug("HidePauseMenu called.");
        if (pausePanel == null)
        {
            LogError("PausePanel reference is null!");
            return;
        }

        SetPanelActive(pausePanel, false);

        if (isGameScene && hudPanel != null)
        {
            SetPanelActive(hudPanel, true);
        }
    }

    private void ShowGameOverPanel()
    {
        LogDebug("ShowGameOverPanel called.");
        if (gameOverPanel == null)
        {
            LogError("GameOverPanel reference is null!");
            return;
        }

        SetPanelActive(gameOverPanel, true);
        SetPanelActive(hudPanel, false);
    }

    private void UpdateHealthUI(int currentHealth)
    {
        LogDebug($"UpdateHealthUI called. Current Health: {currentHealth}");
        if (heartImages == null || heartImages.Length == 0)
        {
            LogError("Heart Images array is not assigned and auto-detect failed!");
            return;
        }

        for (int i = 0; i < heartImages.Length; i++)
        {
            if (heartImages[i] != null)
            {
                heartImages[i].gameObject.SetActive(i < currentHealth);
            }
        }
    }
    #endregion

    #region Button Handlers
    public void OnPlayButtonPressed() 
    {
        LogDebug("Play button pressed.");
        PlayTransition();
        GameManager.Instance?.LoadScene("Level1");
    }
    
    public void OnOptionsButtonPressed()
    {
        LogDebug("Options button pressed.");
        SetPanelActive(optionsMenuPanel, true);
        SetPanelActive(mainMenuPanel, false);
    }
    
    public void OnBackFromOptionsPressed()
    {
        LogDebug("Back from options button pressed.");
        SetPanelActive(optionsMenuPanel, false);
        SetPanelActive(mainMenuPanel, true);
    }
    
    public void OnResumeButtonPressed() 
    {
        LogDebug("Resume button pressed.");
        GameManager.Instance?.TogglePause();
    }
    
    public void OnRestartButtonPressed() 
    {
        LogDebug("Restart button pressed.");
        PlayTransition();
        GameManager.Instance?.RestartGame();
    }
    
    public void OnMenuButtonPressed() 
    {
        LogDebug("Menu button pressed.");
        PlayTransition();
        GameManager.Instance?.ReturnToMenu();
    }
    #endregion

    #region Transition Management
    private void PlayTransition()
    {
        SetPanelActive(transitionPanel, true);
        SetAnimatorTrigger("transitionAnim");
    }

    public void ShowTransitionPanel()
    {
        SetPanelActive(transitionPanel, true);
    }

    public void HideTransitionPanel()
    {
        SetPanelActive(transitionPanel, false);
    }

    public bool IsTransitionPanelActive()
    {
        return transitionPanel != null && transitionPanel.activeSelf;
    }
    #endregion

    #region Utility Methods
    private void SetPanelActive(GameObject panel, bool active)
    {
        if (panel == null) return;
        if (panel.activeSelf != active)
        {
            panel.SetActive(active);
            LogDebug($"Panel '{panel.name}' set to {(active ? "active" : "inactive")}.");
        }
    }

    private void SetAnimatorTrigger(string triggerName)
    {
        if (transitionAnim == null)
        {
            LogError($"Transition Animator is not assigned for trigger '{triggerName}'.");
            return;
        }

        if (transitionAnim.runtimeAnimatorController == null)
        {
            LogError($"Animator '{transitionAnim.name}' doesn't have an AnimatorController assigned.");
            return;
        }

        transitionAnim.SetTrigger(triggerName);
        LogDebug($"Animator trigger '{triggerName}' set successfully.");
    }

    private void LogDebug(string message)
    {
        if (enableDebugLogs)
        {
            Debug.Log($"[UIManager] {message}");
        }
    }

    private void LogError(string message)
    {
        Debug.LogError($"[UIManager] {message}");
    }
    #endregion

    #region Public Methods
    public void RefreshPanelConfiguration()
    {
        currentSceneName = SceneManager.GetActiveScene().name;
        isGameScene = !currentSceneName.Contains("Menu") && !currentSceneName.Contains("MainMenu");
        ConfigurePanelsForScene();
    }

    public bool IsPanelActive(GameObject panel)
    {
        return panel != null && panel.activeSelf;
    }

    public void HideMainMenu()
    {
        if (mainMenuPanel != null)
        {
            SetPanelActive(mainMenuPanel, false);
        }
    }

    public void ShowMainMenu()
    {
        if (mainMenuPanel != null)
        {
            SetPanelActive(mainMenuPanel, true);
        }
    }
    #endregion
}

