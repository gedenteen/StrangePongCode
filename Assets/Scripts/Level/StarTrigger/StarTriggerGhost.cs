using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarTriggerGhost : StarTrigger
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsReady)
            return;

        EventsManager.createGhostBall.Invoke();
        Destroy(this.gameObject);
    }
}