using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CanvasForTwitches : MonoBehaviour
{
    [SerializeField] private BallAccelerator _ballAcceleratorOfLeftPaddle;
    [SerializeField] private BallAccelerator _ballAcceleratorOfRightPaddle;
    [SerializeField] private TextMeshProUGUI _textLeft;
    [SerializeField] private TextMeshProUGUI _textRight;

    private uint _countOfTwitchesOfLeftPaddle = 0;
    private uint _countOfTwitchesOfRightPaddle = 0;

    private void Awake()
    {
        var levelModifiers = LevelManager.instance.GetLevelModifiers();

        if (levelModifiers.ContainsKey(LevelModifier.TwitchingIncreasesImpactOnBall)
            && levelModifiers[LevelModifier.TwitchingIncreasesImpactOnBall] > 0)
        {
            _ballAcceleratorOfLeftPaddle.EventOnIncreaseSpeedSummand.AddListener
                (AddTwitchForLeftPaddle);
            _ballAcceleratorOfRightPaddle.EventOnIncreaseSpeedSummand.AddListener
                (AddTwitchForRightPaddle);
            _ballAcceleratorOfLeftPaddle.EventOnResetSpeedSummand.AddListener
                (ResetTwitchesOfLeftPaddle);
            _ballAcceleratorOfRightPaddle.EventOnResetSpeedSummand.AddListener
                (ResetTwitchesOfRightPaddle);
        }
        else
        {
            gameObject.SetActive(false);
            return;
        }
    }

    private void AddTwitchForLeftPaddle(uint count)
    {
        _countOfTwitchesOfLeftPaddle += count;
        _textLeft.text = string.Concat("x", _countOfTwitchesOfLeftPaddle);
    }

    private void AddTwitchForRightPaddle(uint count)
    {
        _countOfTwitchesOfRightPaddle += count;
        _textRight.text = string.Concat("x", _countOfTwitchesOfRightPaddle);
    }

    private void ResetTwitchesOfLeftPaddle()
    {
        _countOfTwitchesOfLeftPaddle = 0;
        _textLeft.text = string.Concat("x", _countOfTwitchesOfLeftPaddle);
    }

    private void ResetTwitchesOfRightPaddle()
    {
        _countOfTwitchesOfRightPaddle = 0;
        _textRight.text = string.Concat("x", _countOfTwitchesOfRightPaddle);
    }
}
