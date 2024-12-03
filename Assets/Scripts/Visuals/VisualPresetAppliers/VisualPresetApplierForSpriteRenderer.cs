using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualPresetApplierForSpriteRenderer : VisualPresetApplier
{
    [SerializeField] private SpriteRenderer _targetRenderer;

    protected override void ChangeColor(byte indexOfPreset)
    {
        _targetRenderer.color = _visualPresets.GetColor(indexOfPreset, _colorCategory);
    }
}
