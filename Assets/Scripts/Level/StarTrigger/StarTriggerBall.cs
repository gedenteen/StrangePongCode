using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarTriggerBall : StarTrigger
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsReady)
            return;

        //Debug.Log($"StarTrigger: OnTriggerEnter2D(): begin");
        EventsManager.changeBallBehavior.Invoke();
        Destroy(this.gameObject);
    }
}
