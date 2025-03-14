using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBigBase : Paddle
{
    // Public fields
    public float moveSpeed = 1f;
    public float timeToSpawnBall = 0.5f;

    // Private fields
    [Header("References to my objects")]
    [SerializeField] private GameObject[] myGuns;
    [SerializeField] private AudioSource myAudioSource;

    [Header("References to assets")]
    [SerializeField] private AudioClip audioShoot;
    [SerializeField] private AudioClip audioExplosion;
    [SerializeField] private Ball prefabBall;
    
    private Vector3 positonForBall = new Vector3();
    private Vector2 directionForBall = new Vector2(-1f, 0f); //to the left

    private IEnumerator coroutineShoot;
    private IEnumerator coroutineMove;

    // Private const fields
    private readonly float minTimeToSpawnBall = 0.45f;
    private readonly float maxTimeToSpawnBall = 0.65f;

    private void OnValidate()
    {
        base.OnValidate();
        myAudioSource ??= GetComponentInChildren<AudioSource>();
    }

    private new void Awake()
    {
        base.Awake();
    }

    public void Initialize()
    {
        if (myAudioSource == null)
        {
            Debug.LogError("BossBigBase: Awake: can't find AudioSource");
        }
        myAudioSource.clip = audioShoot;

        float playerSkill = DifficultyManager.instance.GetRelativeSkillOfPlayer();

        // The better the player plays, the more often the Balls should fly out
        timeToSpawnBall = Mathf.Lerp(minTimeToSpawnBall, maxTimeToSpawnBall, 1f - playerSkill);
        Debug.Log($"BossBigBase: Update(): playerSkill={playerSkill} timeToSpawnBall={timeToSpawnBall}");

        // Add listeners to events
        EventsManager.levelStart.AddListener(StartCoroutines);
        EventsManager.levelEnd.AddListener(StopCoroutines);

        Debug.Log("BossBigBase: Initialize: end");
    }

    private void StartCoroutines()
    {
        coroutineShoot = CoroutineShoot();
        StartCoroutine(coroutineShoot);
        coroutineMove = CoroutineMoving();
        StartCoroutine(coroutineMove);
    }

    private void StopCoroutines()
    {
        StopCoroutine(coroutineShoot);
        StopCoroutine(coroutineMove);
    }

    private IEnumerator CoroutineShoot()
    {
        int minIndex = 0;
        int maxIndex = myGuns.Length;
        int index;

        while (true)
        {
            // Debug.Log($"random num = {Random.Range(minIndex, maxIndex)}");
            index = Random.Range(minIndex, maxIndex);

            positonForBall = myGuns[index].transform.position;
            positonForBall.x -= 1f;
            Ball ballScipt = Instantiate(prefabBall, positonForBall, Quaternion.identity);
            ballScipt.Initialize();
            ballScipt.PushToDirection(directionForBall);
            myAudioSource.Play(); // play sound shoot

            yield return new WaitForSeconds(timeToSpawnBall);
        }
    }

    private IEnumerator CoroutineMoving()
    {
        float time = 4f;
        float timer = 2f;
        float direction = 1f;

        while (true)
        {
            Move(direction);

            timer += Time.deltaTime;
            if (timer > time)
            {
                timer = 0f;
                direction *= -1f;
            }

            yield return null;
        }
    }

    private void Move(float movement)
    {
        Vector2 velo = rb2d.velocity;
        velo.y = movement * moveSpeed;
        rb2d.velocity = velo;
    }

    public void Die()
    {
        Debug.Log("BossBigBase: i am dying...");

        Move(0f);
        StopCoroutines();

        // Disable sounds for all Balls + stop music + play sound of my destroying
        EventsManager.disableSoundForBalls.Invoke();
        AudioManager.instance.StopMusic();
        myAudioSource.clip = audioExplosion;
        myAudioSource.Play();
    }
}
