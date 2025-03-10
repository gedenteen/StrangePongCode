using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class MusicController : MonoBehaviour
{
    [SerializeField]
    private List<AudioSource> _audioSources;

    [SerializeField, Range(0f, 5f)]
    private float _fadeDuration = 1f; // Длительность фейда

    private int _currentSourceIndex = 0;
    private bool _isFading = false;

    private void OnValidate()
    {
        if (_audioSources == null || _audioSources.Count == 0)
        {
            AudioSource[] sources = GetComponents<AudioSource>();
            _audioSources = new List<AudioSource>(sources);
        }
    }

    private void Awake()
    {
        if (_audioSources == null || _audioSources.Count < 2)
        {
            Debug.LogError("MusicController: Please assign at least two AudioSources in the inspector.");
        }
    }

    public void PlayMusic(AudioClip clip)
    {
        if (_audioSources == null || _audioSources.Count < 2)
        {
            Debug.LogError("MusicController: AudioSources list is not properly initialized.");
            return;
        }

        if (_isFading)
        {
            Debug.LogWarning("MusicController: Fade operation already in progress. Ignoring new request.");
            return;
        }

        //Debug.Log($"MusicController: PlayMusic: clip.name={clip.name}");

        // Запустить смену музыки
        FadeToNextTrack(clip).Forget();
    }

    public void Stopimmediately()
    {
        AudioSource currentSource = _audioSources[_currentSourceIndex];
        currentSource.Stop();
    }

    private async UniTask FadeToNextTrack(AudioClip newClip)
    {
        _isFading = true;

        // Определить текущий и следующий источники звука
        AudioSource currentSource = _audioSources[_currentSourceIndex];
        int nextSourceIndex = (_currentSourceIndex + 1) % _audioSources.Count;
        AudioSource nextSource = _audioSources[nextSourceIndex];
        //Debug.Log($"MusicController: FadeToNextTrack: nextSourceIndex={nextSourceIndex}");

        // Настроить следующий источник
        nextSource.clip = newClip;
        nextSource.volume = 0f;
        nextSource.Play();

        // Плавно уменьшить громкость текущего источника и увеличить громкость следующего
        float timer = 0f;
        while (timer < _fadeDuration)
        {
            float t = timer / _fadeDuration;
            currentSource.volume = Mathf.Lerp(1f, 0f, t);
            nextSource.volume = Mathf.Lerp(0f, 1f, t);

            timer += Time.unscaledDeltaTime;
            await UniTask.Yield(PlayerLoopTiming.Update);
        }

        // Завершить фейд
        currentSource.volume = 0f;
        currentSource.Stop();
        nextSource.volume = 1f;
        _currentSourceIndex = nextSourceIndex;

        _isFading = false;
    }
}
