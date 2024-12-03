using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParameterChromaticAberrationIntensity : StorageParameter<float>
{
    private readonly string _keyForPlayerPrefs = "ChromaticAberrationIntensity";
    private float _chromaticAberrationIntensity = 0f;

    public ParameterChromaticAberrationIntensity()
    {
        SetInitialValue();
    }

    public override void SetInitialValue()
    {
        if (PlayerPrefs.HasKey(_keyForPlayerPrefs))
        {
            _chromaticAberrationIntensity = PlayerPrefs.GetFloat(_keyForPlayerPrefs);
        }
        else
        {
            _chromaticAberrationIntensity = 0f;
        }
    }

    public override void SetNewValue(float newValue)
    {
        _chromaticAberrationIntensity = newValue;
        PlayerPrefs.SetFloat(_keyForPlayerPrefs, newValue);
    }

    public override float GetCurrentValue()
    {
        return _chromaticAberrationIntensity;
    }  
}
