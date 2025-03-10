using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ParameterReceivedRewards
{
    private readonly string _keyForPlayerPrefs = "ReceivedRewards";
    private List<RewardId> _receivedRewards = new List<RewardId>();

    public ParameterReceivedRewards()
    {
        SetInitialValue();
    }

    private void SetInitialValue()
    {
        if (PlayerPrefs.HasKey(_keyForPlayerPrefs))
        {
            string savedData = PlayerPrefs.GetString(_keyForPlayerPrefs);
            _receivedRewards = savedData
                .Split(',')
                .Where(s => int.TryParse(s, out _))
                .Select(s => (RewardId)int.Parse(s))
                .ToList();
        }
        else
        {
            _receivedRewards.Clear();
        }

        // Debug
        for (int i = 0; i < _receivedRewards.Count; i++)
        {
            Debug.Log($"ParameterReceivedRewards: SetInitialValue: received RewardId = " +
                $"{_receivedRewards[i]}");
        }
    }

    private void SaveRewards()
    {
        string serializedData = string.Join(",", _receivedRewards.Select(r => ((int)r).ToString()));
        Debug.Log($"ParameterReceivedRewards: SaveRewards: serializedData={serializedData}");
        PlayerPrefs.SetString(_keyForPlayerPrefs, serializedData);
        PlayerPrefs.Save();
    }

    public void AddReward(RewardId rewardId)
    {
        if (!_receivedRewards.Contains(rewardId))
        {
            _receivedRewards.Add(rewardId);
            SaveRewards();
        }
    }

    public bool HasReward(RewardId rewardId)
    {
        return _receivedRewards.Contains(rewardId);
    }

    // public void ClearRewards()
    // {
    //     _receivedRewards.Clear();
    //     PlayerPrefs.DeleteKey(_keyForPlayerPrefs);
    // }
}
