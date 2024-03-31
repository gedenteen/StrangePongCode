using System.Collections;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class StarsSpawner : MonoBehaviour
{
    [Header("Changable values")]
    public bool spawnStarsBallDirection = false;
    public bool spawnStarsMusic = false;
    public bool spawnStarsBallState = false;
    public bool spawnStarsGhost = false;


    public float timeForSpawnStarsDirection = 9f;
    public float timeForSpawnStarsMusic = 10f;
    public float timeForSpawnStarsBallState = 15f;
    public float timeForSpawnStarsGhost = 13f;

    [Header("References")]
    [SerializeField] private GameObject prefabStarDirection;
    [SerializeField] private GameObject prefabStarMusic;
    [SerializeField] private GameObject prefabStarBall;
    [SerializeField] private GameObject prefabStarGhost;

    // Components
    private SpriteRenderer spriteRenderer;

    private IEnumerator coroutineStarsDirection;
    private IEnumerator coroutineStarsMusic;
    private IEnumerator coroutineStarsBallState;
    private IEnumerator coroutineStarsGhost;

    private float maxDistanceX, maxDistanceY;

    private void Awake()
    {
        spriteRenderer = this.GetComponent<SpriteRenderer>();
        // Debug.Log($"StarsSpawner: size={spriteRenderer.size}");

        EventsManager.levelStart.AddListener(StartSpawn);
        EventsManager.levelEnd.AddListener(StopSpawn);

        var levelModifiers = LevelManager.instance.GetLevelModifiers();
        if (levelModifiers.ContainsKey(LevelModifier.AdditionalPaddle) && levelModifiers[LevelModifier.AdditionalPaddle] > 0)
        {
            // Decrease area for spawn so that the StarTriggers are not between MainPaddle and AdditionalPaddle
            Vector2 size = spriteRenderer.size;
            size.x -= 2.5f;
            spriteRenderer.size = size;
        }

        // Object for this script should be in pos (0, 0). Calculate max distance as size / 2.
        maxDistanceX = (float)spriteRenderer.size.x / 2;
        maxDistanceY = (float)spriteRenderer.size.y / 2;
    }

    private void StartSpawn()
    {
        if (spawnStarsBallDirection)
        {
            coroutineStarsDirection = CoroutineSpawnTrigger(prefabStarDirection, timeForSpawnStarsDirection, 0f, 0f);
            StartCoroutine(coroutineStarsDirection);
        }
        if (spawnStarsMusic)
        {
            coroutineStarsMusic = CoroutineSpawnTrigger(prefabStarMusic, timeForSpawnStarsMusic, 12f, 50f);
            StartCoroutine(coroutineStarsMusic);
        }
        if (spawnStarsBallState)
        {
            coroutineStarsBallState = CoroutineSpawnTrigger(prefabStarBall, timeForSpawnStarsBallState, 0f, 0f);
            StartCoroutine(coroutineStarsBallState);
        }
        if (spawnStarsGhost)
        {
            coroutineStarsGhost = CoroutineSpawnTrigger(prefabStarGhost, timeForSpawnStarsGhost, 0f, 0f);
            StartCoroutine(coroutineStarsGhost);
        }
    }

    private void StopSpawn()
    {
        if (spawnStarsBallDirection)
            StopCoroutine(coroutineStarsDirection);
        if (spawnStarsMusic)
            StopCoroutine(coroutineStarsMusic);
        if (spawnStarsBallState)
            StopCoroutine(coroutineStarsBallState);
        if (spawnStarsGhost)
            StopCoroutine(coroutineStarsGhost);
    }

    private IEnumerator CoroutineSpawnTrigger(GameObject prefabOfStarTrigger, float timeToWait, float timeToIncrease, float timeLimit)
    {
        float timer = timeToWait;
        while (true)
        {
            yield return new WaitForSeconds(timer);
            if (timer <= timeLimit)
            {
                timer += timeToIncrease;
            }

            float x = Random.Range(-maxDistanceX, maxDistanceX);
            float y = Random.Range(-maxDistanceY, maxDistanceY);
            Instantiate(prefabOfStarTrigger, new Vector3(x, y, 0), Quaternion.identity);
        }
    }
}
