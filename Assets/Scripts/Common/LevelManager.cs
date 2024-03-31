using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public enum PlayMode
{
    PlayerVsPlayer,
    PlayerVsAi,
    AiVsAi
}

public enum LevelModifier
{
    StarTriggerChangeBallDirection = 1,
    StarTriggerChangeMusic,
    StarTriggerChangeBallState,
    AdditionalPaddle,
    MakeBallDangerousAfterHitting,
    BossBigBase,
    StarTriggerGhost,
    LAST // it is NOT a modifier, it is pointer for cycles
}

// Manager for loading and setting levels (scenes)
public class LevelManager : MonoBehaviour
{
    // Public fields
    public static LevelManager instance = null;
    public PlayMode playMode;
    public int currentLevel = 0;
    public int hpPlayerLeft;
    public int hpPlayerRight;

    // Private fields
    private Dictionary<LevelModifier, byte> choosedModifiersFor1Player = new Dictionary<LevelModifier, byte>();
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

        // Set initial level modifiers
        foreach (LevelModifier i in Enum.GetValues(typeof(LevelModifier)))
        {
            choosedModifiersFor1Player[i] = 0;
            choosedModifiersFor2Players[i] = 0;
            choosedModifiersForAiVsAi[i] = 0;
        }
        choosedModifiersForAiVsAi[LevelModifier.StarTriggerChangeBallDirection] = 1;

        LoadBackgroundScene();
    }

    public void LoadBackgroundScene()
    {
        SetPlayMode(PlayMode.AiVsAi);
        hpPlayerLeft = hpPlayerRight = 99;
        SceneManager.LoadScene("Level");
    }

    #region PlayMode
    // Method for UI
    public void SetPlayModePlayerVsPlayer()
    {
        playMode = PlayMode.PlayerVsPlayer;
    }

    // Method for UI
    public void SetPlayModePlayerVsAi()
    {
        playMode = PlayMode.PlayerVsAi;
    }

    public void SetPlayMode(PlayMode playMode)
    {
        this.playMode = playMode;
    }

    public bool IsThisPlayerAi(int playerId)
    {
        if (playerId == 1)
        {
            return playMode == PlayMode.AiVsAi;
        }
        else if (playerId == 2)
        {
            return playMode == PlayMode.PlayerVsAi || playMode == PlayMode.AiVsAi;
        }
        else
        {
            Debug.LogError($"Unexpected player id {playerId}");
            return false;
        }
    }
    #endregion

    #region MethodsFor1Player
    public void LoadLevelFor1Player(int levelIndex, bool invokeEvent)
    {
        if (levelIndex < 1 || levelIndex > 5)
        {
            Debug.LogError($"LevelManager: SetLevelModifiers(): invalid level index = {levelIndex}");
            return;
        }

        currentLevel = levelIndex;

        if (levelIndex == 5)
        {
            // If level with Boss Big Base
            // Disable all usual modifiers
            for (int i = 0; i < (int)LevelModifier.LAST; i++)
            {
                choosedModifiersFor1Player[(LevelModifier)i] = 0;
            }
            choosedModifiersFor1Player[LevelModifier.MakeBallDangerousAfterHitting] = 1;
            choosedModifiersFor1Player[LevelModifier.BossBigBase] = 1;

            hpPlayerLeft = defaultHp;
            hpPlayerRight = 99;

            AudioManager.instance.SetMusicForBossBigBase();
        }
        else
        {
            // If level without boss
            // Set level modifiers
            for (int i = 1; i <= levelIndex; i++)
            {
                choosedModifiersFor1Player[(LevelModifier)i] = 1;
            }
            for (int i = levelIndex + 1; i < (int)LevelModifier.LAST; i++)
            {
                choosedModifiersFor1Player[(LevelModifier)i] = 0;
            }

            hpPlayerLeft = hpPlayerRight = defaultHp;
        }

        SceneManager.LoadScene("Level");

        if (invokeEvent)
        {
            EventsManager.levelLoaded.Invoke(levelIndex);
        }
    }

    public void LoadCurrentLevel()
    {
        LoadLevelFor1Player(currentLevel, true);
    }

    public void LoadNextLevel()
    {
        LoadLevelFor1Player(currentLevel + 1, true);
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

    public void SetModChangeBallStateFor2Players(bool cond)
    {
        choosedModifiersFor2Players[LevelModifier.StarTriggerChangeBallState] = cond ? (byte)1 : (byte)0;
    }

    public void SetModAdditionalPaddle2Players(bool cond)
    {
        choosedModifiersFor2Players[LevelModifier.AdditionalPaddle] = cond ? (byte)1 : (byte)0;
    }

    public void SetModStarGhostFor2Players(bool cond)
    {
        choosedModifiersFor2Players[LevelModifier.StarTriggerGhost] = cond ? (byte)1 : (byte)0;
    }

    public void LoadLevelFor2Players()
    {
        hpPlayerLeft = hpPlayerRight = defaultHp;
        SceneManager.LoadScene("Level");
        EventsManager.levelLoaded.Invoke(1);
    }
    #endregion

    public Dictionary<LevelModifier, byte> GetLevelModifiers()
    {
        if (playMode == PlayMode.PlayerVsPlayer)
        {
            return choosedModifiersFor2Players;
        }
        else if (playMode == PlayMode.PlayerVsAi)
        {
            return choosedModifiersFor1Player;
        }
        else if (playMode == PlayMode.AiVsAi)
        {
            return choosedModifiersForAiVsAi;
        }
        else
        {
            Debug.LogError($"LevelManager: GetLevelModifiers: unexpected playMode={playMode}");
            return null;
        }
    }
}
