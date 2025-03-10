using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TabsController : MonoBehaviour
{
    [SerializeField] private TabButton[] _tabs;
    [SerializeField] private GameObject[] _objectsToSwap;

    [Header("References to child objects")]
    [SerializeField] private Transform _tabsHolder;
    [SerializeField] private Transform _contentHolder;

    [Header("Parameters")]
    [SerializeField] private int _indexOfTabForFirstActivate = 0;

    private TabButton _selectedTab = null;

    private void Start()
    {
        if (_indexOfTabForFirstActivate < 0 || _indexOfTabForFirstActivate >= _tabs.Length)
        {
            Debug.LogError($"TabsController: Start: invalud _indexOfTabForFirstActivate=" + 
                "{_indexOfTabForFirstActivate}");
            return;
        }

        _tabs[_indexOfTabForFirstActivate].OnPointerClick(null);
    }

    private void OnValidate()
    {
        // Get tabs
        _tabs = _tabsHolder.GetComponentsInChildren<TabButton>();
        for (int i = 0; i < _tabs.Length; i++)
        {
            _tabs[i].SetTabController(this);
        }

        // Get objects to swap
        int childCount = _contentHolder.transform.childCount;
        _objectsToSwap = new GameObject[childCount];
        for (int i = 0; i < childCount; i++)
        {
            Transform childTransform = _contentHolder.transform.GetChild(i);
            GameObject childObject = childTransform.gameObject;
            
            _objectsToSwap[i] = childObject;
        }

        if (_tabs.Length != _objectsToSwap.Length)
        {
            Debug.LogError($"TabsController: count of tabs ({_tabs.Length}) != count of objects to swap ({_objectsToSwap.Length})");
        }
    }

    public void OnTabSelect(TabButton tab)
    {
        _selectedTab = tab;

        int tabIndex = 0; // = tab.transform.GetSiblingIndex();
        for (int i = 0; i < _tabs.Length; i++)
        {
            if (tab == _tabs[i])
            {
                tabIndex = i;
                break;
            }
        }
        
        for (int i = 0; i < _objectsToSwap.Length; i++)
        {
            bool active = tabIndex == i;
            _objectsToSwap[i].SetActive(active);
        }
    }

    // Make all tabs inactive except for the selected tab and 
    // except for the tab that was last interacted with
    public void ResetInactiveTabs(TabButton lastActiveTab)
    {
        for (int i = 0; i < _tabs.Length; i++)
        {
            if (_tabs[i] == lastActiveTab)
            {
                continue;
            }
            if (_selectedTab != null && _selectedTab == _tabs[i])
            {
                continue;
            }
            
            _tabs[i].Deselect();
        }
    }
}
