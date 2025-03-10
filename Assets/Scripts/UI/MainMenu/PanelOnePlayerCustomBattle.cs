using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelOnePlayerCustomBattle : MonoBehaviour
{
    [SerializeField] private Button _buttonPlay;

    [Header("Toggles")]
    [SerializeField] private Toggle _toggleTriggerChangeDirection;
    [SerializeField] private Toggle _toggleTriggerChangeMusic;
    [SerializeField] private Toggle _toggleTriggerMakeBallDangerous;
    [SerializeField] private Toggle _toggleAdditionalPaddle;
    [SerializeField] private Toggle _toggleMakeBallDangerousAfterHit;
    [SerializeField] private Toggle _toggleTriggerGhost;
    [SerializeField] private Toggle _toggleTwitchingIncreasesImpact;

    [Header("Cells")]
    [SerializeField] private GameObject _cellTriggerChangeDirection;
    [SerializeField] private GameObject _cellTriggerChangeMusic;
    [SerializeField] private GameObject _cellTriggerMakeBallDangerous;
    [SerializeField] private GameObject _cellTriggerGhost;
    [SerializeField] private GameObject _cellAdditionalPaddle;
    [SerializeField] private GameObject _cellTwitchingIncreasesImpact;

    [Header("References to my objects: music pack")]
    [SerializeField] private Toggle _toggleMusicPack1;
    [SerializeField] private Toggle _toggleMusicPack2;

    private MusicPreset _selectedMusicPreset;

    private void Awake()
    {
        _buttonPlay.onClick.AddListener(Play);

        _toggleTriggerChangeDirection.onValueChanged.AddListener(SetModChangeBallDirection);
        _toggleTriggerChangeMusic.onValueChanged.AddListener(SetModChangeChangeMusic);
        _toggleTriggerMakeBallDangerous.onValueChanged.AddListener(SetModDangerousBall);
        _toggleAdditionalPaddle.onValueChanged.AddListener(SetModAdditionalPaddle);
        _toggleMakeBallDangerousAfterHit.onValueChanged.AddListener(SetModMakeBallDangerousAfterHit);
        _toggleTriggerGhost.onValueChanged.AddListener(SetModGhostBall);
        _toggleTwitchingIncreasesImpact.onValueChanged.AddListener(SetModTwitchesIncreaseImpact);

        _toggleMusicPack1.onValueChanged.AddListener(SetMusicPack1);
        _toggleMusicPack2.onValueChanged.AddListener(SetMusicPack2);
        _toggleMusicPack1.isOn = true;
    }

    public void OnEnable()
    {
        HideAndShowMods();
    }

    private void OnDestroy()
    {
        _buttonPlay.onClick.RemoveListener(Play);

        _toggleTriggerChangeDirection.onValueChanged.RemoveListener(SetModChangeBallDirection);
        _toggleTriggerChangeMusic.onValueChanged.RemoveListener(SetModChangeChangeMusic);
        _toggleTriggerMakeBallDangerous.onValueChanged.RemoveListener(SetModDangerousBall);
        _toggleAdditionalPaddle.onValueChanged.RemoveListener(SetModAdditionalPaddle);
        _toggleMakeBallDangerousAfterHit.onValueChanged.RemoveListener(SetModMakeBallDangerousAfterHit);
        _toggleTriggerGhost.onValueChanged.RemoveListener(SetModGhostBall);
        _toggleTwitchingIncreasesImpact.onValueChanged.RemoveListener(SetModTwitchesIncreaseImpact);
        
        _toggleMusicPack1.onValueChanged.RemoveListener(SetMusicPack1);
        _toggleMusicPack2.onValueChanged.RemoveListener(SetMusicPack2);
    }

    private void HideAndShowMods()
    {
        if (RewardsController.Instance == null)
        {
            Debug.LogError($"PanelOnePlayerCustomBattle: HideLockedMods: " +
                $"RewardsController.Instance == null");
            return;
        }

        if (!RewardsController.Instance.HasReward(RewardId.UnlockModStarTriggerChangeDirection))
        {
            _cellTriggerChangeDirection.gameObject.SetActive(false);
        }
        else
        {
            _cellTriggerChangeDirection.gameObject.SetActive(true);
        }

        if (!RewardsController.Instance.HasReward(RewardId.UnlockModStarTriggerChangeMusic))
        {
            _cellTriggerChangeMusic.gameObject.SetActive(false);
        }
        else
        {
            _cellTriggerChangeMusic.gameObject.SetActive(true);
        }

        if (!RewardsController.Instance.HasReward(RewardId.UnlockModStarTriggerMakeBallDangerous))
        {
            _cellTriggerMakeBallDangerous.gameObject.SetActive(false);
        }
        else
        {
            _cellTriggerMakeBallDangerous.gameObject.SetActive(true);
        }

        if (!RewardsController.Instance.HasReward(RewardId.UnlockModStarTriggerCreateGhostBall))
        {
            _cellTriggerGhost.gameObject.SetActive(false);
        }
        else
        {
            _cellTriggerGhost.gameObject.SetActive(true);
        }

        if (!RewardsController.Instance.HasReward(RewardId.UnlockModTwitching))
        {
            _cellTwitchingIncreasesImpact.gameObject.SetActive(false);
        }
        else
        {
            _cellTwitchingIncreasesImpact.gameObject.SetActive(true);
        }

        if (!RewardsController.Instance.HasReward(RewardId.UnlockModAdditionalPaddle))
        {
            _cellAdditionalPaddle.gameObject.SetActive(false);
        }
        else
        {
            _cellAdditionalPaddle.gameObject.SetActive(true);
        }
    }

    private void Play()
    {
        AudioManager.instance.SetMusicPreset(_selectedMusicPreset);
        LevelManager.instance.LoadLevelFor1PlayerCustomBattle();
    }

    private void SetModChangeBallDirection(bool cond)
    {
        LevelManager.instance.SetModChangeBallDirectionFor1Player(cond);
    }

    private void SetModChangeChangeMusic(bool cond)
    {
        LevelManager.instance.SetModChangeMusicFor1Player(cond);
    }

    private void SetModDangerousBall(bool cond)
    {
        LevelManager.instance.SetModDangerousBallStateFor1Player(cond);
    }

    private void SetModAdditionalPaddle(bool cond)
    {
        LevelManager.instance.SetModAdditionalPaddle1Player(cond);
    }

    private void SetModMakeBallDangerousAfterHit(bool cond)
    {
        LevelManager.instance.SetModMakeBallDangerousAfterHittingFor1Player(cond);
    }

    private void SetModGhostBall(bool cond)
    {
        LevelManager.instance.SetModStarGhostFor1Player(cond);
    }

    private void SetModTwitchesIncreaseImpact(bool cond)
    {
        LevelManager.instance.SetModTwitchesIncreaseImpactFor1Player(cond);
    }
    
    // If toggle for Music Pack 1 is enabled, set the music preset to MusicPack1
    private void SetMusicPack1(bool cond)
    {
        if (cond)
        {
            _selectedMusicPreset = MusicPreset.MusicPack1;
        }
    }

    // If toggle for Music Pack 2 is enabled, set the music preset to MusicPack2
    private void SetMusicPack2(bool cond)
    {
        if (cond)
        {
            _selectedMusicPreset = MusicPreset.MusicPack2;
        }
    }
}
