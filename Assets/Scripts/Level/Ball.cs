// using System;
using System.Collections;
using UnityEngine;

public enum BallState
{
    usual,
    dangerous,
    ghost
}

public class Ball : MonoBehaviour
{
    /// Public fields
    [HideInInspector] public BallState state = BallState.usual;
    [HideInInspector] public Rigidbody2D rb2d;
    [HideInInspector] public BallAudio ballAudio;
    [HideInInspector] public ParticleSystem collisionParticle;

    [Header("References to assets")]
    [SerializeField] private GameObject prefabBall;

    [Header("Changable values")]
    [SerializeField] private float minInitialAngle = 0.05f;
    [SerializeField] private float maxInitialAngle = 0.75f;
    [SerializeField] private float rotationDuration = 0.3f; /// Duration in seconds
    [SerializeField] private int rotationCount = 5; /// Number of rotations
    [SerializeField] private float maxStartY = 4f;
    [SerializeField] private float startMoveSpeed = 5f;
    [SerializeField] private float speedSummand = 1f;
    [SerializeField] private int countOfParicles = 16;

    /// Private fields
    private SpriteRenderer spriteRenderer;
    private TrailRenderer trailRenderer;
    private float startX = 0f;
    private float currentMoveSpeed = 0f;
    private bool makeDangerousAfterHit = false;
    private bool forciblyChangedDirection = false;

    private void OnDestroy()
    {
        GameManager.instance.onReset -= ResetBall;
    }

    public void FindComponents()
    {
        rb2d = GetComponentInChildren<Rigidbody2D>();
        collisionParticle = GetComponentInChildren<ParticleSystem>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        trailRenderer = GetComponentInChildren<TrailRenderer>();
    }

    public void Initialize()
    {
        FindComponents();

        var levelModifiers = LevelManager.instance.GetLevelModifiers();
        if (levelModifiers.ContainsKey(LevelModifier.MakeBallDangerousAfterHitting) && levelModifiers[LevelModifier.MakeBallDangerousAfterHitting] == 1)
        {
            makeDangerousAfterHit = true;
        }
        else
        {
            GameManager.instance.onReset += ResetBall;
        }

        EventsManager.levelStart.AddListener(Push);
        EventsManager.levelEnd.AddListener(() => { GameManager.instance.onReset -= ResetBall; });
        EventsManager.disableSoundForBalls.AddListener(() => { ballAudio.gameObject.SetActive(false); });
        EventsManager.changeBallDirection.AddListener(() => { StartCoroutine(StopAndMakeNewDirection()); });
        EventsManager.changeBallBehavior.AddListener(MakeItDangerous);
        EventsManager.createGhostBall.AddListener(CreateGhostAndChangeDirection);
    }

    private void ResetBall()
    {
        // play sound
        ballAudio.PlayResetBallSound();

        // position
        float posY = UnityEngine.Random.Range(-maxStartY, maxStartY);
        Vector2 pos = new Vector2(startX, posY);
        transform.position = pos;

        // Delete ball's trail
        trailRenderer.Clear();

        // Make current speed = 0, we need to use startMoveSpeed
        currentMoveSpeed = 0f;

        forciblyChangedDirection = false;

        if (state == BallState.dangerous)
        {
            MakeItUsual();
        }

        StartCoroutine(StopAndMakeNewDirection());
    }

