using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParameterMusicVolume : StorageParameter<float>
{
    private readonly string _keyForPlayerPrefs = "MusicVolume";
    private float _musicVolume = 0f;

    public ParameterMusicVolume()
    {
        SetInitialValue();
    }

    public override void SetInitialValue()
    {
        if (PlayerPrefs.HasKey(_keyForPlayerPrefs))
        {
            _musicVolume = PlayerPrefs.GetFloat(_keyForPlayerPrefs);
        }
        else
        {
            _musicVolume = 0.5f;
        }
    }

    public override void SetNewValue(float newValue)
    {
        _musicVolume = newValue;
        PlayerPrefs.SetFloat(_keyForPlayerPrefs, newValue);
    }

    public override float GetCurrentValue()
    {
        return _musicVolume;
    }  
}
