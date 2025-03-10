using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class InputKeyAssigner : MonoBehaviour
{
    [SerializeField] InputAction _inputAction;
    [SerializeField] TextMeshProUGUI _textMeshForKey;
    [SerializeField] Button _buttonChangekey;

    [Inject] KeyBindingsController _keyBindingsController;

    private bool _shouldChangeKey = false;

    private void Awake()
    {
        _buttonChangekey.onClick.AddListener(ChangeKey);
    }

    private void OnEnable()
    {
        ShowKeyCode(_keyBindingsController.GetKeyOfInputAction(_inputAction));
    }

    private void OnDestroy()
    {
        _buttonChangekey.onClick.RemoveListener(ChangeKey);
    }

    private void Update()
    {
        if (!_shouldChangeKey)
        {
            return;
        }

        if (Input.anyKeyDown)
        {
            foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(key))
                {
                    Debug.Log("Key pressed: " + key);
                    ShowKeyCode(key);
                    _keyBindingsController.SetNewKeyForInputAction(_inputAction, key);
                    _shouldChangeKey = false;
                }
            }
        }
    }

    private void ShowKeyCode(KeyCode keyCode)
    {
        string symbol;
        switch (keyCode)
        {
            case KeyCode.UpArrow:
                symbol = "\u2191";
                break;
            case KeyCode.DownArrow:
                symbol = "\u2193";
                break;
            case KeyCode.LeftArrow:
                symbol = "\u2190";
                break;
            case KeyCode.RightArrow:
                symbol = "\u2192";
                break;
            default:
                symbol = keyCode.ToString();
                break;
        }

        _textMeshForKey.text = symbol;
    }

    private void ChangeKey()
    {
        _shouldChangeKey = true;
        _textMeshForKey.text = "";
    }
}
