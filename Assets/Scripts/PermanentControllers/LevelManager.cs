using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using Zenject;

// Manager for loading and setting levels (scenes)
public class LevelManager : MonoBehaviour
{
    // Public fields
    public static LevelManager instance {get; private set;} = null;
    public PlayMode playMode {get; private set;}
    public int currentLevel {get; private set;} = 0;
    public int hpPlayerLeft {get; private set;}
    public int hpPlayerRight {get; private set;}

    [Inject] private ConfigOfLevels _configOfLevelModifiers;

    // Private fields
    private ParameterLastCompletedLevel parameterLastCompletedLevel;
    private Dictionary<LevelModifier, byte> choosedModifiersFor1PlayerCampaign = new Dictionary<LevelModifier, byte>();
    private Dictionary<LevelModifier, byte> choosedModifiersFor1PlayerCustomBattle = new Dictionary<LevelModifier, byte>();
    private Dictionary<LevelModifier, byte> choosedModifiersFor2Players = new Dictionary<LevelModifier, byte>();
    private Dictionary<LevelModifier, byte> choosedModifiersForAiVsAi = new Dictionary<LevelModifier, byte>();
    private int defaultHp = 5;

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

        parameterLastCompletedLevel = new ParameterLastCompletedLevel();

        // Set initial level modifiers
        foreach (LevelModifier i in Enum.GetValues(typeof(LevelModifier)))
        {
            choosedModifiersFor1PlayerCampaign[i] = 0;
            choosedModifiersFor1PlayerCustomBattle[i] = 0;
            choosedModifiersFor2Players[i] = 0;
            choosedModifiersForAiVsAi[i] = 0;
        }
        choosedModifiersForAiVsAi[LevelModifier.StarTriggerChangeBallDirection] = 1;

        EventsManager.levelEndWithStatus.AddListener(OnLevelEnd);

