using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

[System.Serializable]
public class SceneMusic
{
    public string sceneName;
    public AudioClip musicClip;
}

public class AudioManager : MonoBehaviour
{
    private static AudioManager _instance;

    // CORRECCIÓN: Lógica completa del singleton de carga automática.
    public static AudioManager Instance
    {
        get
        {
            if (_instance == null)
            {
                // Try to find an existing instance in the scene
                _instance = FindFirstObjectByType<AudioManager>();

                // If no instance exists, try to load from Resources and instantiate
                if (_instance == null)
                {
                    GameObject managerPrefab = Resources.Load<GameObject>("AudioManager");
                    if (managerPrefab != null)
                    {
                        GameObject managerObject = Instantiate(managerPrefab);
                        _instance = managerObject.GetComponent<AudioManager>();
                        _instance.name = "AudioManager (Auto-Instantiated)";
                        Debug.Log("AudioManager: Instantiated from prefab.");
                    }
                    else
                    {
                        Debug.LogError("FATAL ERROR: El Prefab 'AudioManager' no se encontró en la carpeta 'Resources'. No se pudo crear AudioManager.");
                    }
                }
                else
                {
                    Debug.Log("AudioManager: Found existing instance in scene.");
                }
            }

            // Final check before returning
            if (_instance == null)
            {
                Debug.LogError("FATAL ERROR: AudioManager.Instance es nulo después de intentar encontrarlo o crearlo.");
            }
            return _instance;
        }
    }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer masterMixer;

    [Header("Music Playlist")]
    [SerializeField] private SceneMusic[] sceneMusicPlaylist;

    private const string MASTER_VOL_KEY = "MasterVolume";
    private const string MUSIC_VOL_KEY = "MusicVolume";
    private const string SFX_VOL_KEY = "SFXVolume";
    private const float MIN_DB = -80f;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAudio();
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayMusicForScene(scene.name);
    }

    private void PlayMusicForScene(string sceneName)
    {
        AudioClip clipToPlay = null;
        foreach (var sceneMusic in sceneMusicPlaylist)
        {
            if (sceneMusic.sceneName == sceneName)
            {
                clipToPlay = sceneMusic.musicClip;
                break;
            }
        }

        if (clipToPlay != null && musicSource.clip != clipToPlay)
        {
            musicSource.clip = clipToPlay;
            musicSource.Play();
        }
        else if (clipToPlay == null)
        {
            musicSource.Stop();
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    private void InitializeAudio()
    {
        SetMasterVolume(PlayerPrefs.GetFloat(MASTER_VOL_KEY, 1f));
        SetMusicVolume(PlayerPrefs.GetFloat(MUSIC_VOL_KEY, 1f));
        SetSFXVolume(PlayerPrefs.GetFloat(SFX_VOL_KEY, 1f));
    }

    public void SetMasterVolume(float volume)
    {
        SetMixerVolume(MASTER_VOL_KEY, volume);
        PlayerPrefs.SetFloat(MASTER_VOL_KEY, volume);
    }

    public void SetMusicVolume(float volume)
    {
        SetMixerVolume(MUSIC_VOL_KEY, volume);
        PlayerPrefs.SetFloat(MUSIC_VOL_KEY, volume);
    }

    public void SetSFXVolume(float volume)
    {
        SetMixerVolume(SFX_VOL_KEY, volume);
        PlayerPrefs.SetFloat(SFX_VOL_KEY, volume);
    }

    public float GetMasterVolume() => PlayerPrefs.GetFloat(MASTER_VOL_KEY, 1f);
    public float GetMusicVolume() => PlayerPrefs.GetFloat(MUSIC_VOL_KEY, 1f);
    public float GetSFXVolume() => PlayerPrefs.GetFloat(SFX_VOL_KEY, 1f);

    private void SetMixerVolume(string parameter, float linearVolume)
    {
        if (masterMixer == null) return;
        float dB = linearVolume > 0.001f ? Mathf.Log10(linearVolume) * 20 : MIN_DB;
        masterMixer.SetFloat(parameter, dB);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}