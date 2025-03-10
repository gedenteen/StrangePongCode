using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class GameManager : MonoBehaviour
{
    // Public Fields
    public static GameManager instance;
    public Action onReset;

    [Header("Changable fields")]
    public int scorePlayer1;
    public int scorePlayer2;

    [Header("References to objects")]
    public ScoreText scoreTextLeft;
    public ScoreText scoreTextRight;
    //public TextMeshProUGUI winText;
    public Ball ball;
    public StarsSpawner starsSpawner;
    public MainPaddle rightPaddle;
    public BossBigBase bossBigBase;
    public BossBigBarrier bossBigBarrier;

    [Inject] private ConfigOfLevels _configOfLevels;

    private bool _isLevelEnd = false;

    public void Initialize()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.Log("There is 2 (at least) GameManagers (should be 1)");
            Destroy(gameObject);
            return;
        }

        if (LevelManager.instance.playMode == PlayMode.AiVsAi)
        {
            scoreTextLeft.gameObject.SetActive(false);
            scoreTextRight.gameObject.SetActive(false);
        }

        HandleLevelModifiers();
        UpdateScores();
    }

    private void HandleLevelModifiers()
    {
        var levelModifiers = LevelManager.instance.GetLevelModifiers();

        if (levelModifiers.ContainsKey(LevelModifier.StarTriggerChangeBallDirection) && levelModifiers[LevelModifier.StarTriggerChangeBallDirection] > 0)
        {
            starsSpawner.spawnStarsBallDirection = true;
        }
        else
        {
            starsSpawner.spawnStarsBallDirection = false;
        }

        if (levelModifiers.ContainsKey(LevelModifier.StarTriggerChangeMusic) && levelModifiers[LevelModifier.StarTriggerChangeMusic] > 0)
        {
            starsSpawner.spawnStarsMusic = true;
        }
        else
        {
            starsSpawner.spawnStarsMusic = false;
        }

        if (levelModifiers.ContainsKey(LevelModifier.StarTriggerGhost) && levelModifiers[LevelModifier.StarTriggerGhost] > 0)
        {
            starsSpawner.spawnStarsGhost = true;
        }
        else
        {
            starsSpawner.spawnStarsGhost = false;
        }

        if (levelModifiers.ContainsKey(LevelModifier.StarTriggerMakeBallDangerous) && levelModifiers[LevelModifier.StarTriggerMakeBallDangerous] > 0)
        {
            starsSpawner.spawnStarsBallState = true;
        }
        else
        {
            starsSpawner.spawnStarsBallState = false;
        }

        if (levelModifiers.ContainsKey(LevelModifier.ForceSetChromaticAberration) && levelModifiers[LevelModifier.ForceSetChromaticAberration] > 0)
        {
            Settings.instance.SetChromaticAverrationValue(_configOfLevels.ValueOfForceSetChromaticAbberration);
        }

        if (levelModifiers.ContainsKey(LevelModifier.ForceSetBloom) && levelModifiers[LevelModifier.ForceSetBloom] > 0)
        {
            Settings.instance.SetBloomValue(_configOfLevels.ValueOfForceSetBloom);
        }

        if (levelModifiers.ContainsKey(LevelModifier.BossBigBase) && levelModifiers[LevelModifier.BossBigBase] > 0)
        {
            rightPaddle.gameObject.SetActive(false); // disable default enemy Paddle
            ball.gameObject.SetActive(false); //disable default Ball for levels 1-4
            bossBigBase.gameObject.SetActive(true); // and activate BOSS BIG BASE
        }

        if (levelModifiers.ContainsKey(LevelModifier.BossBigBarrier) && levelModifiers[LevelModifier.BossBigBarrier] > 0)
        {
            rightPaddle.gameObject.SetActive(false); // disable default enemy Paddle
            bossBigBarrier.gameObject.SetActive(true); // and activate BOSS BIG BASE
        }

        scorePlayer1 = LevelManager.instance.hpPlayerLeft;
        scorePlayer2 = LevelManager.instance.hpPlayerRight;
    }

    public void OnScoreZoneReached(BallState ballState, int id)
    {
        if (ballState == BallState.usual)
        {
            DecreaseHpOfPlayer(id);
        }
        else if (ballState == BallState.dangerous)
        {
            onReset?.Invoke();
        }
        else
        {
            Debug.LogError($"GameManager: OnScoreZoneReached(): unxpected ball state = {ballState}");
        }
    }

    public void DecreaseHpOfPlayer(int id)
    {
        if (id == 1)
        {
            scorePlayer1--;
        }
        else if (id == 2)
        {
            scorePlayer2--;
        }
        else
        {
            Debug.LogError($"GameManager: DecreaseHpOfPlayer(): unxpected id={id} of ScoreZone");
        }

        UpdateScores();
        HighlightScore(id);
    }

    private void UpdateScores()
    {
        scoreTextLeft.SetScore(scorePlayer1);
        scoreTextRight.SetScore(scorePlayer2);

        if (scorePlayer1 == 0)
        {
            OnLevelEnds(2);
        }
        else if (scorePlayer2 == 0)
        {
            OnLevelEnds(1);
        }
        else
        {
            onReset?.Invoke();
        }
    }

    private void OnLevelEnds(int winnerId)
    {
        if (_isLevelEnd)
        {
            return;
        }
        _isLevelEnd = true;

        Debug.Log($"OnGameEnds: winner {winnerId}");

        // If it's a level with a BossBigBase and the player wins
        Dictionary<LevelModifier, byte> levelMods = LevelManager.instance.GetLevelModifiers();

        if (winnerId == 2)
        {
            StartCoroutine(FinishLevel(winnerId, 0f));
            return;
        }

        // Assert winnerId = 1
        if (levelMods.ContainsKey(LevelModifier.BossBigBase) && levelMods[LevelModifier.BossBigBase] != 0)
        {
            bossBigBase.Die();
            StartCoroutine(FinishLevel(winnerId, 4f));
        }
        else if (levelMods.ContainsKey(LevelModifier.BossBigBarrier) && levelMods[LevelModifier.BossBigBarrier] != 0)
        {
            ball.gameObject.SetActive(false);
            Destroy(ball);

            bossBigBarrier.Die();
            StartCoroutine(FinishLevel(winnerId, 4f));
        }
        else
        {
            StartCoroutine(FinishLevel(winnerId, 0f));
        }
            
        if (LevelManager.instance.currentLevel == _configOfLevels.Levels.Count - 1)
        {
            StartCoroutine(DoTheEnd(6f));
        }
    }

    public void HighlightScore(int id)
    {
        if (id == 1)
        {
            scoreTextLeft.Highlight();
        }
        else if (id == 2)
        {
            scoreTextRight.Highlight();
        }
    }

    private IEnumerator FinishLevel(int winnerId, float timeToWait)
    {
        if (timeToWait > 0f)
        {
            yield return new WaitForSecondsRealtime(timeToWait);
        }

        if (winnerId == 1)
        {
            AudioManager.instance.PlayWinSound();
            if (LevelManager.instance.playMode == PlayMode.PlayerVsAi_Campaign)
            {
                DifficultyManager.instance.ChangePlayerSkillValue(true);
            }
        }
        else
        {
            AudioManager.instance.PlayLooseSound();
            if (LevelManager.instance.playMode == PlayMode.PlayerVsAi_Campaign)
            {
                DifficultyManager.instance.ChangePlayerSkillValue(false);
            }
        }

        StatusOfLevelEnd status = winnerId == 1 ? StatusOfLevelEnd.Player1Won : StatusOfLevelEnd.Player2Won;

        EventsManager.levelEnd.Invoke();
        EventsManager.levelEndWithStatus.Invoke(status);
        LevelPauseMenu.eventLevelEnd.Invoke(winnerId);
        LevelPauseMenu.eventActivateMainGroup.Invoke(true);
    }

    private IEnumerator DoTheEnd(float timeToWait)
    {
        LevelPauseMenu.eventShowPanelEnd.Invoke();

        if (timeToWait > 0f)
        {
            yield return new WaitForSecondsRealtime(timeToWait);
        }

        AudioManager.instance.SetEndMusicAndPlay();
    }
}
