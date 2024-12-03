using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParameterVisualPresetIndex : StorageParameter<byte>
{
    private readonly string _keyForPlayerPrefs = "VisualPresetIndex";
    private byte _currentVisualPresetIndex = 0;

    public ParameterVisualPresetIndex()
    {
        SetInitialValue();
    }

    public override void SetInitialValue()
    {
        if (PlayerPrefs.HasKey(_keyForPlayerPrefs))
        {
            _currentVisualPresetIndex = (byte)PlayerPrefs.GetInt(_keyForPlayerPrefs);
        }
        else
        {
            _currentVisualPresetIndex = 0;
        }
    }

    public override void SetNewValue(byte newValue)
    {
        _currentVisualPresetIndex = newValue;
        PlayerPrefs.SetInt(_keyForPlayerPrefs, newValue);
    }

    public override byte GetCurrentValue()
    {
        return _currentVisualPresetIndex;
    }  
}
