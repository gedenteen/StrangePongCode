using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

public class RewardsController : MonoBehaviour
{
    public static RewardsController Instance { get; private set; }
    public static UnityEvent<RewardInfo> EventOnUnlockReward = new UnityEvent<RewardInfo>();

    [Inject] private ConfigOfRewards _configOfRewards;
    private ParameterReceivedRewards _parameterReceivedRewards;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
            return;
        }
        
        _parameterReceivedRewards = new ParameterReceivedRewards();
        EventsManager.levelEndWithStatus.AddListener(OnLevelEnd);
    }

    private void OnDestroy()
    {
        EventsManager.levelEndWithStatus.RemoveListener(OnLevelEnd);
    }

    private void OnLevelEnd(StatusOfLevelEnd status)
    {
        if (LevelManager.instance.playMode != PlayMode.PlayerVsAi_Campaign)
        {
            return;
        }
        if (status != StatusOfLevelEnd.Player1Won)
        {
            return;
        }

        int levelIndex = LevelManager.instance.currentLevel;
        Debug.Log($"RewardsController: OnLevelEnd: levelIndex");

        if (levelIndex >= _configOfRewards.RewardInfos.Count)
        {
            Debug.LogError($"RewardsController: OnLevelEnd: ConfigOfRewards have no data for " +
                $"index={levelIndex}");
            return;
        }

        RewardInfo rewardInfo = _configOfRewards.RewardInfos[levelIndex];
        Debug.Log($"RewardsController: OnLevelEnd: RewardId={rewardInfo.Id}");

        if (!HasReward(rewardInfo.Id))
        {
            _parameterReceivedRewards.AddReward(rewardInfo.Id);
            EventOnUnlockReward?.Invoke(rewardInfo);
        }
    }
    
    public bool HasReward(RewardId rewardId)
    {
        return _parameterReceivedRewards.HasReward(rewardId);
    }

    public void UnlockAllRewards()
    {
        for (int i = 0; i < _configOfRewards.RewardInfos.Count; i++)
        {
            RewardInfo rewardInfo = _configOfRewards.RewardInfos[i];
            Debug.Log($"RewardsController: UnlockAllRewards: RewardId={rewardInfo.Id}");

            if (!HasReward(rewardInfo.Id))
            {
                _parameterReceivedRewards.AddReward(rewardInfo.Id);
                EventOnUnlockReward?.Invoke(rewardInfo);
            }
        }
    }
}
