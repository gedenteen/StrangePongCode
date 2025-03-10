using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "VisualPresets", menuName = "ScriptableObjects/Create VisualPresets asset")]
public class VisualPresets : ScriptableObject
{
    public List<VisualPreset> ListOfPresets;

    public Color GetColor(byte indexOfPreset, ColorCategory category)
    {
        if (indexOfPreset < 0 || indexOfPreset >= ListOfPresets.Count)
        {
            Debug.LogError($"VisualPresets: GetColor: invalid index = {indexOfPreset}");
            return Color.white;
        }

        switch (category)
        {
            case ColorCategory.PrimaryBright:
                return ListOfPresets[indexOfPreset].PrimaryBrightColor;
            case ColorCategory.PrimaryDark:
                return ListOfPresets[indexOfPreset].PrimaryDarkColor;
            case ColorCategory.SecondaryBright:
                return ListOfPresets[indexOfPreset].SecondaryBrightColor;
            case ColorCategory.SecondaryDark:
                return ListOfPresets[indexOfPreset].SecondaryDarkColor;
            default:
                Debug.LogError($"VisualPresets: GetColor: unexpected ColorCategory = {category}");
                return Color.white;
        }
    }
}

[Serializable]
public class VisualPreset
{
    public VisualPresetId Id;
    public LocalizedString LocalizedName;
    public Color PrimaryBrightColor = Color.white;
    public Color PrimaryDarkColor = Color.white;
    public Color SecondaryBrightColor = Color.white;
    public Color SecondaryDarkColor = Color.white;
}
