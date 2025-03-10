using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.Audio;
using Zenject;

public class Settings : MonoBehaviour
{
    [Header("Public fields")]
    public static Settings instance = null;
    public static UnityEvent<byte> EventChangeVisualPreset = new UnityEvent<byte>();

    [Header("References to assets")]
    [SerializeField] private PostProcessProfile postProcessProfile;
    [SerializeField] private AudioMixer _audioMixer;

    [Header("References to scene objects")]
    [SerializeField] private Slider sliderMusicVolume;
    [SerializeField] private TextMeshProUGUI textMusicVolume;
    [SerializeField] private Slider sliderSoundsVolume;
    [SerializeField] private TextMeshProUGUI textSoundsVolume;
    [SerializeField] private Slider sliderBloom;
    [SerializeField] private TextMeshProUGUI textBloom;
    [SerializeField] private Slider sliderChromaticAberration;
    [SerializeField] private TextMeshProUGUI textChromaticAberration;
    [SerializeField] private Slider _sliderMaxFps;
    [SerializeField] private TextMeshProUGUI _textMaxFps;
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private Toggle toggleFullScreen;
    [SerializeField] private TMP_Dropdown _dropdownForVisualPresets;
    
    [Inject] private VisualPresets _visualPresets;

    // Private fields
    private bool isInitialized = false;
    private Bloom bloom;
    private ChromaticAberration chromaticAberration;
    private float _maxValueOfBloom = 30f;
    private float _maxValueOfChromaticAberraton = 1f;

    // Parameters
    private ParameterMusicVolume _parameterMusicVolume;
    private ParameterSoundsVolume _parameterSoundsVolume;
    private ParameterBloomIntensity _parameterBloomIntensity;
    private ParameterChromaticAberrationIntensity _parameterChromaticAberrationIntensity;
    private ParameterResolutionIndex _parameterResolutionIndex;
    private ParameterFullScreen _parameterFullScreen;
    private ParameterVisualPresetIndex _parameterVisualPresetIndex;
    private ParameterMaxFpsIndex _parameterMaxFpsIndex;

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

        // Create parameters with initialization
        _parameterMusicVolume = new ParameterMusicVolume();
        _parameterSoundsVolume = new ParameterSoundsVolume();
        _parameterBloomIntensity = new ParameterBloomIntensity();
        _parameterChromaticAberrationIntensity = new ParameterChromaticAberrationIntensity();
        _parameterResolutionIndex = new ParameterResolutionIndex();
        _parameterFullScreen = new ParameterFullScreen();
        _parameterVisualPresetIndex = new ParameterVisualPresetIndex();
        _parameterMaxFpsIndex = new ParameterMaxFpsIndex();

        // Get Bloom component
        postProcessProfile.TryGetSettings<Bloom>(out bloom);
        if (bloom != null)
        {
            bloom.intensity.value = 0;
        }
        else
        {
            Debug.LogError("Settings: Awake: can't get component Bloom");
        }

        // Get ChromaticAberration component
        postProcessProfile.TryGetSettings<ChromaticAberration>(out chromaticAberration);
        if (chromaticAberration != null)
        {
            chromaticAberration.intensity.value = 0;
        }
        else
        {
            Debug.LogError("Settings: Awake: can't get component ChromaticAberration");
        }        

