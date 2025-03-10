using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Refs to my objects")]
    [SerializeField] private AudioSource _audioSourceSounds;
    [SerializeField] private MusicController _musicController;

    [Header("Refs to music")]
    [SerializeField] private AudioClip _mainMenuMusic;
    [SerializeField] private List<AudioClip> _listOfTracksPack1 = new List<AudioClip>();
    [SerializeField] private List<AudioClip> _listOfTracksPack2 = new List<AudioClip>();
    [SerializeField] private AudioClip _bossBigBaseMusic;
    [SerializeField] private AudioClip _bossBigBarrierMusic;
    [SerializeField] private AudioClip _endMusic;

    [Header("Refs to sounds")]
    [SerializeField] private AudioClip _scoreSound;
    [SerializeField] private AudioClip _winSound;
    [SerializeField] private AudioClip _looseSound;
    [SerializeField] private AudioClip _starTriggerIsReadySound;
    
    [Header("Public fields")]
    public bool CanPlaySounds = false;

    // Private fields
    private System.Random _random = new System.Random();
    private Dictionary<AudioClip, int> _dictPlayedTracksPack1 = new Dictionary<AudioClip, int>();
    private Dictionary<AudioClip, int> _dictPlayedTracksPack2 = new Dictionary<AudioClip, int>();
    private MusicPreset _currentMusicPreset = MusicPreset.MainMenu;

    private void Awake()
    {
        instance = this;

        // Fill the dictionaries
        foreach (AudioClip track in _listOfTracksPack1)
        {
            _dictPlayedTracksPack1[track] = 0;
        }
        foreach (AudioClip track in _listOfTracksPack2)
        {
            _dictPlayedTracksPack2[track] = 0;
        }
    }

    private void OnEnable()
    {
        EventsManager.levelLoaded.AddListener(NewLevelIsLoaded);
        EventsManager.changeMusicTrack.AddListener(PlayRandomTrack);
        EventsManager.levelEnd.AddListener(StopMusic);
    }

    private void OnDisable()
    {
        EventsManager.levelLoaded.RemoveListener(NewLevelIsLoaded);
        EventsManager.changeMusicTrack.RemoveListener(PlayRandomTrack);
        EventsManager.levelEnd.RemoveListener(StopMusic);
    }

    private void Start()
    {
        _musicController.PlayMusic(_mainMenuMusic);
    }

    private void NewLevelIsLoaded(int levelNumber)
    {
        CanPlaySounds = true;
    }

    private void PlayMusicPack1()
    {
        if (_listOfTracksPack1.Count == 0)
        {
            Debug.LogError("AudioManager: PlayMusicPack1: список треков пуст!");
            return;
        }

        AudioClip track = _listOfTracksPack1[0];
        _musicController.PlayMusic(track);
        _dictPlayedTracksPack1[track]++;
    }

    private void PlayMusicPack2()
    {
        if (_listOfTracksPack2.Count == 0)
        {
            Debug.LogError("AudioManager: PlayMusicPack2: список треков пуст!");
            return;
        }

        AudioClip track = _listOfTracksPack2[0];
        _musicController.PlayMusic(track);
        _dictPlayedTracksPack2[track]++;
    }

    private void PlayRandomMusicFromPack(Dictionary<AudioClip, int> dictPlayedTracks, List<AudioClip> listOfTracks)
    {
        if (listOfTracks == null || listOfTracks.Count == 0)
        {
            Debug.LogError("AudioManager: PlayRandomMusicFromPack: список треков пуст!");
            return;
        }

        // Получаем минимальное количество воспроизведений
        int minCount = dictPlayedTracks.Values.Min();

        // Фильтруем треки с минимальным количеством воспроизведений
        List<AudioClip> minTracks = dictPlayedTracks
            .Where(pair => pair.Value == minCount)
            .Select(pair => pair.Key)
            .ToList();

        if (minTracks.Count == 0)
        {
            Debug.LogError("AudioManager: PlayRandomMusicFromPack: не найдено треков с минимальным количеством воспроизведений!");
            return;
        }

        // Выбираем случайный трек из списка
        int index = minTracks.Count == 1 ? 0 : _random.Next(0, minTracks.Count);
        // Если трек всего один, прибавляем 2, чтобы избежать повтора, иначе прибавляем 1
        dictPlayedTracks[minTracks[index]] += minTracks.Count == 1 ? 2 : 1;

        // Проигрываем выбранный трек
        _musicController.PlayMusic(minTracks[index]);

        // Debug
        Debug.Log($"AudioManager: PlayRandomMusicFromPack: index={index}");
        foreach (var item in dictPlayedTracks)
        {
            Debug.Log($"AudioManager: PlayRandomMusicFromPack: dict.Value={item.Value} dict.Key={item.Key}");
        }
    }

    private void PlayRandomTrack()
    {
        switch (_currentMusicPreset)
        {
            case MusicPreset.MusicPack1:
                PlayRandomMusicFromPack(_dictPlayedTracksPack1, _listOfTracksPack1);
                break;
            case MusicPreset.MusicPack2:
                PlayRandomMusicFromPack(_dictPlayedTracksPack2, _listOfTracksPack2);
                break;
            default:
                Debug.LogError($"AudioManager: PlayRandomTrack: unexpected preset=" +
                    $"{_currentMusicPreset}");
                return;
        }
    }

    #region PlayMusic

    public void PlayMainMenuMusic()
    {
        _musicController.PlayMusic(_mainMenuMusic);
        CanPlaySounds = false;
    }

    public void StopMusic()
    {
        _musicController.Stopimmediately();
    }

    public void SetMusicForBossBigBase()
    {
        _musicController.PlayMusic(_bossBigBaseMusic);
    }

    public void SetMusicForBossBigBarrier()
    {
        _musicController.PlayMusic(_bossBigBarrierMusic);
    }

    public void SetEndMusicAndPlay()
    {
        _musicController.PlayMusic(_endMusic);
        Debug.Log("AudioManager: SetEndMusicAndPlay: end");
    }

    public void SetMusicPreset(MusicPreset newPreset)
    {
        switch (newPreset)
        {
            case MusicPreset.MainMenu:
                PlayMainMenuMusic();
                break;
            case MusicPreset.MusicPack1:
                PlayMusicPack1();
                break;
            case MusicPreset.BossBigBase:
                SetMusicForBossBigBase();
                break;
            case MusicPreset.MusicPack2:
                PlayMusicPack2();
                break;
            case MusicPreset.BossBigBarrier:
                SetMusicForBossBigBarrier();
                break;
            default:
                Debug.LogError($"AudioManager: SetMusicPreset: unexpected preset={newPreset}");
                return;
        }

        _currentMusicPreset = newPreset;
    }

    public void PlayCurrentMusicAgain()
    {
        SetMusicPreset(_currentMusicPreset);
    }

    #endregion

    #region PlaySounds

    public void PlayScoreSound()
    {
        _audioSourceSounds.PlayOneShot(_scoreSound);
    }

    public void PlayWinSound()
    {
        _audioSourceSounds.PlayOneShot(_winSound);
    }

    public void PlayLooseSound()
    {
        _audioSourceSounds.PlayOneShot(_looseSound);
    }

    public void PlayCreationOfStarTriggerSound()
    {
        _audioSourceSounds.PlayOneShot(_starTriggerIsReadySound);
    }

    #endregion
}