    private IEnumerator StopAndMakeNewDirection()
    {
        /// stop the ball
        rb2d.velocity = Vector2.zero;

        /// rotation
        for (int i = 0; i < rotationCount; i++)
        {
            float elapsedTime = 0f;
            Quaternion startRotation = transform.rotation;
            Quaternion targetRotation = Quaternion.Euler(0f, 0f, 90f);

            while (elapsedTime < rotationDuration)
            {
                transform.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsedTime / rotationDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            transform.rotation = new Quaternion();
        }

        /// push the ball
        Push();
    }

    private void Push()
    {
        // Create direction
        Vector2 dir = UnityEngine.Random.value < 0.5f ? Vector2.left : Vector2.right; //random choice: to the left or to the right
        dir.y = UnityEngine.Random.Range(minInitialAngle, maxInitialAngle); //random choice for angle (between minInitialAngle and maxInitialAngle)
        if (UnityEngine.Random.value < 0.5f) //random choice: make the angle negative
        {
            dir.y *= -1;
        }
        dir = dir.normalized;
        //Debug.Log($"Ball: Push(): dir={dir}");

        PushToDirection(dir);
    }

    public void PushToDirection(Vector2 dir)
    {
        if (currentMoveSpeed != 0f)
        {
            rb2d.velocity = dir * currentMoveSpeed;
        }
        else
        {
            rb2d.velocity = dir * startMoveSpeed;
            currentMoveSpeed = startMoveSpeed;
        }

        //Debug.Log($"Ball: Push(): rb2d.velocity={rb2d.velocity} currentMoveSpeed={currentMoveSpeed} rb2d.velocity.magnitude={rb2d.velocity.magnitude}");

        EmitParticle(countOfParicles);
    }

    /// Method for collisons with ScoreZone
    private void OnTriggerEnter2D(Collider2D collision)
    {
        ScoreZone scoreZone = collision.GetComponent<ScoreZone>();
        if (scoreZone != null)
        {
            GameManager.instance.OnScoreZoneReached(state, scoreZone.id);
            if (LevelManager.instance.GetLevelModifiers()[LevelModifier.BossBigBase] != 0)
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Paddle paddle = collision.collider.GetComponent<Paddle>();
        //Debug.Log($"Ball: OnCollisionEnter2D(): paddle is null? {paddle == null}");
        if (paddle != null)
        {
            ballAudio.PlayPaddleSound();
            EmitParticle(countOfParicles / 4);

            if (state == BallState.usual)
            {
                currentMoveSpeed += speedSummand;
                rb2d.velocity = rb2d.velocity.normalized * currentMoveSpeed;
                //Debug.Log($"Ball: OnCollisionEnter2D(): rb2d.velocity={rb2d.velocity} currentMoveSpeed={currentMoveSpeed} rb2d.velocity.magnitude={rb2d.velocity.magnitude}");

                // Check for situation when players dispersed high speed of the Ball
                if (currentMoveSpeed > 110f && !forciblyChangedDirection)
                {
                    Vector2 position = transform.position;
                    position.x = 0f;
                    transform.position = position;
                    StartCoroutine(StopAndMakeNewDirection());
                    forciblyChangedDirection = true;
                }

                if (makeDangerousAfterHit)
                {
                    MakeItDangerous();
                }
            }
            else if (state == BallState.dangerous)
            {
                GameManager.instance.DecreaseHpOfPlayer(paddle.id);
                if (makeDangerousAfterHit)
                {
                    DestroyMe();
                }
                else
                {
                    MakeItUsual();
                }
            }
        }

        Wall wall = collision.collider.GetComponent<Wall>();
        if (wall != null)
        {
            ballAudio.PlayWallSound();
            EmitParticle(countOfParicles / 4);
        }
    }

    private void EmitParticle(int amount)
    {
        collisionParticle.Emit(amount);
    }

    private void ChangeColor(Color newColor)
    {
        spriteRenderer.color = newColor;
        trailRenderer.startColor = newColor;
        ParticleSystem.MainModule settings = collisionParticle.main;
        settings.startColor = newColor;
    }

    private void MakeItDangerous()
    {
        state = BallState.dangerous;
        ChangeColor(MyColors.red);
    }

    public void MakeItGhost()
    {
        state = BallState.ghost;
        ChangeColor(MyColors.yellow);
        gameObject.layer = LayerMask.NameToLayer("Ghost");
    }

    private void MakeItUsual()
    {
        state = BallState.usual;
        ChangeColor(Color.white);
    }

    private void CreateGhostAndChangeDirection()
    {
        GameObject go = Instantiate(prefabBall, transform.position, Quaternion.identity);

        Ball ballComponent = go.GetComponent<Ball>();
        if (ballComponent is null)
        {
            Debug.LogError("Ball: CreateGhostAndChangeDirection: can't get Ball component");
            return;
        }

        ballComponent.FindComponents();
        ballComponent.MakeItGhost();
        ballComponent.rb2d.velocity = rb2d.velocity;//ballComponent.PushToDirection(rb2d.velocity);

        Debug.Log($"Ball: CreateGhostAndChangeDirection: old velocity={rb2d.velocity.magnitude}");
        currentMoveSpeed = startMoveSpeed;
        // Change direction of this Ball
        Push();
        Debug.Log($"Ball: CreateGhostAndChangeDirection: new velocity={rb2d.velocity.magnitude}");

    }

    private void DestroyMe()
    {
        Destroy(gameObject);
    }
}
