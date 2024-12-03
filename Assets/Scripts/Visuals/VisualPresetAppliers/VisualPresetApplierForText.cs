using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class VisualPresetApplierForText : VisualPresetApplier
{
    [SerializeField] private TextMeshProUGUI _textMesh;

    protected override void ChangeColor(byte indexOfPreset)
    {
        _textMesh.color = _visualPresets.GetColor(indexOfPreset, _colorCategory);
    }
}