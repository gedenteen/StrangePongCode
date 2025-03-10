using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ConfigOfLevelModifiers", menuName = "ScriptableObjects/Create ConfigOfLevelModifiers asset")]
public class ConfigOfLevels : ScriptableObject
{
    [field: SerializeField] public List<SettingsOfLevel> Levels { get; private set; }

    [Header("Other parameters")]
    [SerializeField] private float _minSpeedForHitBigBarrier = 18f;
    public float MinSpeedForHitBigBarrier => _minSpeedForHitBigBarrier;

    [SerializeField] private float _valueOfForceSetChromaticAbberration = 0.4f;
    public float ValueOfForceSetChromaticAbberration => _valueOfForceSetChromaticAbberration;

    [SerializeField] private float _valueOfForceSetBloom = 0.4f;
    public float ValueOfForceSetBloom => _valueOfForceSetBloom;
}

[Serializable]
public class SettingsOfLevel
{
    [field: SerializeField] public List<LevelModifier> LevelModifiers { get; private set; }
    [field: SerializeField] public int HpOfLeftParticipant { get; private set; } = 5;
    [field: SerializeField] public int HpOfRightParticipant { get; private set; } = 5;
    [field: SerializeField] public MusicPreset MusicPreset { get; private set; } = MusicPreset.Undefined;
}
