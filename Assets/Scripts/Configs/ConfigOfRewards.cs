using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "ConfigOfRewards", menuName = "ScriptableObjects/Create ConfigOfRewards asset")]
public class ConfigOfRewards : ScriptableObject
{
    [field: SerializeField] public List<RewardInfo> RewardInfos { get; private set; }
}

[Serializable]
public class RewardInfo
{
    [field: SerializeField] public RewardId Id { get; private set; }
    [field: SerializeField] public Sprite SpriteIcon { get; private set; }
    [field: SerializeField] public Color ColorOfIcon { get; private set; }
    [field: SerializeField] public LocalizedString LocalizedString { get; private set; }
}
