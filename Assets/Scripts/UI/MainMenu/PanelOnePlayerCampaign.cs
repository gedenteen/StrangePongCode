using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelOnePlayerCampaign : MonoBehaviour
{
    [SerializeField] private List<Button> _listOfButtonsLoadLevel;
    [SerializeField] private GameObject _holderForButtonsLoadLevel;

    private void OnValidate()
    {
        // Fill _listOfButtonsLoadLevel automatically
        if (_listOfButtonsLoadLevel == null || _listOfButtonsLoadLevel.Count == 0)
        {
            foreach (Button button in _holderForButtonsLoadLevel.GetComponentsInChildren<Button>())
            {
                _listOfButtonsLoadLevel.Add(button);
            }
        }
    }

    private void Awake()
    {
        for (int i = 0; i < _listOfButtonsLoadLevel.Count; i++)
        {
            Button button = _listOfButtonsLoadLevel[i];
            int levelIndex = i;
            button.onClick.AddListener(() => LoadGameLevel(levelIndex));
        }
    }

    public void OnEnable()
    {
        // Enable and disable buttons of levels:
        int lastCompletedLevel = LevelManager.instance.GetLastCompletedLevel();
        
        _listOfButtonsLoadLevel[0].enabled = true;
        for (int i = 1; i < _listOfButtonsLoadLevel.Count; i++)
        {
            _listOfButtonsLoadLevel[i].interactable = i <= (lastCompletedLevel + 1);
        }
    }

    private void OnDestroy()
    {
        for (int i = 0; i < _listOfButtonsLoadLevel.Count; i++)
        {
            Button button = _listOfButtonsLoadLevel[i];
            button.onClick.RemoveAllListeners();
        }
    }

    private void LoadGameLevel(int levelIndex)
    {
        LevelManager.instance.LoadLevelFor1PlayerCampaign(levelIndex, true);
    }
}
