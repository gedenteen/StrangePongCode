using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParameterMaxFpsIndex : StorageParameter<byte>
{
    private readonly string _keyForPlayerPrefs = "MaxFpsIndex";
    private byte _currentMaxFpsIndex = 1;

    public ParameterMaxFpsIndex()
    {
        SetInitialValue();
    }

    public override void SetInitialValue()
    {
        if (PlayerPrefs.HasKey(_keyForPlayerPrefs))
        {
            _currentMaxFpsIndex = (byte)PlayerPrefs.GetInt(_keyForPlayerPrefs);
        }
        else
        {
            _currentMaxFpsIndex = 1;
        }
    }

    public override void SetNewValue(byte newValue)
    {
        _currentMaxFpsIndex = newValue;
        PlayerPrefs.SetInt(_keyForPlayerPrefs, newValue);
    }

    public override byte GetCurrentValue()
    {
        return _currentMaxFpsIndex;
    }  
}
