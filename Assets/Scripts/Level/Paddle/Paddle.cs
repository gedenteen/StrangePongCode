using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paddle : MonoBehaviour
{
    // Components
    protected Rigidbody2D rb2d;
    protected BoxCollider2D bc2d;

    // Private fields
    protected Vector3 startPosition;
    protected float boundX, boundY; // size from center to horizontal bound and to vertical bound

    // Public fields
    public int id;

    protected void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        bc2d = GetComponent<BoxCollider2D>();
        Debug.Log($"Paddle: Awake(): bc2d.bounds.extents.x={bc2d.bounds.extents.x} bc2d.bounds.extents.y={bc2d.bounds.extents.y}");
        boundX = bc2d.bounds.extents.x;
        boundY = bc2d.bounds.extents.y;
    }

    protected void Start()
    {
        startPosition = transform.position;

        // If it usual level (without MakeBallDangerousAfterHitting), then reset position for GameManager.instance.onReset
        var levelModifiers = LevelManager.instance.GetLevelModifiers();
        if (!levelModifiers.ContainsKey(LevelModifier.MakeBallDangerousAfterHitting) || levelModifiers[LevelModifier.MakeBallDangerousAfterHitting] == 0)
        {
            GameManager.instance.onReset += ResetPosition;
        }
    }

    protected void OnDestroy()
    {
        var levelModifiers = LevelManager.instance.GetLevelModifiers();
        if (!levelModifiers.ContainsKey(LevelModifier.MakeBallDangerousAfterHitting) || levelModifiers[LevelModifier.MakeBallDangerousAfterHitting] == 0)
        {
            GameManager.instance.onReset -= ResetPosition;
        }
    }

    protected void ResetPosition()
    {
        transform.position = startPosition;
    }
}
