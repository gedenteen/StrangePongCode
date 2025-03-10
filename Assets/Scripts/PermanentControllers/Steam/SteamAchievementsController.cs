using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class SteamAchievementsController : MonoBehaviour
{
    [SerializeField] private SteamIntergration _steamIntegration;

    private const string _achieventIdBossBigBase = "ACH_BOSS_BIG_BASE";
    private const string _achieventIdBossBigBarrier = "ACH_BOSS_BIG_BARRIER";

    private void Awake()
    {
        EventsManager.levelEndWithStatus.AddListener(OnLevelEnd);
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

        Dictionary<LevelModifier, byte> levelMods = LevelManager.instance.GetLevelModifiers();
        
        if (levelMods.ContainsKey(LevelModifier.BossBigBase))
        {
            _steamIntegration.UnlockAchievement(_achieventIdBossBigBase);
        }
        if (levelMods.ContainsKey(LevelModifier.BossBigBarrier))
        {
            _steamIntegration.UnlockAchievement(_achieventIdBossBigBarrier);
        }
    }

    [Button]
    private void ClearAchievementBossBigBase()
    {
        _steamIntegration.ClearAchievement(_achieventIdBossBigBase);
    }

    [Button]
    private void ClearAchievementBossBigBarrier()
    {
        _steamIntegration.ClearAchievement(_achieventIdBossBigBarrier);
    }
}
