using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BallAccelerator : MonoBehaviour
{
    public UnityEvent<uint> EventOnIncreaseSpeedSummand = new UnityEvent<uint>();
    public UnityEvent EventOnResetSpeedSummand = new UnityEvent();

    [Header("Changeable parameters")]
    [SerializeField] private float _defaultSpeedSummand = 0.9f;
    [SerializeField] private float _additionalSpeedSummand = 0.1f;

    [Header("References")]
    [SerializeField] private Paddle _myPaddle;
    [SerializeField] private AudioSource _myAudioSource;

    private float _accumulatedSpeedSummand;

    private void OnValidate()
    {
        _myAudioSource ??= GetComponent<AudioSource>();
    }

    private void Awake()
    {
        EventsManager.resetSpeedSummandForBallAccelerator.AddListener(CheckForResetSpeedSummand);
    }

    private void OnDestroy()
    {
        EventsManager.resetSpeedSummandForBallAccelerator.RemoveListener(CheckForResetSpeedSummand);
    }

    public float GetSpeedSummand()
    {
        Debug.Log($"BallAccelerator: GetSpeedSummand: " +
            $"_accumulatedSpeedSummand={_accumulatedSpeedSummand}");
        return _accumulatedSpeedSummand;
    }

    public void IncreaseSpeedSummand(uint count)
    {   
        // Debug.Log($"BallAccelerator: IncreaseSpeedSummand: count={count}");

        for (int i = 0; i < count; i++)
        {
            _accumulatedSpeedSummand += _additionalSpeedSummand;
        }
        
        _myAudioSource.pitch = Random.Range(0.9f, 1.1f);
        _myAudioSource.Play();

        EventOnIncreaseSpeedSummand?.Invoke(count);
    }

    public void CheckForResetSpeedSummand(int id)
    {
        if (id == _myPaddle.id)
        {
            _accumulatedSpeedSummand = _defaultSpeedSummand;
            EventOnResetSpeedSummand?.Invoke();
            Debug.Log($"BallAccelerator: CheckForResetSpeedSummand: reseted, id={id}");
        }
    }
}
