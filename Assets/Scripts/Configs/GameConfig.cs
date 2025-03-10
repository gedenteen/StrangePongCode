using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig", menuName = "ScriptableObjects/Create GameConfig asset")]
public class GameConfig : ScriptableObject
{
    [field: SerializeField] public string DiscordServerUrl { get; private set; }
    [field: SerializeField] public string PhraseForUnlockAll { get; private set; }
    [field: SerializeField] public bool AddSteamSevices { get; private set; }
}
