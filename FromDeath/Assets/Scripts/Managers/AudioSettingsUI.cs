using UnityEngine;
using UnityEngine.UI;

// Este script debe ir en el panel que contiene los sliders de opciones de audio.
public class AudioSettingsUI : MonoBehaviour
{
    [Header("UI Controls")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    private void Start()
    {
        StartCoroutine(InitializeAfterAudioManager());
    }

    private System.Collections.IEnumerator InitializeAfterAudioManager()
    {
        // Wait until AudioManager.Instance is not null
        while (AudioManager.Instance == null)
        {
            yield return null; // Wait for one frame
        }

        // Now AudioManager.Instance is ready, proceed with initialization
        // 1. Asignar los valores iniciales a los sliders al abrir el menú.
        // Obtenemos los valores guardados a través del AudioManager.
        masterSlider.value = AudioManager.Instance.GetMasterVolume();
        musicSlider.value = AudioManager.Instance.GetMusicVolume();
        sfxSlider.value = AudioManager.Instance.GetSFXVolume();

        // 2. Añadir "listeners" para que los sliders reaccionen a los cambios.
        // Cuando el valor del slider cambie, llamará al método correspondiente del AudioManager.
        masterSlider.onValueChanged.AddListener(AudioManager.Instance.SetMasterVolume);
        musicSlider.onValueChanged.AddListener(AudioManager.Instance.SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(AudioManager.Instance.SetSFXVolume);
    }

    private void OnDestroy()
    {
        // Es una buena práctica limpiar los listeners cuando el objeto se destruye.
        if (masterSlider != null) masterSlider.onValueChanged.RemoveAllListeners();
        if (musicSlider != null) musicSlider.onValueChanged.RemoveAllListeners();
        if (sfxSlider != null) sfxSlider.onValueChanged.RemoveAllListeners();
    }
}
