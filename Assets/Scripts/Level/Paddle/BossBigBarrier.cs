using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Cysharp.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Localization;
using Zenject;

public class BossBigBarrier : Paddle
{
    [Header("References to my objects")]
    [SerializeField] private GameObject _colliderAndSprite;
    [SerializeField] private DialogBox _dialogBox;
    [SerializeField] private AudioSource _audioSource;

    [Header("References to assets")]
    [SerializeField] private List<LocalizedString> _phrasesForWeakHit;
    [SerializeField] private List<LocalizedString> _phrasesForApplyDamage;
    [SerializeField] private AudioClip _audioClipForDeath;
    
    [Inject] private ConfigOfLevels _configOfLevels;

    private float _startScaleX;
    private float _startPositionX;
    private Vector3 _myPosition;

    private float _stepForDecreaseScaleX;
    private float _stepForMovePostionX;

    private int _countOfWeakHits = 0;
    private int _countOfStrongHits = 0;
    private List<int> _countOfWeakHitsForShowPhrase = new List<int> {2, 5, 8, 10, 13}; 

    public void Initialize()
    {
        _startScaleX = _colliderAndSprite.transform.localScale.x;
        _startPositionX = _colliderAndSprite.transform.localPosition.x;
        _myPosition = _colliderAndSprite.transform.position;

        _stepForDecreaseScaleX = _startScaleX / (float)GameManager.instance.scorePlayer2;
        _stepForMovePostionX = _stepForDecreaseScaleX / 2f;

        Debug.Log($"BossBigBarrier: Initialize: _stepForDecreaseScaleX={_stepForDecreaseScaleX} " +
            $"_stepForMovePostionX={_stepForMovePostionX}");
    }

    public void ApplyWeakHit()
    {
        // Display phrase about weak hit
        _countOfWeakHits++;
        if (GameManager.instance.scorePlayer2 > 1 && 
            _countOfWeakHitsForShowPhrase.Contains(_countOfWeakHits))
        {
            int phraseIndex = UnityEngine.Random.Range(0, _phrasesForWeakHit.Count);
            _dialogBox.ShowText(_phrasesForWeakHit[phraseIndex].GetLocalizedString());
        }
    }

    public void ApplyDamage()
    {
        // Change scale
        Vector3 newScale = _colliderAndSprite.transform.localScale;
        newScale.x -= _stepForDecreaseScaleX;
        _colliderAndSprite.transform.localScale = newScale;

        // Change position
        Vector3 newPosition = _colliderAndSprite.transform.localPosition;
        newPosition.x += _stepForMovePostionX;
        _colliderAndSprite.transform.localPosition = newPosition;

        Debug.Log($"BossBigBarrier: ApplyDamage: newScale={newScale} transform.localPosition={transform.localPosition}");

        // Display phrase about string hit
        if (_countOfStrongHits < _phrasesForApplyDamage.Count)
        {
            _dialogBox.MoveX(_stepForMovePostionX * 2);
            _dialogBox.ShowText(_phrasesForApplyDamage[_countOfStrongHits].GetLocalizedString());
        }
        _countOfStrongHits++;
        _countOfWeakHits = 0;
    }

    public void Die()
    {
        EventsManager.disableSoundForBalls.Invoke();
        AudioManager.instance.StopMusic();
        _audioSource.PlayOneShot(_audioClipForDeath);
    }
}
