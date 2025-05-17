using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class IntroManager : MonoBehaviour
{
    [Header("Configuración Básica")]
    [SerializeField] private float textDisplayTime = 5f;
    [SerializeField] private float buttonsDelay = 60f; // 1 minuto
    [SerializeField] private string nextSceneName = "Level1";

    [Header("Referencias UI")]
    [SerializeField] private GameObject[] introPanels;
    [SerializeField] private Button skipButton;
    [SerializeField] private Button playButton; // Nuevo botón
    [SerializeField] private Button optionsButton; // Nuevo botón
    [SerializeField] private GameObject optionsPanel; // Panel de opciones

    private int currentPanel = 0;
    private bool buttonsActive = false;

    void Start()
    {
        // Configuración inicial
        playButton.gameObject.SetActive(false);
        optionsButton.gameObject.SetActive(false);
        optionsPanel.SetActive(false);

        // Listeners de botones
        skipButton.onClick.AddListener(SkipIntro);
        playButton.onClick.AddListener(LoadGame);
        optionsButton.onClick.AddListener(ShowOptions);

        // Lógica existente
        InvokeRepeating(nameof(NextPanel), textDisplayTime, textDisplayTime);
        
        // Nuevo: Mostrar botones tras 1 minuto
        Invoke(nameof(ShowDelayedButtons), buttonsDelay);
    }

    void NextPanel()
    {
        if(currentPanel >= introPanels.Length - 1)
        {
            LoadGame();
            return;
        }

        introPanels[currentPanel].SetActive(false);
        currentPanel++;
        introPanels[currentPanel].SetActive(true);
    }

    void ShowDelayedButtons()
    {
        if(!buttonsActive)
        {
            playButton.gameObject.SetActive(true);
            optionsButton.gameObject.SetActive(true);
            buttonsActive = true;
        }
    }

    void ShowOptions()
    {
        optionsPanel.SetActive(true);
        // Lógica adicional de opciones (volumen, gráficos, etc)
    }

    void LoadGame() => SceneManager.LoadScene(nextSceneName);

    void SkipIntro() => LoadGame();
}
