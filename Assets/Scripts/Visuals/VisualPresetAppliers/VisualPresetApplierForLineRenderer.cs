using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualPresetApplierForLineRenderer : VisualPresetApplier
{
    [SerializeField] private LineRenderer _lineRenderer;

    protected override void ChangeColor(byte indexOfPreset)
    {
        Color color = _visualPresets.GetColor(indexOfPreset, _colorCategory);
        _lineRenderer.startColor = color;
        _lineRenderer.endColor = color;
    }
}
