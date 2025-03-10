using System.Collections;
using System.Collections.Generic;
using GameAnalyticsSDK;
using UnityEngine;

public class GameAnalyticsController : MonoBehaviour
{
    private void Awake()
    {
        EventsManager.levelLoaded.AddListener(OnLevelStart);
        EventsManager.levelEndWithStatus.AddListener(OnLevelEnd);
    }

    private void Start()
    {
        GameAnalytics.Initialize();
    }

    private void OnDestroy()
    {
        EventsManager.levelLoaded.RemoveListener(OnLevelStart);
        EventsManager.levelEndWithStatus.RemoveListener(OnLevelEnd);
    }

    private void OnLevelStart(int levelIndex)
    {
        if (LevelManager.instance == null)
        {
            Debug.LogError($"GameAnalyticsController: LevelManager.instance is null");
            return;
        }

        //Debug.Log($"GameAnalyticsController: OnLevelStart: levelIndex={levelIndex}");

        GameAnalytics.NewProgressionEvent(
            GAProgressionStatus.Start,
            LevelManager.instance.playMode.ToString(),
            $"Level{levelIndex}"
        );

        Debug.Log($"GameAnalyticsController: OnLevelStart: end");
    }

    private void OnLevelEnd(StatusOfLevelEnd status)
    {
        if (LevelManager.instance == null)
        {
            Debug.LogError($"GameAnalyticsController: LevelManager.instance is null");
            return;
        }

        // Send event, if it is singleplayer campaign
        if (LevelManager.instance.playMode != PlayMode.PlayerVsAi_Campaign)
        {
            return;
        }

        switch (status)
        {
            case StatusOfLevelEnd.Player1Won:
                GameAnalytics.NewProgressionEvent(
                    GAProgressionStatus.Complete,
                    LevelManager.instance.playMode.ToString(),
                    LevelManager.instance.currentLevel.ToString()
                );
                break;

            case StatusOfLevelEnd.Player2Won:
                GameAnalytics.NewProgressionEvent(
                    GAProgressionStatus.Fail,
                    LevelManager.instance.playMode.ToString(),
                    LevelManager.instance.currentLevel.ToString()
                );
                break;

            default:
                Debug.LogError($"GameAnalyticsController: unexpected status of level end = {status}");
                break;
        }
    }
}
