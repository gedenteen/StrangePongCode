using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;
using Zenject;

public class StarsSpawner : MonoBehaviour
{
    [Header("Changeable values")]
    public bool spawnStarsBallDirection = false;
    public bool spawnStarsMusic = false;
    public bool spawnStarsBallState = false;
    public bool spawnStarsGhost = false;

    [Header("References to components")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("References to assets")]
    [SerializeField] private List<Sprite> _spritesForAppear;
    [SerializeField] private StarTrigger prefabStarDirection;
    [SerializeField] private StarTrigger prefabStarMusic;
    [SerializeField] private StarTrigger prefabStarBall;
    [SerializeField] private StarTrigger prefabStarGhost;
    
    [Inject] private ConfigForStarsSpawner _myConfig;

    private float maxDistanceX, maxDistanceY;
    private bool canSpawn = true;

    private void Awake()
    {
        EventsManager.levelStart.AddListener(StartSpawn);
        EventsManager.levelEnd.AddListener(StopSpawn);

        var levelModifiers = LevelManager.instance.GetLevelModifiers();
        if (levelModifiers.ContainsKey(LevelModifier.AdditionalPaddle) &&
            levelModifiers[LevelModifier.AdditionalPaddle] > 0)
        {
            Vector2 size = spriteRenderer.size;
            size.x -= 2.5f;
            spriteRenderer.size = size;
        }

        maxDistanceX = spriteRenderer.size.x / 2f;
        maxDistanceY = spriteRenderer.size.y / 2f;
    }

    private void OnDestroy()
    {
        canSpawn = false;
        EventsManager.levelStart.RemoveListener(StartSpawn);
        EventsManager.levelEnd.RemoveListener(StopSpawn);
    }

    private void StartSpawn()
    {
        canSpawn = true;

        if (spawnStarsBallDirection)
        {
            SpawnStars(prefabStarDirection, 
                _myConfig.TimeForSpawnStarsDirection, 0f, 0f,
                _myConfig.SpriteForTriggerChangeDirection,
                _myConfig.ColorForTriggerChangeDirection).Forget();
        }
        if (spawnStarsMusic)
        {
            SpawnStars(prefabStarMusic, 
                _myConfig.TimeForSpawnStarsMusic, 12f, 50f,
                _myConfig.SpriteForTriggerMusic,
                _myConfig.ColorForTriggerMusic).Forget();
        }
        if (spawnStarsBallState)
        {
            SpawnStars(prefabStarBall, 
                _myConfig.TimeForSpawnStarsDangerousBall, 0f, 0f,
                _myConfig.SpriteForTriggerDangerousBall,
                _myConfig.ColorForTriggerDangerousBall).Forget();
        }
        if (spawnStarsGhost)
        {
            SpawnStars(prefabStarGhost, 
                _myConfig.TimeForSpawnStarsGhost, 0f, 0f,
                _myConfig.SpriteForTriggerGhost,
                _myConfig.ColorForTriggerGhost).Forget();
        }
    }

    private void StopSpawn()
    {
        canSpawn = false;
    }

    private async UniTaskVoid SpawnStars(StarTrigger prefabOfStarTrigger,
        float timeToWait, float timeToIncrease, float timeLimit,
        Sprite finalSprite, Color finalColor)
    {
        float timer = timeToWait - _myConfig.TimeToAppearForAnyTrigger;

        while (canSpawn)
        {
            await UniTask.WaitForSeconds(timer);

            if (timer <= timeLimit)
            {
                timer += timeToIncrease;
            }

            // Determine random position
            float x = Random.Range(-maxDistanceX, maxDistanceX);
            float y = Random.Range(-maxDistanceY, maxDistanceY);

            if (!canSpawn)
            {
                break;
            }

            StarTrigger starTrigger = Instantiate(
                prefabOfStarTrigger, new Vector3(x, y, 0), Quaternion.identity, transform);

            float delay = _myConfig.TimeToAppearForAnyTrigger / (_spritesForAppear.Count + 1);

            // Gradually change starTrigger
            for (int i = 0; i < _spritesForAppear.Count; i++)
            {
                if (!canSpawn)
                {
                    break;
                }

                float t = (float)i / (_spritesForAppear.Count + 1);
                Color interpolatedColor = Color.Lerp(
                    _myConfig.ColorForAppearForAnyTrigger, finalColor, t);
                
                starTrigger.SetSpriteAndColor(_spritesForAppear[i], interpolatedColor);
                await UniTask.WaitForSeconds(delay);
            }

            // Set final state
            starTrigger.SetSpriteAndColor(finalSprite, finalColor);
            starTrigger.IsReady = true;

            if (_myConfig.PlaySound)
            {
                AudioManager.instance.PlayCreationOfStarTriggerSound();
            }
        }
    }
}
