using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    public static DifficultyManager instance;
    public int playerSkill /*{ get; private set; }*/ = 10; // The higher this value, the higher the skill of the player

    public readonly int minPlayerSkill = 1; // Value for the worst player
    public readonly int maxPlayerSkill = 15; // Value for the best player 

    // Strings for PlayerPrefs
    private readonly string pp_playerSkill = "PlayerSkill";

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (PlayerPrefs.HasKey(pp_playerSkill))
        {
            playerSkill = PlayerPrefs.GetInt(pp_playerSkill);
            Debug.Log($"DifficultyManager: Awake(): found saved player skill = {playerSkill}");
        }
        else
        {
            Debug.Log($"DifficultyManager: Awake(): saved player skill (equals {playerSkill} now) is not found");
        }
    }

    public float GetRelativeSkillOfPlayer()
    {
        return (float)(playerSkill - minPlayerSkill) / (maxPlayerSkill - minPlayerSkill);
        // This method should return value float between 0 and 1.
        // If playerSkill is minimum, then value = 0.
        // If playerSkill in the middle between minimum and maximum, then value = 0.5.
        // If playerSkill is maximum, then value = 1.
    }

    public void ChangePlayerSkillValue(bool playerWon)
    {
        if (playerWon)
        {
            playerSkill = Math.Min(playerSkill + 1, maxPlayerSkill);
        }
        else
        {
            playerSkill = Math.Max(playerSkill - 1, minPlayerSkill);
        }

        Debug.Log($"DifficultyManager: ChangePlayerSkillValue(): new playerSkill={playerSkill}");

        PlayerPrefs.SetInt(pp_playerSkill, playerSkill);
    }
}
