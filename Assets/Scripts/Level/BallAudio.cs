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
        canPlay = AudioManager.instance.CanPlaySounds;
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
}
