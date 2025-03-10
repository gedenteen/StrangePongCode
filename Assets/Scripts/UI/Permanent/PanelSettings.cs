using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using Zenject;

public class PanelSettings : MonoBehaviour
{
    public static UnityEvent<bool> eventActivate = new UnityEvent<bool>();

    [Header("References to my objects")]
    [SerializeField] private GameObject myPanel;
    [SerializeField] private GameObject _cellChromaticAberration;
    [SerializeField] private GameObject _cellBloom;
    [SerializeField] private GameObject _cellVisualPresets;
    [SerializeField] private TMP_Dropdown _dropdownForVisualPresets;

    [Inject] private VisualPresets _visualPresets;

    private void Awake()
    {
        EventsManager.levelLoaded.AddListener((int value) => myPanel.gameObject.SetActive(false));
        eventActivate.AddListener(Activate);
    }

    public void Activate(bool value)
    {
        Debug.Log($"PanelSettings: Activate: value={value}");
        myPanel.gameObject.SetActive(value);
        
        if (value)
        {
            HideAndShowCells();
            FillDropdownForVisualPresets();
        }
    }

    private void HideAndShowCells()
    {
        if (!RewardsController.Instance.HasReward(RewardId.UnlockPostEffectChromaticAberration))
        {
            _cellChromaticAberration.gameObject.SetActive(false);
        }
        else
        {
            _cellChromaticAberration.gameObject.SetActive(true);
        }

        if (!RewardsController.Instance.HasReward(RewardId.UnlockPostEffectBloom))
        {
            _cellBloom.gameObject.SetActive(false);
        }
        else
        {
            _cellBloom.gameObject.SetActive(true);
        }
    }

    private void FillDropdownForVisualPresets()
    {
        _dropdownForVisualPresets.ClearOptions();

        var options = new List<string>();

        foreach (var preset in _visualPresets.ListOfPresets)
        {
            if (preset.Id == VisualPresetId.DarkPurple && 
                !RewardsController.Instance.HasReward(RewardId.UnlockVisualPresetDarkPurple))
            {
                continue;
            }
            else if (preset.Id == VisualPresetId.DarkYellow && 
                !RewardsController.Instance.HasReward(RewardId.UnlockVisualPresetDarkYellow))
            {
                continue;
            }
            
            options.Add(preset.LocalizedName.GetLocalizedString());
        }

        _dropdownForVisualPresets.AddOptions(options);

        if (options.Count <= 1)
        {
            _cellVisualPresets.SetActive(false);
        }
        else
        {
            _cellVisualPresets.SetActive(true);
        }
    }
}
