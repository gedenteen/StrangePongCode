using UnityEngine;
using TMPro;
using UnityEngine.Events;
using UnityEngine.Localization;

public class MainMenu : MonoBehaviour
{
    public static MainMenu instance;
    public static UnityEvent eventHideMenu = new UnityEvent();
    public static UnityEvent<bool> eventActivateMainGroup = new UnityEvent<bool>();

    private bool isActive = true;

    [Header("References")]
    [SerializeField] private GameObject startPanelChoosingLocale;
    [SerializeField] private GameObject warningForEpileptics;
    [SerializeField] private TextMeshProUGUI mainLabel;
    [SerializeField] private TextMeshProUGUI textLabel;
    [SerializeField] private GameObject buttonQuit;
    [SerializeField] private GameObject buttonCredits;
    [SerializeField] private CanvasGroup mainCanvasGroup;

    [Header("Localized strings")]
    [SerializeField] private LocalizedString lsLeftPlayerWins;
    [SerializeField] private LocalizedString lsRightPlayerWins;
    [SerializeField] private LocalizedString lsYouWon;
    [SerializeField] private LocalizedString lsYouLost;
    [SerializeField] private LocalizedString lsBotsDonePlaying;

    // Private const fields
    private readonly string pp_selectedLocale = "SelectedLocale";

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

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

        textLabel.text = "version " + Application.version;

        EventsManager.levelLoaded.AddListener(LevelWasLoaded);
        eventActivateMainGroup.AddListener(ActivateMainGroup);
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
        isActive = value;
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

    public void Singleplayer()
    {
        LoadSignleplayerLevel(1);
    }

    public void LoadSignleplayerLevel(int levelNumber)
    {
        if (levelNumber < 0)
        {
            Debug.LogError($"MainMenu: LoadSignleplayerLevel(): invalid levelNumber={levelNumber}");
        }

        LevelManager.instance.LoadLevelFor1Player(levelNumber, true);
    }

    public void OnButtonQuitClicked()
    {
        Debug.Log("MainMenu: OnButtonQuitClicked");
        Application.Quit();
    }
}
