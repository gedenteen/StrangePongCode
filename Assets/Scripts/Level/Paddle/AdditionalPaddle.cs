using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdditionalPaddle : Paddle
{
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

    public void Move(Vector2 velocity)
    {
        rb2d.velocity = velocity;
    }
}
