using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParameterSoundsVolume : StorageParameter<float>
{
    private readonly string _keyForPlayerPrefs = "SoundsVolume";
    private float _soundsVolume = 0f;

    public ParameterSoundsVolume()
    {
        SetInitialValue();
    }

    public override void SetInitialValue()
    {
        if (PlayerPrefs.HasKey(_keyForPlayerPrefs))
        {
            _soundsVolume = PlayerPrefs.GetFloat(_keyForPlayerPrefs);
        }
        else
        {
            _soundsVolume = 0.5f;
        }
    }

    public override void SetNewValue(float newValue)
    {
        _soundsVolume = newValue;
        PlayerPrefs.SetFloat(_keyForPlayerPrefs, newValue);
    }

    public override float GetCurrentValue()
    {
        return _soundsVolume;
    }  
}
