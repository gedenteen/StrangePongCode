using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarTriggerMusic : StarTrigger
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsReady)
            return;

        EventsManager.changeMusicTrack.Invoke();
        Destroy(this.gameObject);
    }
}