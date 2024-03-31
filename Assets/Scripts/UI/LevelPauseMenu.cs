using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Localization;
using UnityEngine.UI;

public class LevelPauseMenu : MonoBehaviour
{
    public static UnityEvent<bool> eventActivateMainGroup = new UnityEvent<bool>();
    public static UnityEvent<int> eventLevelEnd = new UnityEvent<int>();
    public static UnityEvent eventShowPanelEnd = new UnityEvent();

    [Header("References to my objects")]
    [SerializeField] private CanvasGroup mainCanvasGroup;
    [SerializeField] private TextMeshProUGUI mainLabel;
    [SerializeField] private Button buttonRevengeOrNextLevel;
    [SerializeField] private TextMeshProUGUI textRevengeOrNextLevel;
    [SerializeField] private GameObject buttonContinue;
    [SerializeField] private GameObject buttonQuit;
    [SerializeField] private GameObject panelTheEnd;

    [Header("Localized strings")]
    [SerializeField] private LocalizedString lsLeftPlayerWins;
    [SerializeField] private LocalizedString lsRightPlayerWins;
    [SerializeField] private LocalizedString lsYouWon;
    [SerializeField] private LocalizedString lsYouLost;
    [SerializeField] private LocalizedString lsRevenge;
    [SerializeField] private LocalizedString lsNextLevel;

    private bool isActive = true;

    private void Awake()
    {
        if (LevelManager.instance.playMode == PlayMode.AiVsAi)
        {
            // If loaded level AI vs AI then delete this menu (assert: we are in main menu)
            Destroy(gameObject);
        }

#if UNITY_WEBGL
        buttonQuit.SetActive(false);
#endif

        eventActivateMainGroup.AddListener(ActivateThisMenu);
        eventLevelEnd.AddListener(OnLevelEnd);
        eventShowPanelEnd.AddListener(ShowPanelTheEnd);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Esc key was pressed");
            if (mainCanvasGroup.gameObject.activeInHierarchy)
            {
                ActivateThisMenu(false);
            }
            else
            {
                ActivateThisMenu(true);
            }
        }
    }

    public void ActivateThisMenu(bool value)
    {
        mainCanvasGroup.gameObject.SetActive(value);
        isActive = value;

        if (isActive)
            Time.timeScale = 0f;
        else
        {
            Time.timeScale = 1f;
            PanelSettings.eventActivate.Invoke(false);
        }
    }

    private void OnLevelEnd(int winnerId)
    {
        ChangeTextOfMainLabel(winnerId);

        buttonContinue.SetActive(false);

        if (LevelManager.instance.currentLevel == 5 && winnerId == 1) // TODO: make it check more flexible
        {
            // End of the game - don't show button with "Next level"
            return;
        }

        buttonRevengeOrNextLevel.gameObject.SetActive(true);

        if (LevelManager.instance.playMode == PlayMode.PlayerVsPlayer)
        {
            textRevengeOrNextLevel.text = lsRevenge.GetLocalizedString();
            buttonRevengeOrNextLevel.onClick.AddListener(LevelManager.instance.LoadLevelFor2Players);
        }
        else if (LevelManager.instance.playMode == PlayMode.PlayerVsAi)
        {
            if (winnerId == 1)
            {
                textRevengeOrNextLevel.text = lsNextLevel.GetLocalizedString();
                buttonRevengeOrNextLevel.onClick.AddListener(LevelManager.instance.LoadNextLevel);
            }
            else
            {
                textRevengeOrNextLevel.text = lsRevenge.GetLocalizedString();
                buttonRevengeOrNextLevel.onClick.AddListener(LevelManager.instance.LoadCurrentLevel);
            }
        }
    }

    public void ChangeTextOfMainLabel(int winnerId)
    {
        if (LevelManager.instance.playMode == PlayMode.PlayerVsPlayer)
        {
            if (winnerId == 1)
                mainLabel.text = lsLeftPlayerWins.GetLocalizedString();
            else if (winnerId == 2)
                mainLabel.text = lsRightPlayerWins.GetLocalizedString();
            else
                Debug.LogError($"LevelPauseMenu: ChangeTextOfMainLabel: unexpected winnerId={winnerId}");
        }
        else if (LevelManager.instance.playMode == PlayMode.PlayerVsAi)
        {
            if (winnerId == 1)
                mainLabel.text = lsYouWon.GetLocalizedString();
            else if (winnerId == 2)
                mainLabel.text = lsYouLost.GetLocalizedString();
            else
                Debug.LogError($"LevelPauseMenu: ChangeTextOfMainLabel: unexpected winnerId={winnerId}");
        }
    }

    public void OpenSettings()
    {
        PanelSettings.eventActivate.Invoke(true);
        Debug.Log("LevelPauseMenu: OpenSettings: end");
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
        MainMenu.instance.ActivateMainGroup(true);
        LevelManager.instance.LoadBackgroundScene();
        AudioManager.instance.PlayMainMenuMusic();
        Destroy(gameObject);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void ShowPanelTheEnd()
    {
        panelTheEnd.SetActive(true);
    }
}
