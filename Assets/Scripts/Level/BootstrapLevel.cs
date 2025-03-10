using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This is a class that sets the correct order of script initialization at the level. Events may not work correctly without the correct order.
public class BootstrapLevel : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Ball ball;
    [SerializeField] private MainPaddle leftMainPaddle;
    [SerializeField] private MainPaddle rightMainPaddle;
    [SerializeField] private BossBigBase bossBigBase;
    [SerializeField] private BossBigBarrier bossBigBarrier;

    private void Awake()
    {
        gameManager.Initialize();
        ball.Initialize();
        if (leftMainPaddle.gameObject.activeSelf) leftMainPaddle.Initialize();
        if (rightMainPaddle.gameObject.activeSelf) rightMainPaddle.Initialize();
        if (bossBigBase.gameObject.activeSelf) bossBigBase.Initialize();
        if (bossBigBarrier.gameObject.activeSelf) bossBigBarrier.Initialize();
    }
}
