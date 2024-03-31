using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarTriggerBall : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.Log($"StarTrigger: OnTriggerEnter2D(): begin");
        EventsManager.changeBallBehavior.Invoke();
        Destroy(this.gameObject);
    }
}
