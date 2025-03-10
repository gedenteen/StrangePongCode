using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteamIntergration : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        try
        {
            Steamworks.SteamClient.Init(2689460);
            PrintPlayerNickname();
            //PrintFriends();
        }
        catch (Exception ex)
        {
            Debug.LogError($"SteamIntergration: Start: exception message: {ex.Message}");
        }
    }

    private void Update()
    {
        Steamworks.SteamClient.RunCallbacks();
    }

    private void OnApplicationQuit()
    {
        Steamworks.SteamClient.Shutdown();
        Debug.Log($"SteamIntergration: OnApplicationQuit: end");
    }

    private void PrintPlayerNickname()
    {
        Debug.Log($"SteamIntergration: PrintPlayerNickname: {Steamworks.SteamClient.Name}");
    }

    private void PrintFriends()
    {
        foreach (var friend in Steamworks.SteamFriends.GetFriends())
        {
            Debug.Log($"SteamIntergration: PrintFriends: friend's name: {friend.Name}");
        }
    }

    public void UnlockAchievement(string id)
    {
        var achievement = new Steamworks.Data.Achievement(id);
    
        Debug.Log($"SteamIntergration: UnlockAchievement: achievement.State={achievement.State}");

        if (!achievement.State)
        {
            achievement.Trigger();
        }
    }

    public void ClearAchievement(string id)
    {
        var achievement = new Steamworks.Data.Achievement(id);
        achievement.Clear();

        Debug.Log($"SteamIntergration: ClearAchievement: achievement.State={achievement.State}");
    }
}
