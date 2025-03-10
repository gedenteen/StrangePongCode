using UnityEngine;
using TMPro;
using UnityEngine.Events;
using UnityEngine.Localization;
using UnityEngine.UI;
using System.Collections.Generic;
using Zenject;

public class MainMenu : MonoBehaviour
{
    [Header("References to child objects: panels")]
    [SerializeField] private GameObject _panelPrimary;
    [SerializeField] private GameObject _panelOnePlayer;
    [SerializeField] private GameObject _panelTwoPlayers;
    
    [Header("References to child objects: other")]
    [SerializeField] private GameObject startPanelChoosingLocale;
    [SerializeField] private GameObject warningForEpileptics;
    [SerializeField] private TextMeshProUGUI mainLabel;
    [SerializeField] private TextMeshProUGUI textLabel;
    [SerializeField] private GameObject buttonQuit;
    [SerializeField] private GameObject buttonCredits;
    [SerializeField] private Button buttonSettings;
    [SerializeField] private Button buttonDiscord;
    [SerializeField] private CanvasGroup mainCanvasGroup;

    [Header("Localized strings")]
    [SerializeField] private LocalizedString lsLeftPlayerWins;
    [SerializeField] private LocalizedString lsRightPlayerWins;
    [SerializeField] private LocalizedString lsYouWon;
    [SerializeField] private LocalizedString lsYouLost;
    [SerializeField] private LocalizedString lsBotsDonePlaying;

    [Inject] private GameConfig _gameConfig;

    // Private fields
    private List<GameObject> _listOfPanels = new List<GameObject>();

    // Private const fields
    private readonly string pp_selectedLocale = "SelectedLocale";

    private void Awake()
    {
        // Deactivaing/activating objects
#if UNITY_WEBGL
        buttonQuit.SetActive(false);
#endif

        if (MyBuildSettings.currentBuildPlatform == BuildPlatform.Yandex)
            buttonCredits.SetActive(false);

        if (!PlayerPrefs.HasKey(pp_selectedLocale))
        {
            // If player run this game at first time, then show panel for choosing language
            startPanelChoosingLocale.SetActive(true);
            warningForEpileptics.SetActive(true);
        }

        // Show game version
        textLabel.text = "version " + Application.version;

        // Fill _listOfPlayers
        _listOfPanels.Add(_panelPrimary);
        _listOfPanels.Add(_panelOnePlayer);
        _listOfPanels.Add(_panelTwoPlayers);

        // Add listeners
        buttonSettings.onClick.AddListener(OnClickButtonSettings);
        buttonDiscord.onClick.AddListener(OnClickButtonDiscord);
        EventsManager.levelLoaded.AddListener(LevelWasLoaded);
    }

    private void OnDestroy()
    {
        buttonSettings.onClick.RemoveListener(OnClickButtonSettings);
    }

    private void LevelWasLoaded(int level)
    {
        Time.timeScale = 1f;
        //ChangeTextOfMainLabel("Pause");
        ActivateMainGroup(false);
    }

    public void ActivateMainGroup(bool value)
    {
        mainCanvasGroup.gameObject.SetActive(value);
    }

    public void ChangeTextOfMainLabel(string text)
    {
        mainLabel.text = text;
    }

    public void ChangeTextOfMainLabel(int winnerId)
    {
        if (LevelManager.instance.playMode == PlayMode.AiVsAi)
        {
            mainLabel.text = lsBotsDonePlaying.GetLocalizedString();
        }
        else
        {
            Debug.LogError($"MainMenu: ChangeTextOfMainLabel: unexpected playMode={LevelManager.instance.playMode}");
        }
    }

    public void OnButtonQuitClicked()
    {
        Debug.Log("MainMenu: OnButtonQuitClicked");
        Application.Quit();
    }

    private void OnClickButtonSettings()
    {
        Debug.Log("MainMenu: OnClickButtonSettings");
        PanelSettings.eventActivate.Invoke(true); // TODO: через zenject это делать
    }

    private void OnClickButtonDiscord()
    {
        Application.OpenURL(_gameConfig.DiscordServerUrl);
    }


    #region Methods for change active panel

    private void ActivateOnlyCertainPanel(GameObject targetPanel)
    {
        for (int i = 0; i < _listOfPanels.Count; i++)
        {
            GameObject panel = _listOfPanels[i];
            panel.SetActive(panel == targetPanel);
        }
    }

    public void ActivatePanelPrimary()
    {
        ActivateOnlyCertainPanel(_panelPrimary);
    }

    public void ActivatePanelOnePlayer()
    {
        ActivateOnlyCertainPanel(_panelOnePlayer);
    }

    public void ActivatePanelTwoPlayers()
    {
        ActivateOnlyCertainPanel(_panelTwoPlayers);
    }

    #endregion
}
