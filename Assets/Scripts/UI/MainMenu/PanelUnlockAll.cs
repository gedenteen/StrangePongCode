using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Zenject;

public class PanelUnlockAll : MonoBehaviour
{
    [Header("References to my objects")]
    [SerializeField] private Button _openPanelButton;
    [SerializeField] private GameObject _contentHolder;
    [SerializeField] private TextMeshProUGUI _textMeshForPhrase;
    [SerializeField] private TMP_InputField _inputField;
    [SerializeField] private Button _closeButton;
    [SerializeField] private Button _checkButton;

    [Header("References to other objects")]
    [SerializeField] private PanelOnePlayerCampaign _panelOnePlayerCampaign;
    [SerializeField] private PanelOnePlayerCustomBattle _panelOnePlayerCustomBattle;

    [Inject] private GameConfig _gameConfig;

    private void Awake()
    {
        if (_gameConfig == null)
        {
            Debug.LogError($"PanelUnlockAll: Awake: i not got GameConfig");
        }
        _textMeshForPhrase.text = _gameConfig.PhraseForUnlockAll;

        // Add listener for the button that opens the panel
        _openPanelButton.onClick.AddListener(OpenPanel);

        // Add listeners for the panel buttons
        _closeButton.onClick.AddListener(ClosePanel);
        _checkButton.onClick.AddListener(CheckInput);

        // Hide the panel at start
        _contentHolder.SetActive(false);
    }

    private void OnDestroy()
    {
        // Remove listeners to prevent memory leaks
        _openPanelButton.onClick.RemoveListener(OpenPanel);
        _closeButton.onClick.RemoveListener(ClosePanel);
        _checkButton.onClick.RemoveListener(CheckInput);
    }

    // Method to open the panel
    private void OpenPanel()
    {
        _contentHolder.SetActive(true);
    }

    // Method to close the panel
    private void ClosePanel()
    {
        _contentHolder.SetActive(false);
    }

    // Method to check the text in the InputField
    private void CheckInput()
    {
        string trimmedInput = _inputField.text.Trim();
        if (trimmedInput.Equals(_gameConfig.PhraseForUnlockAll, StringComparison.OrdinalIgnoreCase))
        {
            LevelManager.instance.UnlockAllLevels();
            RewardsController.Instance.UnlockAllRewards();
            _panelOnePlayerCampaign.OnEnable();
            _panelOnePlayerCustomBattle.OnEnable();
            Debug.Log("PanelUnlockAll: CheckInput: Correct phrase entered!");
        }
        else
        {
            Debug.Log("PanelUnlockAll: CheckInput: Entered phrase is incorrect.");
        }
    }
}
