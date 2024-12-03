using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainPaddle : Paddle
{
    // Public fields
    public float moveSpeed = 2f;
    public float aiDeadZoneY = 0.8f;
    public float timeForDoNothing = 0.2f;
    public float timeToReact = 0.5f;
    public AdditionalPaddle additionalPaddle = null;

    // Private fields
    private float upperBound, bottomBound; //bounds with walls
    private float inputByUiButtons = 0f;
    private int direction = 0; //direction to move
    private float moveSpeedMultiplier = 1f;
    private float timerToDoNothing = 0;
    private bool thisPlayerIsAi = false; //set it in Awake()
    private bool delayBeforeAction = true; // field for AI
    private bool levelEnd = false;

    [Header("Refs to scene objects")]
    [SerializeField] private BoxCollider2D upperWall;
    [SerializeField] private BoxCollider2D bottomWall;

    // Private const fields
    private const string player1InputName = "MovePlayer1";
    private const string player2InputName = "MovePlayer2";
    private readonly float minAiDeadZoneY = 0.5f;
    private readonly float maxAiDeadZoneY = 1.4f;
    private readonly float minTimeForDoNothing = 0.15f;
    private readonly float maxTimeForDoNothing = 0.35f;
    private readonly float minTimeToReact = 0.25f;
    private readonly float maxTimeToReact = 0.9f;

    private new void Awake()
    {
        base.Awake();
    }

    private new void Start()
    {
        base.Start();
    }

    private new void OnDestroy()
    {
        base.OnDestroy();
    }

    private void Update()
    {
        if (levelEnd)
        {
            return;
        }

        if (thisPlayerIsAi)
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
        thisPlayerIsAi = LevelManager.instance.IsThisPlayerAi(id);

        var levelModifiers = LevelManager.instance.GetLevelModifiers();
        if (levelModifiers.ContainsKey(LevelModifier.AdditionalPaddle) && levelModifiers[LevelModifier.AdditionalPaddle] > 0)
        {
            additionalPaddle.gameObject.SetActive(true);
            Debug.Log("MainPaddle: Initialize(): i am activate additional Paddle");
        }
        else
        {
            additionalPaddle.gameObject.SetActive(false);
            additionalPaddle = null;
        }

        float playerSkill = DifficultyManager.instance.GetRelativeSkillOfPlayer();

        // The better the player plays, the smaller the aiDeadZoneY should be
        aiDeadZoneY = Mathf.Lerp(minAiDeadZoneY, maxAiDeadZoneY, 1f - playerSkill);
        // The better the player plays, the less AI do useless movement
        timeForDoNothing = Mathf.Lerp(minTimeForDoNothing, maxTimeForDoNothing, 1f - playerSkill);
        // The better the player plays, the faster AI think
        timeToReact = Mathf.Lerp(minTimeToReact, maxTimeToReact, 1f - playerSkill);

        Debug.Log($"MainPaddle: Initialize(): playerSkill={playerSkill} aiDeadZoneY={aiDeadZoneY}");
        Debug.Log($"MainPaddle: Initialize(): timeForDoNothing={timeForDoNothing} timeToReact={timeToReact}");

        upperBound = upperWall.transform.position.y - upperWall.bounds.extents.y;
        bottomBound = bottomWall.transform.position.y + bottomWall.bounds.extents.y;
        Debug.Log($"MainPaddle: Initialize: upperWall.transform.position.y ={upperWall.transform.position.y} upperBound={upperBound}");
        Debug.Log($"MainPaddle: Initialize: bottomWall.transform.position.y ={bottomWall.transform.position.y} bottomBound={bottomBound}");

        EventsManager.levelEnd.AddListener(() =>
        {
            levelEnd = true;
            Move(0f);
        });
    }

    private float GetInput()
    {
        float movement = 0f;

        if (inputByUiButtons != 0f)
        {
            movement = inputByUiButtons;
        }
        else
        {
            switch (id)
            {
                case 1:
                    movement = Input.GetAxis(player1InputName);
                    break;
                case 2:
                    movement = Input.GetAxis(player2InputName);
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
        float timeToReach; // time ro reach this paddle
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
        // Unbeatable AI code:
        //transform.position = new Vector2(startPosition.x, ballPos.y); 
        //return;

        if (timerToDoNothing > 0)
        {
            timerToDoNothing = Mathf.Clamp(timerToDoNothing - Time.deltaTime, 0f, 2f);
            Move(direction);
            return;
        }

        Vector2 ballPos = GameManager.instance.ball.transform.position;
        Vector2 ballVelocity = GameManager.instance.ball.rb2d.velocity;

        // Does Ball flies to another Paddle?
        if ((id == 1 && ballVelocity.x > 0) || (id == 2 && ballVelocity.x < 0))
        {
            // If yes - hold position
            direction = 0; // no movement
            Move(direction);
            delayBeforeAction = true;
            return;
        }
        else
        {
            // If no, check this:
            // if Ball rotates and no moves, then this paddle should go closer to Ball.
            // If Ball moves then check for delayBeforeAction. If there is no delay, then start calculating and movement
            if (ballVelocity.x == 0)
            {
                delayBeforeAction = true;
            }
            else if (delayBeforeAction)
            {
                // If this Paddle have delayBeforeAction, then hold position.
                // Explanation: if the paddle starts action immediately after the ball flies in this direction, it will look unfair to the player.
                // To avoid this, I made a delay before action (in other words, to make the AI not react immediately).
                direction = 0;  // no movement
                Move(direction);
                timerToDoNothing = timeToReact;
                delayBeforeAction = false;
                return;
            }
        }

        DetermineMovementOfAI(ballPos, ballVelocity);
    }

    // Method for AI: determine movement for this Paddle
    private void DetermineMovementOfAI(Vector2 ballPos, Vector2 ballVelocity)
    {
        float yDifference = CalculateYDifference(ballPos, ballVelocity, transform);

        if (additionalPaddle is not null)
        {
            // If this paddle have additional paddle, then calculate yDiffierence for additional paddle
            // And choose minimal yDifference
            float temp = CalculateYDifference(ballPos, ballVelocity, additionalPaddle.transform);
            if (Mathf.Abs(temp) < Mathf.Abs(yDifference))
            {
                // Rewrite yDifference. And set minus, because we need move to different direction as MainPaddle
                yDifference = -temp;
            }
        }

        if (Mathf.Abs(yDifference) > aiDeadZoneY && GameManager.instance.ball.state == BallState.usual)
        {
            if (GameManager.instance.ball.state == BallState.usual)
            {
                direction = yDifference > 0 ? 1 : -1;
                timerToDoNothing = timeForDoNothing;
            }
        }
        else if (GameManager.instance.ball.state == BallState.dangerous)
        {
            if (Mathf.Abs(yDifference) < boundY * 2)
            {
                // Check does the Paddle rest against the Wall??
                direction = yDifference > 0 ? -1 : 1;
                timerToDoNothing = timeForDoNothing;
            }
            else
            {
                // Ball is far away enough, don't move
                direction = 0;
                timerToDoNothing = timeForDoNothing * 3;
            }
        }
        else
        {
            direction = 0;
        }

        // TODO: check for walls
        // if (direction == 1)
        // {
        //     float dif = upperBound - (transform.position.y + boundY);
        //     Debug.Log($"MainPaddle: DetermineMovementOfAI: dif={dif}");
        // }
        // else if (direction == -1)
        // {
        //     float dif = bottomBound - (transform.position.y - boundY);
        //     Debug.Log($"MainPaddle: DetermineMovementOfAI: dif={dif}");
        // }

        Move(direction);

    }

    private void Move(float movement)
    {
        Vector2 velo = rb2d.velocity;
        velo.y = movement * moveSpeedMultiplier * moveSpeed;
        rb2d.velocity = velo;

        if (additionalPaddle != null)
        {
            additionalPaddle.Move(-velo);
        }
    }

    public void InputFromUi(float value)
    {
        inputByUiButtons += value;
    }
}
