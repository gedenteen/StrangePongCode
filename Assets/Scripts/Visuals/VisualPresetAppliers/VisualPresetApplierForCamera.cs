using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualPresetApplierForCamera : VisualPresetApplier
{
    [SerializeField] private Camera _camera;

    protected override void ChangeColor(byte indexOfPreset)
    {
        _camera.backgroundColor = _visualPresets.GetColor(indexOfPreset, _colorCategory);
    }
}
