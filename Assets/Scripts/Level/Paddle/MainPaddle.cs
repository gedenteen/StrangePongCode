using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class MainPaddle : Paddle
{
    [Header("Changeable parameters")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float aiDeadZoneY = 0.8f;
    [SerializeField] private float timeForDoNothing = 0.2f;
    [SerializeField] private float timeToReact = 0.5f;

    [Header("Referencess to my objects")]
    [SerializeField] private AdditionalPaddle additionalPaddle;

    [Header("Referencess to scene objects")]
    [SerializeField] private BoxCollider2D upperWall;
    [SerializeField] private BoxCollider2D bottomWall;

    [Inject] KeyBindingsController _keyBindingsController;

    // Private fields
    private float _upperBound, _bottomBound; //bounds with walls
    private float _inputByUiButtons = 0f;
    private int _directionToMove = 0;
    private float _moveSpeedMultiplier = 1f;
    private float _timerToDoNothing = 0;
    private bool _thisPlayerIsAi = false; //set it in Awake()
    private bool _delayBeforeAction = true; // field for AI
    private bool _levelEnd = false;

    // Private const fields
    private const string player1InputName = "MovePlayer1";
    private const string player2InputName = "MovePlayer2";
    private readonly float minAiDeadZoneY = 0.6f;
    private readonly float maxAiDeadZoneY = 1.2f;
    private readonly float minTimeForDoNothing = 0.15f;
    private readonly float maxTimeForDoNothing = 0.35f;
    private readonly float minTimeToReact = 0.5f;
    private readonly float maxTimeToReact = 1f;

    // Private fields for idle move
    private float _idleMovementTimer = 0f;
    private float _idleMovementInterval = 1f / 4.4f; // time between direction switches. 
    private int _idleMoveDirection = 1; 

    // Private fields for delay before AI reacts to dangerous state
    private float _dangerousStateDelay = 0.3f; // You can tweak this
    private float _dangerousStateTimer = 0f;

    // Store the previous ball state to detect changes
    private BallState _prevBallState;

    private float _previousMoveDirection = 0f;
    private bool _isJerkingIncreasesImpactOnBall = false;
    private uint _countOfTwitches = 0;

    private new void Awake()
    {
        base.Awake();
    }

    private new void Start()
    {
        base.Start();
        _prevBallState = GameManager.instance.ball.state; 
    }

    private new void OnDestroy()
    {
        base.OnDestroy();
    }

    private void Update()
    {
        if (_levelEnd)
        {
            return;
        }

        // Check if ball state changed from usual to dangerous
        if (_prevBallState == BallState.usual && GameManager.instance.ball.state == BallState.dangerous)
        {
            // Start delay timer
            _dangerousStateTimer = _dangerousStateDelay;
        }
        _prevBallState = GameManager.instance.ball.state;

        if (_thisPlayerIsAi)
        {
            DetermineBehaviorOfAi();
        }
        else
        {
            float movement = GetInput();
            Move(movement);
        }
    }

    public void Initialize()
    {
        _thisPlayerIsAi = LevelManager.instance.IsThisPlayerAi(id);

        HandleLevelModifiers();

        float playerSkill = DifficultyManager.instance.GetRelativeSkillOfPlayer();

        // The better the player plays, the smaller the aiDeadZoneY should be
        aiDeadZoneY = Mathf.Lerp(minAiDeadZoneY, maxAiDeadZoneY, 1f - playerSkill);
        // The better the player plays, the less AI do useless movement
        timeForDoNothing = Mathf.Lerp(minTimeForDoNothing, maxTimeForDoNothing, 1f - playerSkill);
        // The better the player plays, the faster AI think
        timeToReact = Mathf.Lerp(minTimeToReact, maxTimeToReact, 1f - playerSkill);

        Debug.Log($"MainPaddle: Initialize(): playerSkill={playerSkill} aiDeadZoneY={aiDeadZoneY}");
        Debug.Log($"MainPaddle: Initialize(): timeForDoNothing={timeForDoNothing} timeToReact={timeToReact}");

        _upperBound = upperWall.transform.position.y - upperWall.bounds.extents.y;
        _bottomBound = bottomWall.transform.position.y + bottomWall.bounds.extents.y;
        Debug.Log($"MainPaddle: Initialize: upperWall.transform.position.y ={upperWall.transform.position.y} _upperBound={_upperBound}");
        Debug.Log($"MainPaddle: Initialize: bottomWall.transform.position.y ={bottomWall.transform.position.y} _bottomBound={_bottomBound}");

        EventsManager.levelEnd.AddListener(() =>
        {
            _levelEnd = true;
            Move(0f);
        });
    }

    private void HandleLevelModifiers()
    {
        var levelModifiers = LevelManager.instance.GetLevelModifiers();

        if (levelModifiers.ContainsKey(LevelModifier.AdditionalPaddle)
            && levelModifiers[LevelModifier.AdditionalPaddle] > 0)
        {
            additionalPaddle.gameObject.SetActive(true);
            //Debug.Log("MainPaddle: Initialize(): i am activate additional Paddle");
        }
        else
        {
            additionalPaddle.gameObject.SetActive(false);
            additionalPaddle = null;
        }

        if (levelModifiers.ContainsKey(LevelModifier.TwitchingIncreasesImpactOnBall) 
            && levelModifiers[LevelModifier.TwitchingIncreasesImpactOnBall] > 0)
        {
            _isJerkingIncreasesImpactOnBall = true;
        }
        else
        {
            _isJerkingIncreasesImpactOnBall = false;
        }
    }

    private float GetInput()
    {
        float movement = 0f;

        if (_inputByUiButtons != 0f)
        {
            movement = _inputByUiButtons;
        }
        else
        {
            switch (id)
            {
                case 1:
                    if (Input.GetKey(_keyBindingsController.KeyForLeftPlayerUp))
                    {
                        movement = 1f;
                    }
                    else if (Input.GetKey(_keyBindingsController.KeyForLeftPlayerDown))
                    {
                        movement = -1f;
                    }
                    break;
                case 2:
                    if (Input.GetKey(_keyBindingsController.KeyForRightPlayerUp))
                    {
                        movement = 1f;
                    }
                    else if (Input.GetKey(_keyBindingsController.KeyForRightPlayerDown))
                    {
                        movement = -1f;
                    }
                    break;
                default:
                    Debug.LogError($"Unexpected id={id} in GetInput() for Paddle");
                    break;
            }
        }

        return movement;
    }

    public float CalculateYDifference(Vector2 ballPos, Vector2 ballVelocity, Transform targetTransform)
    {
        if (ballVelocity.x == 0)
        {
            // If Ball rotates and not moves, then just calculate difference between positions
            return ballPos.y - targetTransform.position.y;
        }

        // If ball moves
        float xDifference = ballPos.x - targetTransform.position.x; //x-distance between ball and this paddle
        float timeToReach; // time to reach this paddle
        float yDifference; //y-distance taking into account the movement of the ball

        if (ballVelocity != Vector2.zero)
        {
            timeToReach = Mathf.Abs(xDifference) / Mathf.Abs(ballVelocity.x); // time of arrival of the Ball to this Paddle
            //Debug.Log($"MainPaddle: CalculateYDifference(): timeToReach={timeToReach}");
            yDifference = ballPos.y + ballVelocity.y * timeToReach - targetTransform.position.y;
        }
        else
        {
            //Debug.Log($"Paddle: MoveAi(): ballVelocity == Vector2.zero");
            yDifference = ballPos.y - targetTransform.position.y;
        }

        //Debug.Log($"MainPaddle: CalculateYDifference(): yDifference={yDifference}");
        return yDifference;
    }

    // Method for AI: determine whether it is necessary to move for this Paddle
    public void DetermineBehaviorOfAi()
    {
        if (_timerToDoNothing > 0)
        {
            _timerToDoNothing = Mathf.Clamp(_timerToDoNothing - Time.deltaTime, 0f, 2f);
            Move(_directionToMove);
            return;
        }

        Vector2 ballPos = GameManager.instance.ball.transform.position;
        Vector2 ballVelocity = GameManager.instance.ball.rb2d.velocity;

        // If ball is NOT flying towards me, we do the "idle movement"
        if ((id == 1 && ballVelocity.x > 0) || (id == 2 && ballVelocity.x < 0))
        {
            DoIdleMovement();
            _delayBeforeAction = true;
            return;
        }
        else
        {
            // If ball rotates in place => we keep _delayBeforeAction = true
            if (ballVelocity.x == 0)
            {
                _delayBeforeAction = true;
            }
            else if (_delayBeforeAction)
            {
                // If this Paddle have delayBeforeAction, then hold position.
                // Explanation: if the paddle starts action immediately after the ball flies in this direction, it will look unfair to the player.
                // To avoid this, I made a delay before action (in other words, to make the AI not react immediately).
                _directionToMove = 0;  // no movement
                Move(_directionToMove);
                _timerToDoNothing = timeToReact;
                _delayBeforeAction = false;
                return;
            }
        }

        DetermineMovementOfAi(ballPos, ballVelocity);
    }

    // Method for AI: determine movement for this Paddle
    private void DetermineMovementOfAi(Vector2 ballPos, Vector2 ballVelocity)
    {
        // If the ball is in dangerous state and we still have delay => do not move
        if (GameManager.instance.ball.state == BallState.dangerous && _dangerousStateTimer > 0f)
        {
            _dangerousStateTimer -= Time.deltaTime;
            _directionToMove = 0;
            Move(_directionToMove);
            return;
        }

        float yDifference = CalculateYDifference(ballPos, ballVelocity, transform);

        if (additionalPaddle != null)
        {
            // If this paddle have additional paddle, then calculate yDiffierence for additional paddle
            // And choose minimal yDifference
            float temp = CalculateYDifference(ballPos, ballVelocity, additionalPaddle.transform);
            if (Mathf.Abs(temp) < Mathf.Abs(yDifference))
            {
                // Rewrite yDifference. And set minus, because we need move to opposite direction
                yDifference = -temp;
            }
        }

        if (Mathf.Abs(yDifference) > aiDeadZoneY && GameManager.instance.ball.state == BallState.usual)
        {
            _directionToMove = yDifference > 0 ? 1 : -1;
            _timerToDoNothing = timeForDoNothing;
        }
        else if (GameManager.instance.ball.state == BallState.dangerous)
        {
            if (Mathf.Abs(yDifference) < boundY * 2)
            {
                // Check does the Paddle rest against the Wall??
                _directionToMove = yDifference > 0 ? -1 : 1;
                _timerToDoNothing = timeForDoNothing;
            }
            else
            {
                // Ball is far away enough, don't move
                _directionToMove = 0;
                _timerToDoNothing = timeForDoNothing * 3;
            }
        }
        else
        {
            _directionToMove = 0;
        }

        Move(_directionToMove);
    }

    // Method to make the paddle "idle move" up and down when the ball is not flying towards it
    private void DoIdleMovement()
    {
        // Timer for direction toggling
        _idleMovementTimer += Time.deltaTime;
        if (_idleMovementTimer >= _idleMovementInterval)
        {
            _idleMovementTimer = 0f;
            // Toggle direction from 1 to -1 or -1 to 1
            _idleMoveDirection = -_idleMoveDirection;
        }

        Move(_idleMoveDirection);
    }

    private void Move(float movement)
    {
        Vector2 velo = rb2d.velocity;
        velo.y = movement * _moveSpeedMultiplier * moveSpeed;
        rb2d.velocity = velo;

        HandleTwitching(movement);

        if (additionalPaddle != null)
        {
            // We'll invert the velocity for the additional paddle
            additionalPaddle.Move(-velo);
        }
    }

    private void HandleTwitching(float movement) // подергивания вниз-вверх
    {
        if (!_isJerkingIncreasesImpactOnBall)
        {
            return;
        }

        // Debug.Log($"MainPaddle: Move: movement={movement} " + 
        //     $"_previousMoveDirection={_previousMoveDirection}");

        if ((_previousMoveDirection > 0.1f && movement < -0.1f)
            ||
            (_previousMoveDirection < -0.1f && movement > 0.1f))
        {
            _countOfTwitches++;
            //Debug.Log($"MainPaddle: Move: new _countOfJerks={_countOfTwitches}");

            if (additionalPaddle != null)
            {
                BallAccelerator.IncreaseSpeedSummand(2);
                additionalPaddle.BallAccelerator.IncreaseSpeedSummand(2);
            }
            else
            {
                BallAccelerator.IncreaseSpeedSummand(1);
            }
        }

        if (movement > 0.1f || movement < -0.1f)
        {
            _previousMoveDirection = movement;
        }
    }

    public void InputFromUi(float value)
    {
        _inputByUiButtons += value;
    }
}
