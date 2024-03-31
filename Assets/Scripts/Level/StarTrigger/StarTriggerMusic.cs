using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarTriggerMusic : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        EventsManager.changeMusicTrack.Invoke();
        Destroy(this.gameObject);
    }
}