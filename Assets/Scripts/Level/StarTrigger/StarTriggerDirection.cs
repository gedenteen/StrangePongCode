using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarTriggerDirection : StarTrigger
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsReady)
            return;

        //Debug.Log($"StarTrigger: OnTriggerEnter2D(): begin");
        EventsManager.changeBallDirection.Invoke();
        Destroy(this.gameObject);
    }
}
