using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParameterLastCompletedLevel : StorageParameter<int>
{
    private readonly string _keyForPlayerPrefs = "LastCompletedLevel";
    private int _lastCompletedLevel = -1;

    public ParameterLastCompletedLevel()
    {
        SetInitialValue();
    }

    public override void SetInitialValue()
    {
        if (PlayerPrefs.HasKey(_keyForPlayerPrefs))
        {
            _lastCompletedLevel = PlayerPrefs.GetInt(_keyForPlayerPrefs);
        }
        else
        {
            _lastCompletedLevel = -1;
        }
    }

    public override void SetNewValue(int newValue)
    {
        _lastCompletedLevel = newValue;
        PlayerPrefs.SetInt(_keyForPlayerPrefs, newValue);
    }

    public override int GetCurrentValue()
    {
        return _lastCompletedLevel;
    }  
}
