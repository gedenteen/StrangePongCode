using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParameterBloomIntensity : StorageParameter<float>
{
    private readonly string _keyForPlayerPrefs = "BloomIntensity";
    private float _bloomIntensity = 0f;

    public ParameterBloomIntensity()
    {
        SetInitialValue();
    }

    public override void SetInitialValue()
    {
        if (PlayerPrefs.HasKey(_keyForPlayerPrefs))
        {
            _bloomIntensity = PlayerPrefs.GetFloat(_keyForPlayerPrefs);
        }
        else
        {
            _bloomIntensity = 0f;
        }
    }

    public override void SetNewValue(float newValue)
    {
        _bloomIntensity = newValue;
        PlayerPrefs.SetFloat(_keyForPlayerPrefs, newValue);
    }

    public override float GetCurrentValue()
    {
        return _bloomIntensity;
    }  
}