        LoadMainMenuAndLevel();
    }

    public void LoadMainMenuAndLevel()
    {
        SceneManager.LoadScene(SceneName.MainMenu.ToString());
        playMode = PlayMode.AiVsAi;
        hpPlayerLeft = hpPlayerRight = 999999;
        SceneManager.LoadScene(SceneName.Level.ToString(), LoadSceneMode.Additive);
    }

    #region MethodsFor1PlayerCampaign
    public void LoadLevelFor1PlayerCampaign(int levelIndex, bool invokeEvent)
    {
        if (_configOfLevelModifiers == null)
        {
            Debug.LogError($"LevelManager: LoadLevelFor1PlayerCampaign: no ref to ConfigOfLevels");
            return;
        }

        if (levelIndex < 0 || levelIndex >= _configOfLevelModifiers.Levels.Count)
        {
            Debug.LogError($"LevelManager: SetLevelModifiers(): invalid level index = {levelIndex}");
            return;
        }

        playMode = PlayMode.PlayerVsAi_Campaign;

        currentLevel = levelIndex;

        SettingsOfLevel settingsOfLevel = _configOfLevelModifiers.Levels[currentLevel];

        // Set level modifiries
        choosedModifiersFor1PlayerCampaign.Clear();
        for (int i = 0; i < settingsOfLevel.LevelModifiers.Count; i++)
        {
            choosedModifiersFor1PlayerCampaign.Add(settingsOfLevel.LevelModifiers[i], 1);
        }

        AudioManager.instance.SetMusicPreset(settingsOfLevel.MusicPreset);

        // Set HP
        hpPlayerLeft = settingsOfLevel.HpOfLeftParticipant;
        hpPlayerRight = settingsOfLevel.HpOfRightParticipant;

        SceneManager.LoadScene(SceneName.Level.ToString());

        if (invokeEvent)
        {
            EventsManager.levelLoaded.Invoke(levelIndex);
        }
    }

    public void LoadCurrentLevel()
    {
        LoadLevelFor1PlayerCampaign(currentLevel, true);
    }

    public void LoadNextLevel()
    {
        LoadLevelFor1PlayerCampaign(currentLevel + 1, true);
    }

    public void OnLevelEnd(StatusOfLevelEnd status)
    {
        // Checking for update last completed campaign level
        if (playMode == PlayMode.PlayerVsAi_Campaign && status == StatusOfLevelEnd.Player1Won)
        {
            int savedLastLevel = parameterLastCompletedLevel.GetCurrentValue();
            if (currentLevel > savedLastLevel)
            {
                parameterLastCompletedLevel.SetNewValue(currentLevel);
            }
        }
    }

    public void UnlockAllLevels()
    {
        int lastLevel = _configOfLevelModifiers.Levels.Count - 1;
        parameterLastCompletedLevel.SetNewValue(lastLevel);
    }
    #endregion

    #region MethodsFor1PlayerCustomBattle
    
    public void SetModChangeBallDirectionFor1Player(bool cond)
    {
        choosedModifiersFor1PlayerCustomBattle[LevelModifier.StarTriggerChangeBallDirection] = cond ? (byte)1 : (byte)0;
    }

    public void SetModChangeMusicFor1Player(bool cond)
    {
        choosedModifiersFor1PlayerCustomBattle[LevelModifier.StarTriggerChangeMusic] = cond ? (byte)1 : (byte)0;
    }

    public void SetModDangerousBallStateFor1Player(bool cond)
    {
        choosedModifiersFor1PlayerCustomBattle[LevelModifier.StarTriggerMakeBallDangerous] = cond ? (byte)1 : (byte)0;
    }

    public void SetModAdditionalPaddle1Player(bool cond)
    {
        choosedModifiersFor1PlayerCustomBattle[LevelModifier.AdditionalPaddle] = cond ? (byte)1 : (byte)0;
    }

    public void SetModMakeBallDangerousAfterHittingFor1Player(bool cond)
    {
        choosedModifiersFor1PlayerCustomBattle[LevelModifier.MakeBallDangerousAfterHitting] = cond ? (byte)1 : (byte)0;
    }

    public void SetModStarGhostFor1Player(bool cond)
    {
        choosedModifiersFor1PlayerCustomBattle[LevelModifier.StarTriggerGhost] = cond ? (byte)1 : (byte)0;
    }

    public void SetModTwitchesIncreaseImpactFor1Player(bool cond)
    {
        choosedModifiersFor1PlayerCustomBattle[LevelModifier.TwitchingIncreasesImpactOnBall] = cond ? (byte)1 : (byte)0;
    }

    public void LoadLevelFor1PlayerCustomBattle()
    {
        playMode = PlayMode.PlayerVsAi_CustomBattle;
        hpPlayerLeft = hpPlayerRight = defaultHp;
        
        currentLevel = -1;
        SceneManager.LoadScene(SceneName.Level.ToString());
        EventsManager.levelLoaded.Invoke(currentLevel);
    }

    #endregion

    #region MethodsFor2Players

    public void SetModChangeBallDirectionFor2Players(bool cond)
    {
        choosedModifiersFor2Players[LevelModifier.StarTriggerChangeBallDirection] = cond ? (byte)1 : (byte)0;
    }

    public void SetModChangeMusicFor2Players(bool cond)
    {
        choosedModifiersFor2Players[LevelModifier.StarTriggerChangeMusic] = cond ? (byte)1 : (byte)0;
    }

    public void SetModDangerousBallStateFor2Players(bool cond)
    {
        choosedModifiersFor2Players[LevelModifier.StarTriggerMakeBallDangerous] = cond ? (byte)1 : (byte)0;
    }

    public void SetModAdditionalPaddle2Players(bool cond)
    {
        choosedModifiersFor2Players[LevelModifier.AdditionalPaddle] = cond ? (byte)1 : (byte)0;
    }

    public void SetModMakeBallDangerousAfterHittingFor2Players(bool cond)
    {
        choosedModifiersFor2Players[LevelModifier.MakeBallDangerousAfterHitting] = cond ? (byte)1 : (byte)0;
    }

    public void SetModStarGhostFor2Players(bool cond)
    {
        choosedModifiersFor2Players[LevelModifier.StarTriggerGhost] = cond ? (byte)1 : (byte)0;
    }

    public void SetModTwitchesIncreaseImpactFor2Players(bool cond)
    {
        choosedModifiersFor2Players[LevelModifier.TwitchingIncreasesImpactOnBall] = cond ? (byte)1 : (byte)0;
    }

    public void LoadLevelFor2Players()
    {
        playMode = PlayMode.PlayerVsPlayer;
        hpPlayerLeft = hpPlayerRight = defaultHp;
        
        currentLevel = -1;
        SceneManager.LoadScene(SceneName.Level.ToString());
        EventsManager.levelLoaded.Invoke(-1);
    }

    #endregion

    #region GetMethods

    public bool IsThisPlayerAi(int playerId)
    {
        if (playerId == 1)
        {
            return playMode == PlayMode.AiVsAi;
        }
        else if (playerId == 2)
        {
            return playMode == PlayMode.PlayerVsAi_Campaign || playMode == PlayMode.AiVsAi || playMode == PlayMode.PlayerVsAi_CustomBattle;
        }
        else
        {
            Debug.LogError($"Unexpected player id {playerId}");
            return false;
        }
    }

    public Dictionary<LevelModifier, byte> GetLevelModifiers()
    {
        switch (playMode)
        {
            case PlayMode.PlayerVsPlayer:
                return choosedModifiersFor2Players;
            case PlayMode.PlayerVsAi_Campaign:
                return choosedModifiersFor1PlayerCampaign;
            case PlayMode.PlayerVsAi_CustomBattle:
                return choosedModifiersFor1PlayerCustomBattle;
            case PlayMode.AiVsAi:
                return choosedModifiersForAiVsAi;
            default:
                Debug.LogError($"LevelManager: GetLevelModifiers: unexpected playMode={playMode}");
                return null;
        }
    }

    public int GetLastCompletedLevel()
    {
        return parameterLastCompletedLevel.GetCurrentValue();
    }

    #endregion
}
