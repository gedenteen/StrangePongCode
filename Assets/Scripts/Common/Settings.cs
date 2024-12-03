using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.Events;

public class Settings : MonoBehaviour
{
    [Header("Public fields")]
    public static Settings instance = null;
    public static UnityEvent<byte> EventChangeVisualPreset = new UnityEvent<byte>();

    [Header("References to assets")]
    [SerializeField] private PostProcessProfile postProcessProfile;
    [SerializeField] private VisualPresets _visualPresets;

    [Header("References to scene objects")]
    [SerializeField] private Slider sliderMusicVolume;
    [SerializeField] private TextMeshProUGUI textMusicVolume;
    [SerializeField] private Slider sliderSoundsVolume;
    [SerializeField] private TextMeshProUGUI textSoundsVolume;
    [SerializeField] private Slider sliderBloom;
    [SerializeField] private TextMeshProUGUI textBloom;
    [SerializeField] private Slider sliderChromaticAberration;
    [SerializeField] private TextMeshProUGUI textChromaticAberration;
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private Toggle toggleFullScreen;
    [SerializeField] private TMP_Dropdown _dropdownForVisualPresets;

    // Private fields
    private bool isInitialized = false;
    private Bloom bloom;
    private ChromaticAberration chromaticAberration;

    // Parameters
    private ParameterMusicVolume _parameterMusicVolume;
    private ParameterSoundsVolume _parameterSoundsVolume;
    private ParameterBloomIntensity _parameterBloomIntensity;
    private ParameterChromaticAberrationIntensity _parameterChromaticAberrationIntensity;
    private ParameterResolutionIndex _parameterResolutionIndex;
    private ParameterFullScreen _parameterFullScreen;
    private ParameterVisualPresetIndex _parameterVisualPresetIndex;

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
        FillDropdownForVisualPresets();

        // Set values
        ChangeVolumeMusic(_parameterMusicVolume.GetCurrentValue());
        sliderMusicVolume.value = _parameterMusicVolume.GetCurrentValue();
        ChangeVolumeSounds(_parameterSoundsVolume.GetCurrentValue());
        sliderSoundsVolume.value = _parameterSoundsVolume.GetCurrentValue();
        ChangeBloomValue(_parameterBloomIntensity.GetCurrentValue());
        sliderBloom.value = _parameterBloomIntensity.GetCurrentValue();
        ChangeChromaticAberrationIntensity(_parameterChromaticAberrationIntensity.GetCurrentValue());
        sliderChromaticAberration.value = _parameterChromaticAberrationIntensity.GetCurrentValue();
        SetResolution(_parameterResolutionIndex.GetCurrentValue());
        SetFullScreen(_parameterFullScreen.GetCurrentValue());

        UpdateUi();

        isInitialized = true;
    }

    #region Set methods

    public void ChangeVolumeMusic(float value)
    {
        AudioManager.instance.ChangeVolumeMusic(value);
        textMusicVolume.text = $"{Mathf.RoundToInt(value * 100)}%";

        if (isInitialized)
        {
            _parameterMusicVolume.SetNewValue(value);
        }
    }

    public void ChangeVolumeSounds(float value)
    {
        AudioManager.instance.ChangeVolumeSounds(value);
        textSoundsVolume.text = $"{Mathf.RoundToInt(value * 100)}%";

        if (isInitialized)
        {
            _parameterSoundsVolume.SetNewValue(value);
        }
    }

    public void ChangeBloomValue(float value)
    {
        bloom.intensity.value = value;
        textBloom.text = $"{Mathf.RoundToInt((value / 40) * 100)}%";

        if (isInitialized)
        {
            _parameterBloomIntensity.SetNewValue(value);
        }
    }

    public void ChangeChromaticAberrationIntensity(float value)
    {
        chromaticAberration.intensity.value = value;
        textChromaticAberration.text = $"{Mathf.RoundToInt((value / 40) * 100)}%";

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

    private void FillDropdownForVisualPresets()
    {
        _dropdownForVisualPresets.ClearOptions();

        var options = new List<string>();

        foreach (var preset in _visualPresets.ListOfPresets)
        {
            options.Add(preset.Name);
        }

        _dropdownForVisualPresets.AddOptions(options);
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
