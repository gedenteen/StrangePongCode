using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PanelSettings : MonoBehaviour
{
    public static UnityEvent<bool> eventActivate = new UnityEvent<bool>();

    [SerializeField] private GameObject myPanel;

    private void Awake()
    {
        EventsManager.levelLoaded.AddListener((int value) => myPanel.gameObject.SetActive(false));
        eventActivate.AddListener(Activate);
    }

    public void Activate(bool value)
    {
        myPanel.gameObject.SetActive(value);
        // if (value)
        // {
        //     Settings.instance.UpdateUi();
        // }
    }
}
