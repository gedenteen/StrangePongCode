using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization;
using UnityEngine.UI;

public class LevelModifierOnUi : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("References to objects")]
    [SerializeField] private Image _backgroundImage;
    [SerializeField] private TextMeshProUGUI _textMeshForDescription;

    [Header("References to assets")]
    [SerializeField] private LocalizedString _localizedDescription;

    [Header("Changeable fields")]
    [SerializeField] private Color _hoverColor = Color.gray;

    private Color _originalColor;

    private void Awake()
    {
        _originalColor = _backgroundImage.color;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _backgroundImage.color = _hoverColor;
        _textMeshForDescription.text = _localizedDescription.GetLocalizedString();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _backgroundImage.color = _originalColor;
    }
}

