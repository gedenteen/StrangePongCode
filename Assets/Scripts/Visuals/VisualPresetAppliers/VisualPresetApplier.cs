using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class VisualPresetApplier : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] protected ColorCategory _colorCategory;

    [Header("References")]
    [SerializeField] protected VisualPresets _visualPresets;

    protected void Awake()
    {
        Settings.EventChangeVisualPreset.AddListener(ChangeColor);
    }

    protected void Start()
    {        
        byte indexOfCurrentPreset = Settings.instance.GetCurrentVisualPresetIndex();
        ChangeColor(indexOfCurrentPreset);
    }

    protected void OnDestroy()
    {
        Settings.EventChangeVisualPreset.RemoveListener(ChangeColor);   
    }

    protected abstract void ChangeColor(byte indexOfPreset);
}
