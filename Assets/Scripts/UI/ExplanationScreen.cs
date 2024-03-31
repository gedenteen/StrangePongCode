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
    [SerializeField] private GameObject cellAdditionalPaddle;
    [SerializeField] private GameObject cellMakeBallDangerousAfterHitting;
    [SerializeField] private GameObject cellBossBigBase;
    [SerializeField] private GameObject cellGhost;

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
        else if (LevelManager.instance.playMode == PlayMode.PlayerVsAi)
        {
            mainText.text = String.Concat(lsLevel.GetLocalizedString(), " ", LevelManager.instance.currentLevel.ToString());
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

        if (!levelModifiers.ContainsKey(LevelModifier.StarTriggerChangeBallState) || levelModifiers[LevelModifier.StarTriggerChangeBallState] == 0)
        {
            cellTriggerBallState.SetActive(false);
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

        // Show screen and wait for any key
        bgImage.SetActive(true);
        StartCoroutine(WaitForAnyKey());
    }

    private IEnumerator WaitForAnyKey()
    {
        while (!Input.anyKeyDown)
        {
            yield return null;
        }

        bgImage.SetActive(false);
        Debug.Log("ExplanationScreen: WaitForAnyKey(): key is pressed");

        EventsManager.levelStart.Invoke();
    }
}
