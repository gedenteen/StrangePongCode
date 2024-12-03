using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParameterResolutionIndex : StorageParameter<int>
{
    private readonly string _keyForPlayerPrefs = "ResolutionIndex";
    private int _currentResolutionIndex = 0;
    private RefreshRate _currentRefreshRate;
    private List<Resolution> _filteredResolutions;
    private List<string> _availableResolutions = new List<string>();

    public ParameterResolutionIndex()
    {
        GetScreenResolutions();
        SetInitialValue();
    }

    private void GetScreenResolutions()
    {
        float acceptableAspectRatio = 16f / 9f;
        float aspectRatioTolerance = 0.001f;

        // Get resolutuons and refresh rate (Hz)
        Resolution[] resolutions = Screen.resolutions;
        _currentRefreshRate = Screen.currentResolution.refreshRateRatio;
        _filteredResolutions = new List<Resolution>();

        // Cycle for filtering resolutions
        for (int i = 0; i < resolutions.Length; i++)
        {
            // //Debug.Log($"{resolutions[i].width}x{resolutions[i].height} {resolutions[i].refreshRateRatio.value} Hz");
            
            float aspectRatio = (float)resolutions[i].width / (float)resolutions[i].height;
            
            if (Mathf.Abs(aspectRatio - acceptableAspectRatio) > aspectRatioTolerance)
            {
                continue;
            }

            if (i + 1 < resolutions.Length)
            {
                if (resolutions[i].width != resolutions[i + 1].width || resolutions[i].height != resolutions[i + 1].height)
                {
                    _filteredResolutions.Add(resolutions[i]);
                }
            }
            else
            {
                _filteredResolutions.Add(resolutions[i]);
            }
        }

        // Cycle for add filtered resolutuions to dropdown
        for (int i = 0; i < _filteredResolutions.Count; i++)
        {
            int hz = Mathf.RoundToInt((float)_filteredResolutions[i].refreshRateRatio.value);
            string optionText = string.Concat(_filteredResolutions[i].width, "x", _filteredResolutions[i].height, " ", hz.ToString(), "Hz");
            _availableResolutions.Add(optionText);

            if (_filteredResolutions[i].width == Screen.currentResolution.width && _filteredResolutions[i].height == Screen.currentResolution.height)
            {
                _currentResolutionIndex = i;
            }
        }
    }

    public List<string> GetAvailableResolutions()
    {
        return _availableResolutions;
    }

    public Resolution GetResolutionByIndex(int index)
    {
        return _filteredResolutions[index];
    }

    public override void SetInitialValue()
    {
        if (PlayerPrefs.HasKey(_keyForPlayerPrefs))
        {
            _currentResolutionIndex = PlayerPrefs.GetInt(_keyForPlayerPrefs);
        }
        // Default value set in GetScreenResolutions()
    }

    public override void SetNewValue(int newValue)
    {
        _currentResolutionIndex = newValue;
        PlayerPrefs.SetInt(_keyForPlayerPrefs, newValue);
    }

    public override int GetCurrentValue()
    {
        return _currentResolutionIndex;
    }  
}
