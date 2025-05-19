using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class GameManager : MonoBehaviour 
{
    #region Singleton
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            SceneManager.sceneLoaded += OnSceneLoaded;
            DontDestroyOnLoad(gameObject); // ✅ Persistir
        }
        else
        {
            Destroy(gameObject); // ✅ Destruir copias
        }
    }

    private void OnDestroy()
    {
        SavePlayerPreferences();
        SceneManager.sceneLoaded -= OnSceneLoaded;
        Debug.Log("GameManager destruido");
    }

    private void ForceActivation()
    {
        // Prevent deactivation at runtime
        if (!gameObject.activeSelf) 
        {
            gameObject.SetActive(true);
            Debug.LogWarning("GameManager reactivado forzosamente");
        }
    }

    // Safe SetActive method for GameManager
    public void SetActive(bool state)
    {
        if (!state)
        {
            Debug.LogError("No se puede desactivar el GameManager");
            return;
        }
        gameObject.SetActive(state); // Only allows activation
    }
    #endregion

    #region Scene Management
    [Header("Scene Transitions")]
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private string menuScene = "MainMenu";
    private Animator fadeAnimator;

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"[SCENE] Escena cargada: {scene.name}");
        ForceActivation();
        Time.timeScale = 1;
        
        // Las referencias UI deben buscarse después de asegurar que el GameManager está activo
        FindUIReferences();
        
        // Inicializar características específicas de la escena
        InitializeSceneFeatures();
    }

    private void InitializeSceneFeatures()
    {
        // Primero inicializar UI básica
        InitializeSceneUI();
        
        // Configurar elementos específicos de la escena
        string sceneName = SceneManager.GetActiveScene().name;
        Debug.Log($"Inicializando características específicas para escena: {sceneName}");
        
        if (sceneName == "EndingScene")
        {
            SetupEndingScene();
        }
    }

    public void LoadSceneWithFade(string sceneName)
    {
        if (!gameObject.activeInHierarchy)
        {
            Debug.LogError("GameManager inactivo. Reactivando...");
            gameObject.SetActive(true);
        }
        StartCoroutine(TransitionRoutine(sceneName));
    }

    private IEnumerator TransitionRoutine(string sceneName)
    {
        // 1. Validate critical components
        if (fadeAnimator == null) FindFadeAnimator();

        // 2. Execute transition
        if (fadeAnimator != null)
        {
            fadeAnimator.SetTrigger("FadeOut");
            yield return new WaitForSecondsRealtime(fadeDuration);
        }

        // 3. Load scene asynchronously
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;

        while (!asyncLoad.isDone)
        {
            if (asyncLoad.progress >= 0.9f)
            {
                asyncLoad.allowSceneActivation = true;
            }
            yield return null;
        }

        // 4. Post-load
        Debug.Log($"[SCENE] Escena {sceneName} cargada exitosamente");
        if (fadeAnimator != null) fadeAnimator.SetTrigger("FadeIn");
    }
    #endregion

    #region UI Management
    [Header("UI References")]
    [SerializeField] private string optionsMenuTag = "OptionsMenu";
    [SerializeField] private string mainMenuTag = "MainMenuPanel";
    [SerializeField] private string fadeAnimatorTag = "FadeAnimator";
    [SerializeField] private string gameOverTag = "GameOverScreen";
    [SerializeField] private string pauseMenuTag = "PauseMenu";

    private GameObject mainMenuPanel;
    private GameObject optionsMenu;
    private GameObject gameOverScreen;
    private GameObject pauseMenu;
    private Slider masterVolumeSlider;
    private Slider sfxVolumeSlider;
    private TMP_Dropdown languageDropdown;

    private void FindUIReferences()
    {
        // Buscar objetos principales por tag
        mainMenuPanel = GameObject.FindGameObjectWithTag(mainMenuTag);
        optionsMenu = GameObject.FindGameObjectWithTag(optionsMenuTag);
        gameOverScreen = GameObject.FindGameObjectWithTag(gameOverTag);
        pauseMenu = GameObject.FindGameObjectWithTag(pauseMenuTag);

        // Ver si las referencias se encontraron correctamente
        Debug.Log($"MainMenuPanel encontrado: {mainMenuPanel != null}");
        Debug.Log($"OptionsMenu encontrado: {optionsMenu != null}");
        Debug.Log($"GameOverScreen encontrado: {gameOverScreen != null}");
        Debug.Log($"PauseMenu encontrado: {pauseMenu != null}");

        // IMPORTANTE: No fallar si las referencias son nulas
        // Algunas escenas no tendrán todos los paneles
        
        // Configurar elementos del menú de opciones sólo si existe
        if (optionsMenu != null)
        {
            // Buscar componentes utilizando Transform.Find que es más seguro que GetComponent
            Transform masterSliderTransform = optionsMenu.transform.Find("MasterVolumeSlider");
            Transform sfxSliderTransform = optionsMenu.transform.Find("SFXVolumeSlider");
            Transform languageDropdownTransform = optionsMenu.transform.Find("LanguageDropdown");
            
            // Obtener componentes si se encontraron los objetos
            masterVolumeSlider = masterSliderTransform != null ? masterSliderTransform.GetComponent<Slider>() : null;
            sfxVolumeSlider = sfxSliderTransform != null ? sfxSliderTransform.GetComponent<Slider>() : null;
            languageDropdown = languageDropdownTransform != null ? languageDropdownTransform.GetComponent<TMP_Dropdown>() : null;
            
            Debug.Log($"Sliders encontrados: {masterVolumeSlider != null}, {sfxVolumeSlider != null}");
            Debug.Log($"Dropdown encontrado: {languageDropdown != null}");
            
            // Configurar solo si hay elementos válidos
            if (masterVolumeSlider != null || sfxVolumeSlider != null || languageDropdown != null)
            {
                ConfigureOptionsMenu();
            }
        }
    }

    private void InitializeSceneUI()
    {
        try {
            string sceneName = SceneManager.GetActiveScene().name;
            Debug.Log($"Inicializando UI para escena: {sceneName}");
            
            // Manejar específicamente la escena Opening
            if (sceneName == "Opening")
            {
                if (mainMenuPanel != null) 
                {
                    mainMenuPanel.SetActive(true);
                    Debug.Log("MainMenuPanel activado");
                }
                
                if (optionsMenu != null) 
                {
                    optionsMenu.SetActive(false);
                    Debug.Log("OptionsMenu desactivado");
                }
                
                Debug.Log("UI de Opening inicializada");
            }
            
            // Siempre ocultar estos elementos al cargar una escena
            if (gameOverScreen != null) 
            {
                gameOverScreen.SetActive(false);
                Debug.Log("GameOverScreen desactivado");
            }
            
            if (pauseMenu != null) 
            {
                pauseMenu.SetActive(false);
                Debug.Log("PauseMenu desactivado");
            }
        }
        catch (System.Exception e) {
            Debug.LogError($"Error en InitializeSceneUI: {e.Message}");
            // Continuar ejecución, no detener por un error en UI
        }
    }

    private void ConfigureOptionsMenu()
    {
        Debug.Log("Configurando panel de opciones...");
        
        try {
            // Configure Sliders
            if (masterVolumeSlider != null)
            {
                // Eliminar listeners anteriores para evitar duplicados
                masterVolumeSlider.onValueChanged.RemoveAllListeners();
                masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
                masterVolumeSlider.value = AudioListener.volume;
                Debug.Log("Slider de volumen maestro configurado");
            }

            if (sfxVolumeSlider != null)
            {
                // Eliminar listeners anteriores para evitar duplicados
                sfxVolumeSlider.onValueChanged.RemoveAllListeners();
                sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);
                sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);
                Debug.Log("Slider de SFX configurado");
            }

            // Configure language dropdown
            if (languageDropdown != null)
            {
                // Eliminar listeners anteriores para evitar duplicados
                languageDropdown.onValueChanged.RemoveAllListeners();
                languageDropdown.onValueChanged.AddListener(SetLanguage);
                languageDropdown.value = PlayerPrefs.GetInt("Language", 0);
                Debug.Log("Dropdown de idiomas configurado");
            }
        }
        catch (System.Exception e) {
            Debug.LogError($"Error al configurar menú de opciones: {e.Message}");
            // Continuar ejecución, no detener por un error en UI
        }
    }
    #endregion

    #region Options System
    public void OpenOptions()
    {
        Debug.Log("Botón Options presionado");
        ToggleOptionsMenu(true);
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
    }

    public void CloseOptions()
    {
        Debug.Log("Botón Cerrar Options presionado");
        ToggleOptionsMenu(false);
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
    }

    private void ToggleOptionsMenu(bool state)
    {
        if (optionsMenu != null)
        {
            optionsMenu.SetActive(state);
            Debug.Log($"OptionsMenu activado: {state}");
        }
    }

    // Methods for options panel controls
    private void SetMasterVolume(float volume)
    {
        AudioListener.volume = volume;
        PlayerPrefs.SetFloat("MasterVolume", volume);
        Debug.Log($"Volumen maestro actualizado: {volume}");
    }

    private void SetSFXVolume(float volume)
    {
        PlayerPrefs.SetFloat("SFXVolume", volume);
        Debug.Log($"Volumen SFX actualizado: {volume}");
        // Implement specific SFX logic here
    }

    private void SetLanguage(int index)
    {
        PlayerPrefs.SetInt("Language", index);
        Debug.Log($"Idioma seleccionado: {languageDropdown.options[index].text}");
        // Implement language change logic here
    }

    public void OnPlayButtonClicked()
    {
        Debug.Log("Botón Play presionado");
        LoadSceneWithFade("Level1");
    }
    #endregion

    #region Player Preferences
    private void LoadPlayerPreferences()
    {
        // Load master volume
        float masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        AudioListener.volume = masterVolume;

        // Load SFX volume (if you have a separate system)
        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
    
        // Load selected language
        int languageIndex = PlayerPrefs.GetInt("Language", 0);
    
        Debug.Log("Preferencias del jugador cargadas");
    }

    private void SavePlayerPreferences()
    {
        PlayerPrefs.SetFloat("MasterVolume", AudioListener.volume);
        
        // Save current SFX volume if slider exists
        if (sfxVolumeSlider != null) {
            PlayerPrefs.SetFloat("SFXVolume", sfxVolumeSlider.value);
        }
        
        // Save current language if dropdown exists
        if (languageDropdown != null) {
            PlayerPrefs.SetInt("Language", languageDropdown.value);
        }
        
        PlayerPrefs.Save();
        Debug.Log("Preferencias guardadas");
    }
    #endregion

    #region Game Flow
    [Header("Game Over")]
    [SerializeField] private float gameOverDelay = 1f;

    public void TriggerGameOver()
    {
        Debug.Log("Activando Game Over");
        StartCoroutine(ShowGameOverScreen());
    }

    private IEnumerator ShowGameOverScreen()
    {
        Debug.Log("Mostrando pantalla de Game Over");
        yield return new WaitForSecondsRealtime(gameOverDelay);
        
        if (gameOverScreen != null)
        {
            gameOverScreen.SetActive(true);
            Time.timeScale = 0;
        }
    }

    public void RestartGame()
    {
        Debug.Log("Reiniciando juego");
        Time.timeScale = 1;
        LoadSceneWithFade(SceneManager.GetActiveScene().name);
    }
    #endregion

    #region Pause System
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("Tecla P presionada");
            TogglePause();
        }
    }

    public void TogglePause()
    {
        Time.timeScale = Time.timeScale == 0 ? 1 : 0;
        if (pauseMenu != null)
        {
            pauseMenu.SetActive(!pauseMenu.activeSelf);
            Debug.Log($"Juego en pausa: {Time.timeScale == 0}");
        }
    }
    #endregion

    #region Ending Scene
    [Header("Ending Configuration")]
    [SerializeField] private float textScrollSpeed = 25f;
    [SerializeField] private Text endingText;
    [SerializeField] private Button returnButton;

    private void SetupEndingScene()
    {
        if (returnButton != null && endingText != null)
        {
            returnButton.onClick.RemoveAllListeners();
            returnButton.onClick.AddListener(ReturnToMenu);
            StartCoroutine(ScrollEndingText());
        }
        else
        {
            Debug.LogError("Faltan referencias de Ending en el Inspector");
        }
    }

    private IEnumerator ScrollEndingText()
    {
        RectTransform textTransform = endingText.GetComponent<RectTransform>();
        float startPosition = textTransform.anchoredPosition.y;

        while(textTransform.anchoredPosition.y < startPosition + 1000)
        {
            textTransform.anchoredPosition += Vector2.up * textScrollSpeed * Time.deltaTime;
            yield return null;
        }

        returnButton.gameObject.SetActive(true);
    }

    private void ReturnToMenu() => SceneManager.LoadScene(menuScene);
    #endregion

    #region Helper Methods
    private void FindFadeAnimator()
    {
        GameObject fadeObject = GameObject.FindGameObjectWithTag(fadeAnimatorTag);
        if (fadeObject != null)
        {
            fadeAnimator = fadeObject.GetComponent<Animator>();
            Debug.Log("Animator de fade encontrado");
        }
    }
    #endregion
}