using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasPermanent : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
