using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    #region Singleton Pattern
    public static AudioManager Instance { get; private set; }
    #endregion

    #region Audio Mixer Configuration
    [Header("Mixer Configuration")]
    [SerializeField] private AudioMixer masterMixer;
    [SerializeField] private AudioMixerGroup musicGroup;
    [SerializeField] private AudioMixerGroup sfxGroup;
    #endregion

    #region Volume Parameters
    private const string MASTER_VOL = "MasterVol";
    private const string MUSIC_VOL = "MusicVol";
    private const string SFX_VOL = "SFXVol";
    private const float MIN_VOL = -80f;
    private const float MAX_VOL = 0f;
    #endregion

    #region UI References
    [Header("UI Controls")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    #endregion

    #region Initialization
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAudioSettings();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeAudioSettings()
    {
        ConfigureMixerOutput();
        LoadVolumeSettings();
        SetupSliderCallbacks();
    }
    #endregion

    #region Mixer Setup
    private void ConfigureMixerOutput()
    {
        // Garantiza salida correcta en todas las plataformas
        masterMixer.SetFloat(MASTER_VOL, 0f);
        masterMixer.SetFloat(MUSIC_VOL, 0f);
        masterMixer.SetFloat(SFX_VOL, 0f);
    }
    #endregion

    #region Volume Control Logic
    public void SetMasterVolume(float volume)
    {
        SetMixerVolume(MASTER_VOL, volume);
        PlayerPrefs.SetFloat(MASTER_VOL, volume);
    }

    public void SetMusicVolume(float volume)
    {
        SetMixerVolume(MUSIC_VOL, volume);
        PlayerPrefs.SetFloat(MUSIC_VOL, volume);
    }

    public void SetSFXVolume(float volume)
    {
        SetMixerVolume(SFX_VOL, volume);
        PlayerPrefs.SetFloat(SFX_VOL, volume);
    }

    private void SetMixerVolume(string parameter, float value)
    {
        float dB = value > 0.01f ? 
            Mathf.Log10(value) * 20 : 
            MIN_VOL;

        masterMixer.SetFloat(parameter, Mathf.Clamp(dB, MIN_VOL, MAX_VOL));
    }
    #endregion

    #region Save/Load System
    private void LoadVolumeSettings()
    {
        masterSlider.value = PlayerPrefs.GetFloat(MASTER_VOL, 1f);
        musicSlider.value = PlayerPrefs.GetFloat(MUSIC_VOL, 1f);
        sfxSlider.value = PlayerPrefs.GetFloat(SFX_VOL, 1f);
    }

    public void ResetToDefaults()
    {
        masterSlider.value = 1f;
        musicSlider.value = 1f;
        sfxSlider.value = 1f;
        PlayerPrefs.DeleteKey(MASTER_VOL);
        PlayerPrefs.DeleteKey(MUSIC_VOL);
        PlayerPrefs.DeleteKey(SFX_VOL);
    }
    #endregion

    #region UI Integration
    private void SetupSliderCallbacks()
    {
        masterSlider.onValueChanged.AddListener(SetMasterVolume);
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }
    #endregion

    #region Debug Utilities
    public void PrintCurrentMixerState()
    {
        Debug.Log($"Master: {masterSlider.value} | Music: {musicSlider.value} | SFX: {sfxSlider.value}");
    }
    #endregion
}
