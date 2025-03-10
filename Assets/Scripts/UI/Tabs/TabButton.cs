using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TabButton : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    public TabButtonState state = TabButtonState.NotSelectedNotHovered;

    [Header("References to objects")]
    [SerializeField] private Image _image;
    [SerializeField] private TabsController _tabsController;

    [Header("Colors")]
    [SerializeField] private Color _colorNotSelectedNotHovered = Color.white;
    [SerializeField] private Color _colorHover = new Color(1f, 1f, 0.5f);
    [SerializeField] private Color _colorSelect = Color.yellow;

    public void SetTabController(TabsController tabsController)
    {
        _tabsController = tabsController;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnHover();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnSelect();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnMouseLeave();
    }

    public void OnHover()
    {
        // Change color, if tab is not selected
        if (state == TabButtonState.NotSelectedNotHovered)
        {
            state = TabButtonState.NotSelectedHovered;
            _image.color = _colorHover;
        }

        if (_tabsController != null)
        {
            _tabsController.ResetInactiveTabs(this);
        }
    }

    public void OnSelect()
    {
        state = TabButtonState.Selected;
        _image.color = _colorSelect;

        _tabsController.OnTabSelect(this);

        if (_tabsController != null)
        {
            _tabsController.ResetInactiveTabs(this);
        }
    }

    public void OnMouseLeave()
    {
        if (state != TabButtonState.Selected)
        {
            state = TabButtonState.NotSelectedNotHovered;
            _image.color = _colorNotSelectedNotHovered;
        }
    }

    public void Deselect()
    {
        state = TabButtonState.NotSelectedNotHovered;
        _image.color = _colorNotSelectedNotHovered;
    }
}