        FillResolutionsDropdown();
    }

    private void Start()
    {
        // Set values (changes audio mixer must be set on Start)
        ChangeVolumeMusic(_parameterMusicVolume.GetCurrentValue());
        sliderMusicVolume.value = _parameterMusicVolume.GetCurrentValue();
        ChangeVolumeSounds(_parameterSoundsVolume.GetCurrentValue());
        sliderSoundsVolume.value = _parameterSoundsVolume.GetCurrentValue();
        ChangeBloomValue(_parameterBloomIntensity.GetCurrentValue());
        sliderBloom.value = _parameterBloomIntensity.GetCurrentValue();
        ChangeChromaticAberrationIntensity(_parameterChromaticAberrationIntensity.GetCurrentValue());
        sliderChromaticAberration.value = _parameterChromaticAberrationIntensity.GetCurrentValue();
        SetMaxFpsIndex(_parameterMaxFpsIndex.GetCurrentValue());
        _sliderMaxFps.value = _parameterMaxFpsIndex.GetCurrentValue();
        SetResolution(_parameterResolutionIndex.GetCurrentValue());
        SetFullScreen(_parameterFullScreen.GetCurrentValue());

        UpdateUi();

        isInitialized = true;
    }

    #region Set methods

    public void ChangeVolumeMusic(float volume)
    {
        //Debug.Log($"Settings: ChangeVolumeMusic: volume={volume}");
        _audioMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);
        textMusicVolume.text = $"{Mathf.RoundToInt(volume * 100)}%";

        if (isInitialized)
        {
            _parameterMusicVolume.SetNewValue(volume);
        }
    }

    public void ChangeVolumeSounds(float volume)
    {
        //Debug.Log($"Settings: ChangeVolumeSounds: volume={volume}");
        _audioMixer.SetFloat("SoundsVolume", Mathf.Log10(volume) * 20);
        textSoundsVolume.text = $"{Mathf.RoundToInt(volume * 100)}%";

        if (isInitialized)
        {
            _parameterSoundsVolume.SetNewValue(volume);
        }
    }

    public void ChangeBloomValue(float value)
    {
        bloom.intensity.value = value;
        textBloom.text = $"{Mathf.RoundToInt((value / _maxValueOfBloom) * 100)}%";

        if (isInitialized)
        {
            _parameterBloomIntensity.SetNewValue(value);
        }
    }

    public void ChangeChromaticAberrationIntensity(float value)
    {
        chromaticAberration.intensity.value = value;
        textChromaticAberration.text = $"{Mathf.RoundToInt((value / _maxValueOfChromaticAberraton) * 100)}%";

        if (isInitialized)
        {
            _parameterChromaticAberrationIntensity.SetNewValue(value);
        }
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution res = _parameterResolutionIndex.GetResolutionByIndex(resolutionIndex);
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);

        if (isInitialized)
        {
            _parameterResolutionIndex.SetNewValue(resolutionIndex);
        }
    }

    public void SetFullScreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;

        if (isInitialized)
        {
            _parameterFullScreen.SetNewValue(isFullScreen);
        }
    }

    public void SetVisualPreset(int index)
    {
        Debug.Log($"Settings: SetVisualPreset: index={index}");
        EventChangeVisualPreset?.Invoke((byte)index);

        if (isInitialized)
        {
            _parameterVisualPresetIndex.SetNewValue((byte)index);
        }
    }

    public void SetMaxFpsIndex(float index)
    {
        //Debug.Log($"Settings: SetMaxFpsIndex: index={index}");

        QualitySettings.vSyncCount = 0; // VSync off

        if (index == _sliderMaxFps.maxValue)
        {
            Application.targetFrameRate = -1;
            _textMaxFps.text = "No limit";
        }
        else
        {
            int targetFps = ((int)index + 1) * 30;
            Application.targetFrameRate = targetFps;
            _textMaxFps.text = targetFps.ToString();
        }

        if (isInitialized)
        {
            _parameterMaxFpsIndex.SetNewValue((byte)index);
        }
    }

    public void SetChromaticAverrationValue(float value)
    {
        value = Mathf.Clamp01(value);
        value *= _maxValueOfChromaticAberraton;
        sliderChromaticAberration.value = value;
    }

    public void SetBloomValue(float value)
    {
        value = Mathf.Clamp01(value);
        value *= _maxValueOfBloom;
        sliderBloom.value = value;
    }

    #endregion

    #region Get methods 

    public byte GetCurrentVisualPresetIndex()
    {
        return _parameterVisualPresetIndex.GetCurrentValue();
    }

    #endregion

    #region Methods for updating UI

    private void FillResolutionsDropdown()
    {
        resolutionDropdown.ClearOptions();
        resolutionDropdown.AddOptions(_parameterResolutionIndex.GetAvailableResolutions());
    }

    public void UpdateUi()
    {
        toggleFullScreen.isOn = _parameterFullScreen.GetCurrentValue();
        resolutionDropdown.value = _parameterResolutionIndex.GetCurrentValue();
        resolutionDropdown.RefreshShownValue();
        _dropdownForVisualPresets.value = _parameterVisualPresetIndex.GetCurrentValue();
        _dropdownForVisualPresets.RefreshShownValue();
    }

    #endregion
}
