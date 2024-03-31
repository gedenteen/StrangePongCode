using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallAudio : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip resetBallSound;
    public AudioClip wallSound;
    public AudioClip paddleSound;

    private bool canPlay = false;

    private void Awake()
    {
        // Get values from AudioManager
        canPlay = AudioManager.instance.canPlaySounds;
        audioSource.volume = AudioManager.instance.audioSourceSounds.volume;
        
        // Add listener to event
        AudioManager.instance.onSoundsVolumeChange += ChangeVolumeSounds;
    }

    private void OnDestroy()
    {
        AudioManager.instance.onSoundsVolumeChange -= ChangeVolumeSounds;
    }

    public void PlayResetBallSound()
    {
        if (canPlay) audioSource.PlayOneShot(resetBallSound);
    }
    public void PlayWallSound()
    {
        if (canPlay) audioSource.PlayOneShot(wallSound);
    }
    public void PlayPaddleSound()
    {
        if (canPlay) audioSource.PlayOneShot(paddleSound);
    }

    public void ChangeVolumeSounds(float volume)
    {
        audioSource.volume = volume;
    }
}
