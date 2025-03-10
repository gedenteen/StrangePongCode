using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks.Triggers;
using UnityEngine;

public class Paddle : MonoBehaviour
{
    // Private fields
    protected Vector3 startPosition;
    protected float boundX, boundY; // size from center to horizontal bound and to vertical bound

    [Header("Main")]
    public int id;
    [field: SerializeField] public BallAccelerator BallAccelerator { get; private set; }
    [SerializeField] protected Rigidbody2D rb2d;
    [SerializeField] protected BoxCollider2D bc2d;

    protected void OnValidate()
    {
        rb2d ??= GetComponent<Rigidbody2D>();
        bc2d ??= GetComponent<BoxCollider2D>();
    }

    protected void Awake()
    {
        Debug.Log($"Paddle: Awake(): bc2d.bounds.extents.x={bc2d.bounds.extents.x} bc2d.bounds.extents.y={bc2d.bounds.extents.y}");
        boundX = bc2d.bounds.extents.x;
        boundY = bc2d.bounds.extents.y;
    }

    protected void Start()
    {
        startPosition = transform.position;

        // If it usual level (without MakeBallDangerousAfterHitting), then reset position for GameManager.instance.onReset
        var levelModifiers = LevelManager.instance.GetLevelModifiers();

        if (levelModifiers.ContainsKey(LevelModifier.MakeBallDangerousAfterHitting) && levelModifiers[LevelModifier.MakeBallDangerousAfterHitting] == 1)
            return;
        if (levelModifiers.ContainsKey(LevelModifier.BossBigBarrier) && levelModifiers[LevelModifier.BossBigBarrier] == 1 && id == 2)
            return;

        Debug.Log($"Paddle: Start: add onReset callback, my id = {id}");
        GameManager.instance.onReset += ResetPosition;
    }

    protected void OnDestroy()
    {
        var levelModifiers = LevelManager.instance.GetLevelModifiers();

        if (levelModifiers.ContainsKey(LevelModifier.MakeBallDangerousAfterHitting) && levelModifiers[LevelModifier.MakeBallDangerousAfterHitting] == 1)
            return;
        if (levelModifiers.ContainsKey(LevelModifier.BossBigBarrier) && levelModifiers[LevelModifier.BossBigBarrier] == 1 && id == 2)
            return;

        GameManager.instance.onReset -= ResetPosition;
    }

    protected void ResetPosition()
    {
        transform.position = startPosition;
    }
}
