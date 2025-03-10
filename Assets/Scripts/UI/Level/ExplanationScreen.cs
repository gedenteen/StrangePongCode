using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;

public class ExplanationScreen : MonoBehaviour
{
    [Header("References on scene")]
    [SerializeField] private GameObject bgImage;
    [SerializeField] private TextMeshProUGUI mainText;
    [SerializeField] private GameObject cellPlayerRight;
    [SerializeField] private GameObject cellTriggerDirection;
    [SerializeField] private GameObject cellTriggerMusic;
    [SerializeField] private GameObject cellTriggerBallState;
    [SerializeField] private GameObject cellChromaticAberration;
    [SerializeField] private GameObject cellAdditionalPaddle;
    [SerializeField] private GameObject cellMakeBallDangerousAfterHitting;
    [SerializeField] private GameObject cellBossBigBase;
    [SerializeField] private GameObject cellGhost;
    [SerializeField] private GameObject cellTwitching;
    [SerializeField] private GameObject cellBloom;
    [SerializeField] private GameObject cellBossBigBarrier;

    [Header("Localization")]
    [SerializeField] private LocalizedString lsPlayerVsPlayer;
    [SerializeField] private LocalizedString lsLevel;

    private void Start()
    {
        ShowInfo();
    }

    private void ShowInfo()
    {
        // Handle play mode
        if (LevelManager.instance.playMode == PlayMode.AiVsAi)
        {
            EventsManager.levelStart.Invoke();
            return;
        }

        if (LevelManager.instance.playMode == PlayMode.PlayerVsPlayer)
        {
            mainText.text = lsPlayerVsPlayer.GetLocalizedString();
        }
        else if (LevelManager.instance.playMode == PlayMode.PlayerVsAi_Campaign)
        {
            mainText.text = String.Concat(lsLevel.GetLocalizedString(), " ", (LevelManager.instance.currentLevel + 1).ToString());
            cellPlayerRight.SetActive(false);
        }
        else if (LevelManager.instance.playMode == PlayMode.PlayerVsAi_CustomBattle)
        {
            mainText.text = "Custom battle";
            cellPlayerRight.SetActive(false);
        }

        // Handle level modifiers
        var levelModifiers = LevelManager.instance.GetLevelModifiers();

        if (!levelModifiers.ContainsKey(LevelModifier.StarTriggerChangeBallDirection) || levelModifiers[LevelModifier.StarTriggerChangeBallDirection] == 0)
        {
            cellTriggerDirection.SetActive(false);
        }

        if (!levelModifiers.ContainsKey(LevelModifier.StarTriggerChangeMusic) || levelModifiers[LevelModifier.StarTriggerChangeMusic] == 0)
        {
            cellTriggerMusic.SetActive(false);
        }

        if (!levelModifiers.ContainsKey(LevelModifier.StarTriggerMakeBallDangerous) || levelModifiers[LevelModifier.StarTriggerMakeBallDangerous] == 0)
        {
            cellTriggerBallState.SetActive(false);
        }

        if (!levelModifiers.ContainsKey(LevelModifier.ForceSetChromaticAberration) || levelModifiers[LevelModifier.ForceSetChromaticAberration] == 0)
        {
            cellChromaticAberration.SetActive(false);
        }

        if (!levelModifiers.ContainsKey(LevelModifier.AdditionalPaddle) || levelModifiers[LevelModifier.AdditionalPaddle] == 0)
        {
            cellAdditionalPaddle.SetActive(false);
        }

        if (!levelModifiers.ContainsKey(LevelModifier.MakeBallDangerousAfterHitting) || levelModifiers[LevelModifier.MakeBallDangerousAfterHitting] == 0)
        {
            cellMakeBallDangerousAfterHitting.SetActive(false);
        }

        if (!levelModifiers.ContainsKey(LevelModifier.BossBigBase) || levelModifiers[LevelModifier.BossBigBase] == 0)
        {
            cellBossBigBase.SetActive(false);
        }

        if (!levelModifiers.ContainsKey(LevelModifier.StarTriggerGhost) || levelModifiers[LevelModifier.StarTriggerGhost] == 0)
        {
            cellGhost.SetActive(false);
        }

        if (!levelModifiers.ContainsKey(LevelModifier.TwitchingIncreasesImpactOnBall) || levelModifiers[LevelModifier.TwitchingIncreasesImpactOnBall] == 0)
        {
            cellTwitching.SetActive(false);
        }

        if (!levelModifiers.ContainsKey(LevelModifier.ForceSetBloom) || levelModifiers[LevelModifier.ForceSetBloom] == 0)
        {
            cellBloom.SetActive(false);
        }

        if (!levelModifiers.ContainsKey(LevelModifier.BossBigBarrier) || levelModifiers[LevelModifier.BossBigBarrier] == 0)
        {
            cellBossBigBarrier.SetActive(false);
        }

        // Show screen and wait for any key
        bgImage.SetActive(true);
    }

    public void StartLevel()
    {
        Debug.Log("ExplanationScreen: StartLevel: begin");
        bgImage.SetActive(false);
        EventsManager.levelStart.Invoke();
    }
}
