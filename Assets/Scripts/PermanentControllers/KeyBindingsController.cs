using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class KeyBindingsController : MonoBehaviour
{
    private ParametersKeyBindings _parametersKeyBindings;

    public KeyCode KeyForLeftPlayerUp { get { return _parametersKeyBindings.KeyForLeftPlayerUp; }}
    public KeyCode KeyForLeftPlayerDown { get { return _parametersKeyBindings.KeyForLeftPlayerDown; }}
    public KeyCode KeyForRightPlayerUp { get { return _parametersKeyBindings.KeyForRightPlayerUp; }}
    public KeyCode KeyForRightPlayerDown { get { return _parametersKeyBindings.KeyForRightPlayerDown; }}

    [Inject]
    private void Construct()
    {
        _parametersKeyBindings = new ParametersKeyBindings();
    }

    public KeyCode GetKeyOfInputAction(InputAction inputAction)
    {
        switch (inputAction)
        {
            case InputAction.LeftPlayerMoveUp:
                return KeyForLeftPlayerUp;
            case InputAction.LeftPlayerMoveDown:
                return KeyForLeftPlayerDown;
            case InputAction.RightPlayerMoveUp:
                return KeyForRightPlayerUp;
            case InputAction.RightPlayerMoveDown:
                return KeyForRightPlayerDown;
            default:
                Debug.LogError("KeyBindingsController: GetKeyOfInputAction: unexpected " +
                    $"inputAction={inputAction}");
                return KeyCode.None;
        }
    }

    public void SetNewKeyForInputAction(InputAction inputAction, KeyCode newKey)
    {
        switch (inputAction)
        {
            case InputAction.LeftPlayerMoveUp:
                _parametersKeyBindings.SetNewKeyForLeftPlayerUp(newKey);
                break;
            case InputAction.LeftPlayerMoveDown:
                _parametersKeyBindings.SetNewKeyForLeftPlayerDown(newKey);
                break;
            case InputAction.RightPlayerMoveUp:
                _parametersKeyBindings.SetNewKeyForRightPlayerUp(newKey);
                break;
            case InputAction.RightPlayerMoveDown:
                _parametersKeyBindings.SetNewKeyForRightPlayerDown(newKey);
                break;
            default:
                Debug.LogError("KeyBindingsController: SetNewKeyForInputAction: unexpected " +
                    $"inputAction={inputAction}");
                break;
        }
    }
}
