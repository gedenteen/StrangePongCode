using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParameterFullScreen : StorageParameter<bool>
{
    private readonly string _keyForPlayerPrefs = "IsFullScreen";
    private bool _isFullScreen = true;

    public ParameterFullScreen()
    {
        SetInitialValue();
    }

    public override void SetInitialValue()
    {
        if (PlayerPrefs.HasKey(_keyForPlayerPrefs))
        {
            _isFullScreen = PlayerPrefs.GetInt(_keyForPlayerPrefs) == 1;
        }
        else
        {
            _isFullScreen = true;
        }
    }

    public override void SetNewValue(bool newValue)
    {
        _isFullScreen = newValue;
        PlayerPrefs.SetInt(_keyForPlayerPrefs, newValue ? 1 : 0);
    }

    public override bool GetCurrentValue()
    {
        return _isFullScreen;
    }  
}
