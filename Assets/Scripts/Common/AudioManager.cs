using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Refs to Audio Sources")]
    public AudioSource audioSourceSounds;
    public AudioSource audioSourceMusic;

    [Header("Refs to music")]
    public AudioClip mainMenuMusic;
    public List<AudioClip> listTracks = new List<AudioClip>();
    public AudioClip bossBigBaseMusic;
    public AudioClip endMusic;

    [Header("Refs to sounds")]
    public AudioClip scoreSound;
    public AudioClip winSound;
    public AudioClip looseSound;

    // Public fields
    [SerializeField] public bool canPlaySounds = false; //can't play sounds until loading level PlayerVsAi or PlayerVsPlayer
    public Action<float> onSoundsVolumeChange;

    // Private fields
    private System.Random random = new System.Random();
    private Dictionary<AudioClip, int> dictPlayedTracks = new Dictionary<AudioClip, int>();
    private bool specialTrackIsSet = false;

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

        // Fill the dictionary
        foreach (AudioClip track in listTracks)
        {
            dictPlayedTracks.Add(track, 0);
        }

        EventsManager.levelLoaded.AddListener(NewLevelIsLoaded);
        EventsManager.changeMusicTrack.AddListener(PlayRandomTrack);
        EventsManager.levelEnd.AddListener(StopMusic);
    }

    private void NewLevelIsLoaded(int levelNumber)
    {
        //Debug.Log($"AudioManager: NewLevelIsLoaded(): {levelNumber}");

        // If special track is not set, then play default track (index 0)
        if (!specialTrackIsSet)
        {
            audioSourceMusic.clip = listTracks[0];
            dictPlayedTracks[listTracks[0]]++;
        }
        audioSourceMusic.Play();

        canPlaySounds = true;
    }

    private void PlayRandomTrack()
    {
        // Determine minimal count of playing a track
        int minCountOfPlays = dictPlayedTracks[listTracks[0]];
        foreach (var item in dictPlayedTracks)
        {
            if (item.Value < minCountOfPlays)
            {
                minCountOfPlays = item.Value;
            }
        }

        // List of tracks with the least number of plays
        List<AudioClip> listMinTracks = new List<AudioClip>();
        foreach (var item in dictPlayedTracks)
        {
            if (item.Value == minCountOfPlays)
            {
                listMinTracks.Add(item.Key);
            }
        }

        // Determine index of track
        int index;
        if (listMinTracks.Count <= 0)
        {
            Debug.LogError($"AudioManager: PlayRandomTrack: listMinTracks.Count <= 0, how?!?");
            return;
        }
        if (listMinTracks.Count == 1)
        {
            index = 0;
            dictPlayedTracks[listMinTracks[index]] += 2; //2, not 1, so that the next track does not repeat
        }
        else
        {
            index = random.Next(0, listMinTracks.Count);
            dictPlayedTracks[listMinTracks[index]] += 1;
        }

        // Play track
        audioSourceMusic.clip = listMinTracks[index];
        audioSourceMusic.Play();

        // Debug
        // Debug.Log($"AudioManager: PlayRandomTrack: index={index}");
        // foreach (var item in dictPlayedTracks)
        // {
        //     Debug.Log($"AudioManager: PlayRandomTrack: dict.Value={item.Value} dict.Key={item.Key}");
        // }
    }

    #region PlayMusic
    public void PlayMainMenuMusic()
    {
        specialTrackIsSet = false;
        audioSourceMusic.clip = mainMenuMusic;
        audioSourceMusic.Play();
        canPlaySounds = false;
    }

    public void StopMusic()
    {
        audioSourceMusic.Stop();
        specialTrackIsSet = false;
    }

    public void SetMusicForBossBigBase()
    {
        audioSourceMusic.clip = bossBigBaseMusic;
        specialTrackIsSet = true;
    }

    public void SetEndMusicAndPlay()
    {
        audioSourceMusic.clip = endMusic;
        audioSourceMusic.Play();
        Debug.Log("AudioManager: SetEndMusicAndPlay: end");
    }
    #endregion

    #region PlaySounds
    public void PlayScoreSound()
    {
        audioSourceSounds.PlayOneShot(scoreSound);
    }
    public void PlayWinSound()
    {
        audioSourceSounds.PlayOneShot(winSound);
    }
    public void PlayLooseSound()
    {
        audioSourceSounds.PlayOneShot(looseSound);
    }
    #endregion

    public void ChangeVolumeMusic(float volume)
    {
        audioSourceMusic.volume = volume;
    }
    public void ChangeVolumeSounds(float volume)
    {
        audioSourceSounds.volume = volume;
        onSoundsVolumeChange?.Invoke(volume);
    }
}
