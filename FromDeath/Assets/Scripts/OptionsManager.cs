using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class OptionsManager : MonoBehaviour
{
    [Header("Referencias UI")]
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Dropdown graphicsQualityDropdown;
    [SerializeField] private Button backButton;

    //[Header("Configuración")]
    //[SerializeField] private string openingSceneName = "OpeningScene";

    private void Start()
    {
        // Configurar valores iniciales
        masterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1f);
        musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
        
        // Llenar dropdown de calidad gráfica
        graphicsQualityDropdown.ClearOptions();
        graphicsQualityDropdown.AddOptions(QualitySettings.names.ToList());
        graphicsQualityDropdown.value = QualitySettings.GetQualityLevel();

        // Asignar eventos
        masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
        musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        graphicsQualityDropdown.onValueChanged.AddListener(OnGraphicsQualityChanged);
        backButton.onClick.AddListener(OnBackButtonClicked);
    }

    private void OnMasterVolumeChanged(float value)
    {
        AudioListener.volume = value;
        PlayerPrefs.SetFloat("MasterVolume", value);
    }

    private void OnMusicVolumeChanged(float value)
    {
        // Implementa lógica para música específica
        PlayerPrefs.SetFloat("MusicVolume", value);
    }

    private void OnGraphicsQualityChanged(int index)
    {
        QualitySettings.SetQualityLevel(index);
    }

    public void OnBackButtonClicked()
    {
        optionsPanel.SetActive(false);
        // Si necesitas cargar otra escena:
        // SceneManager.LoadScene(openingSceneName);
    }
}
