using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VisualPresetApplierForImage : VisualPresetApplier
{
    [SerializeField] private Image _image;

    protected override void ChangeColor(byte indexOfPreset)
    {
        _image.color = _visualPresets.GetColor(indexOfPreset, _colorCategory);
    }
}