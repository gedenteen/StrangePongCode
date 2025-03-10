using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardsDisplayer : MonoBehaviour
{
    [SerializeField] private GameObject _containerForPanels;
    [SerializeField] private PanelForReward _prefabPanelForReward;

    private void Awake()
    {
        RewardsController.EventOnUnlockReward.AddListener(OnUnlockReward);
    }

    private void OnDestroy()
    {
        RewardsController.EventOnUnlockReward.RemoveListener(OnUnlockReward);
    }

    private void OnUnlockReward(RewardInfo rewardInfo)
    {
        Debug.Log($"RewardsDisplayer: OnUnlockReward: RewardId={rewardInfo.Id}");
        PanelForReward panelForReward = Instantiate(_prefabPanelForReward, 
            _containerForPanels.transform);
        panelForReward.DisplayReward(rewardInfo);
    }
}
