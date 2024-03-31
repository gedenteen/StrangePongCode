using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;
using TMPro;

public class Settings : MonoBehaviour
{
    [Header("Public fields")]
    public static Settings instance = null;

    [Header("References to assets")]
    [SerializeField] private PostProcessProfile postProcessProfile;

    [Header("References to scene objects")]
    [SerializeField] private Slider sliderMusicVolume;
    [SerializeField] private TextMeshProUGUI textMusicVolume;
    [SerializeField] private Slider sliderSoundsVolume;
    [SerializeField] private TextMeshProUGUI textSoundsVolume;
    [SerializeField] private Slider sliderBloom;
    [SerializeField] private TextMeshProUGUI textBloom;

    // Private fields
    private bool isInitialized = false;
    private Bloom bloom;

    // Strings for PlayerPrefs
    private readonly string pp_musicVolume = "MusicVolume";
    private readonly string pp_soundVolume = "SoundVolume";
    private readonly string pp_bloomValue = "BloomValue";

    private void Awake()
    {
        if (instance)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        // Get Bloom component
        postProcessProfile.TryGetSettings<Bloom>(out bloom);
        if (bloom != null)
        {
            bloom.intensity.value = 0;
        }
        else
        {
            Debug.LogError("Settings: NewLevelIsLoaded: can't get component Bloom");
        }

        // Default values of settings
        float musicVolume = 1f;
        float soundVolume = 1f;
        float bloomValue = 1.5f; //5%

        // Get player values, if he change them
        if (PlayerPrefs.HasKey(pp_musicVolume))
        {
            musicVolume = PlayerPrefs.GetFloat(pp_musicVolume);
        }
        if (PlayerPrefs.HasKey(pp_soundVolume))
        {
            soundVolume = PlayerPrefs.GetFloat(pp_soundVolume);
        }
        if (PlayerPrefs.HasKey(pp_bloomValue))
        {
            bloomValue = PlayerPrefs.GetFloat(pp_bloomValue);
        }

        // Set values
        ChangeVolumeMusic(musicVolume);
        sliderMusicVolume.value = musicVolume;
        ChangeVolumeSounds(soundVolume);
        sliderSoundsVolume.value = soundVolume;
        ChangeBloomValue(bloomValue);
        sliderBloom.value = bloomValue;

        isInitialized = true;
    }

    public void ChangeVolumeMusic(float value)
    {
        AudioManager.instance.ChangeVolumeMusic(value);
        textMusicVolume.text = $"{Mathf.RoundToInt(value * 100)}%";

        if (isInitialized)
        {
            PlayerPrefs.SetFloat(pp_musicVolume, value);
        }
    }
    public void ChangeVolumeSounds(float value)
    {
        AudioManager.instance.ChangeVolumeSounds(value);
        textSoundsVolume.text = $"{Mathf.RoundToInt(value * 100)}%";

        if (isInitialized)
        {
            PlayerPrefs.SetFloat(pp_soundVolume, value);
        }
    }
    public void ChangeBloomValue(float value)
    {
        bloom.intensity.value = value;
        textBloom.text = $"{Mathf.RoundToInt((value / 40) * 100)}%";

        if (isInitialized)
        {
            PlayerPrefs.SetFloat(pp_bloomValue, value);
        }
    }
}
