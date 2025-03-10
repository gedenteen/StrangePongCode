using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParametersKeyBindings
{
    private KeyCode _defaultKeyForLeftPlayerUp = KeyCode.W;
    private KeyCode _defaultKeyForLeftPlayerDown = KeyCode.S;
    private KeyCode _defaultKeyForRightPlayerUp = KeyCode.UpArrow;
    private KeyCode _defaultKeyForRightPlayerDown = KeyCode.DownArrow;

    private readonly string _keyForPlayerPrefs_KeyForLeftPlayerUp = "KeyForLeftPlayerUp";
    private readonly string _keyForPlayerPrefs_KeyForLeftPlayerDown = "KeyForLeftPlayerDown";
    private readonly string _keyForPlayerPrefs_KeyForRightPlayerUp = "KeyForRightPlayerUp";
    private readonly string _keyForPlayerPrefs_KeyForRightPlayerDown = "KeyForRightPlayerDown";

    public KeyCode KeyForLeftPlayerUp { get; private set; }
    public KeyCode KeyForLeftPlayerDown { get; private set; }
    public KeyCode KeyForRightPlayerUp { get; private set; }
    public KeyCode KeyForRightPlayerDown { get; private set; }
    
    public ParametersKeyBindings()
    {
        SetInitialValues();
    }

    public void SetInitialValues()
    {
        KeyForLeftPlayerUp = GetInitialValue(_defaultKeyForLeftPlayerUp, 
                                             _keyForPlayerPrefs_KeyForLeftPlayerUp);

        KeyForLeftPlayerDown = GetInitialValue(_defaultKeyForLeftPlayerDown, 
                                               _keyForPlayerPrefs_KeyForLeftPlayerDown);

        KeyForRightPlayerUp = GetInitialValue(_defaultKeyForRightPlayerUp, 
                                              _keyForPlayerPrefs_KeyForRightPlayerUp);

        KeyForRightPlayerDown = GetInitialValue(_defaultKeyForRightPlayerDown, 
                                                _keyForPlayerPrefs_KeyForRightPlayerDown);
    }

    private KeyCode GetInitialValue(KeyCode defaultKeyCode, string plyerPrefsKey)
    {
        if (PlayerPrefs.HasKey(plyerPrefsKey))
        {
            return (KeyCode)PlayerPrefs.GetInt(plyerPrefsKey);
        }
        else
        {
            return defaultKeyCode;
        }
    }

    public void SetNewKeyForLeftPlayerUp(KeyCode newKeyCode)
    {
        KeyForLeftPlayerUp = newKeyCode;
        PlayerPrefs.SetInt(_keyForPlayerPrefs_KeyForLeftPlayerUp, (int)newKeyCode);
    }

    public void SetNewKeyForLeftPlayerDown(KeyCode newKeyCode)
    {
        KeyForLeftPlayerDown = newKeyCode;
        PlayerPrefs.SetInt(_keyForPlayerPrefs_KeyForLeftPlayerDown, (int)newKeyCode);
    }

    public void SetNewKeyForRightPlayerUp(KeyCode newKeyCode)
    {
        KeyForRightPlayerUp = newKeyCode;
        PlayerPrefs.SetInt(_keyForPlayerPrefs_KeyForRightPlayerUp, (int)newKeyCode);
    }

    public void SetNewKeyForRightPlayerDown(KeyCode newKeyCode)
    {
        KeyForRightPlayerDown = newKeyCode;
        PlayerPrefs.SetInt(_keyForPlayerPrefs_KeyForRightPlayerDown, (int)newKeyCode);
    }
}
