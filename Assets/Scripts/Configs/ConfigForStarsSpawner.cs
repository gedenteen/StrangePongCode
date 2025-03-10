using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ConfigForStarsSpawner", menuName = "ScriptableObjects/Create ConfigForStarsSpawner asset")]
public class ConfigForStarsSpawner : ScriptableObject
{
    #region Private serialized fields

    [Header("Common")]
    [SerializeField] private float _timeToAppearForAnyTrigger = 1f;
    [SerializeField] private Color _colorForAppearForAnyTrigger = Color.white;
    [SerializeField] private bool _playSoundForReadyStarTrigger = false;

    [Header("Star Trigger Change direction of ball")]
    [SerializeField] private Sprite _spriteForTriggerChangeDirection;
    [SerializeField] private Color _colorForTriggerChangeDirection = Color.white;
    [SerializeField] private float _timeForSpawnStarsDirection = 9f;

    [Header("Star Trigger Change music")]
    [SerializeField] private Sprite _spriteForTriggerMusic;
    [SerializeField] private Color _colorForTriggerMusic = Color.white;
    [SerializeField] private float _timeForSpawnStarsMusic = 10f;

    [Header("Star Trigger Make ball dangerous")]
    [SerializeField] private Sprite _spriteForTriggerDangerousBall;
    [SerializeField] private Color _colorForTriggerDangerousBall = Color.white;
    [SerializeField] private float _timeForSpawnStarsDangerousBall = 15f;

    [Header("Star Trigger Create ghost ball")]
    [SerializeField] private Sprite _spriteForTriggerGhost;
    [SerializeField] private Color _colorForTriggerGhost = Color.white;
    [SerializeField] private float _timeForSpawnStarsGhost = 11f;

    #endregion

    #region Public read-only properties

    // Common
    public float TimeToAppearForAnyTrigger => _timeToAppearForAnyTrigger;
    public Color ColorForAppearForAnyTrigger => _colorForAppearForAnyTrigger;
    public bool PlaySound => _playSoundForReadyStarTrigger;

    // Star Trigger Change direction of ball
    public Sprite SpriteForTriggerChangeDirection => _spriteForTriggerChangeDirection;
    public Color ColorForTriggerChangeDirection => _colorForTriggerChangeDirection;
    public float TimeForSpawnStarsDirection => _timeForSpawnStarsDirection;

    // Star Trigger Change music
    public Sprite SpriteForTriggerMusic => _spriteForTriggerMusic;
    public Color ColorForTriggerMusic => _colorForTriggerMusic;
    public float TimeForSpawnStarsMusic => _timeForSpawnStarsMusic;

    // Star Trigger Make ball dangerous
    public Sprite SpriteForTriggerDangerousBall => _spriteForTriggerDangerousBall;
    public Color ColorForTriggerDangerousBall => _colorForTriggerDangerousBall;
    public float TimeForSpawnStarsDangerousBall => _timeForSpawnStarsDangerousBall;

    // Star Trigger Create ghost ball
    public Sprite SpriteForTriggerGhost => _spriteForTriggerGhost;
    public Color ColorForTriggerGhost => _colorForTriggerGhost;
    public float TimeForSpawnStarsGhost => _timeForSpawnStarsGhost;

    #endregion
}
