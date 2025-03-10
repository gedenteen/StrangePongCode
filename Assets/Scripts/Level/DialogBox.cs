using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

public class DialogBox : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] private int _charDelay = 50;
    [SerializeField] private int _delayBeforeHide = 1000;

    [Header("References")]
    [SerializeField] private GameObject _dialogBoxObject;
    [SerializeField] private TextMeshProUGUI _textMesh;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip[] _typeSounds;
    
    private CancellationTokenSource _cts;

    private static readonly char[] _ignoredCharacters = 
        { ' ', '.', ',', '!', '?', ':', ';', '"', '\\' };

    private void OnDestroy()
    {
        _cts?.Cancel();
    }

    public void ShowText(string text)
    {
        // Cancel the previous execution if it's still running
        _cts?.Cancel();
        _cts = new CancellationTokenSource();
        
        _ = TypeTextAsync(text, _cts.Token);
    }

    public void MoveX(float offsetX)
    {
        Vector3 newPosition = _dialogBoxObject.transform.position;
        newPosition.x += offsetX;
        _dialogBoxObject.transform.position = newPosition;
    }

    private async UniTask TypeTextAsync(string text, CancellationToken token)
    {
        _textMesh.text = "";
        _dialogBoxObject.SetActive(true);

        foreach (char c in text)
        {
            if (token.IsCancellationRequested)
                return;
            
            _textMesh.text += c;

            // Play sound only if the character is not in the ignored list
            if (_typeSounds.Length > 0 && _audioSource != null && !IsIgnoredCharacter(c))
            {
                _audioSource.pitch = Random.Range(0.8f, 1f);
                _audioSource.PlayOneShot(_typeSounds[Random.Range(0, _typeSounds.Length)]);
            }

            await UniTask.Delay(_charDelay, cancellationToken: token);
        }

        await UniTask.Delay(_delayBeforeHide, cancellationToken: token);
        _dialogBoxObject.SetActive(false);
    }

    private bool IsIgnoredCharacter(char c)
    {
        foreach (char ignored in _ignoredCharacters)
        {
            if (c == ignored)
                return true;
        }
        return false;
    }
}
