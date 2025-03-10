using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PanelForReward : MonoBehaviour
{
    [SerializeField] private Image _imageForIcon;
    [SerializeField] private TextMeshProUGUI _textDescription;

    public void DisplayReward(RewardInfo rewardInfo)
    {
        Debug.Log($"PanelForReward: DisplayReward: RewardId={rewardInfo.Id}");

        if (rewardInfo.SpriteIcon != null)
        {
            _imageForIcon.sprite = rewardInfo.SpriteIcon;
            _imageForIcon.color = rewardInfo.ColorOfIcon;
            _imageForIcon.gameObject.SetActive(true);
        }
        else
        {
            _imageForIcon.gameObject.SetActive(false);
        }

        _textDescription.text = rewardInfo.LocalizedString.GetLocalizedString();
    }
}
