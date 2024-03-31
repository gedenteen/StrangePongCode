using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.Events;

public class FpsCounter : MonoBehaviour
{
    public static UnityEvent<bool> eventActivate = new UnityEvent<bool>();

    [SerializeField] private TextMeshProUGUI textMesh;
    [SerializeField] private float delayInSeconds = 0.1f;
    [SerializeField] private bool activateOnStart = false;

    private WaitForSeconds delay;
    private float fpsCount;
    private IEnumerator coroutineShowFps;

    private void Awake()
    {
        if (textMesh == null)
        {
            Debug.LogError("FpsCounter: Awake: i have no reference to testmesh");
            Destroy(gameObject);
            return;
        }

        delay = new WaitForSeconds(delayInSeconds);
        coroutineShowFps = ShowFps();
        eventActivate.AddListener(Activate);
    }

    private void Start()
    {
        if (activateOnStart)
        {
            Activate(true);
        }
    }

    private void OnDisable()
    {
        Activate(false);
    }

    public void Activate(bool activate)
    {
        if (activate)
        {
            StartCoroutine(coroutineShowFps);
        }
        else
        {
            StopCoroutine(coroutineShowFps);
            textMesh.text = "";
        }
    }

    private IEnumerator ShowFps()
    {
        while (true)
        {
            //Debug.Log("FpsCounter: ShowFps: iteration");
            fpsCount = 1f / Time.unscaledDeltaTime;
            textMesh.text = Mathf.Round(fpsCount).ToString();
            yield return delay;
        }
    }
}