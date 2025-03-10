using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Image))]
public class BlackoutForUi : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image _imageBlackout;
    [SerializeField] private Image _myImage;

    private void OnValidate()
    {
        _myImage ??= GetComponent<Image>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Debug.Log($"Cursor entered the UI element: {gameObject.name}");
        _imageBlackout.gameObject.SetActive(false);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Debug.Log($"Cursor exited the UI element: {gameObject.name}");
        _imageBlackout.gameObject.SetActive(true);
    }
}
