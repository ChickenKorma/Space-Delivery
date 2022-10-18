using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    private PlayerInputActions playerInputActions;

    public Action<Vector3> thrustEvent = delegate { };

    public Action<Vector2> rotateEvent = delegate { };

    public Action<float> rollEvent = delegate { };

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void OnEnable()
    {
        playerInputActions = new PlayerInputActions();
        playerInputActions.Enable();

        playerInputActions.Game.Thrust.started += OnThrust;
        playerInputActions.Game.Thrust.performed += OnThrust;
        playerInputActions.Game.Thrust.canceled += OnThrust;

        playerInputActions.Game.Rotate.started += OnRotate;
        playerInputActions.Game.Rotate.performed += OnRotate;
        playerInputActions.Game.Rotate.canceled += OnRotate;

        playerInputActions.Game.Roll.started += OnRoll;
        playerInputActions.Game.Roll.performed += OnRoll;
        playerInputActions.Game.Roll.canceled += OnRoll;
    }

    private void OnDisable()
    {
        playerInputActions.Game.Thrust.started -= OnThrust;
        playerInputActions.Game.Thrust.performed -= OnThrust;
        playerInputActions.Game.Thrust.canceled -= OnThrust;

        playerInputActions.Game.Rotate.started -= OnRotate;
        playerInputActions.Game.Rotate.performed -= OnRotate;
        playerInputActions.Game.Rotate.canceled -= OnRotate;

        playerInputActions.Game.Roll.started -= OnRoll;
        playerInputActions.Game.Roll.performed -= OnRoll;
        playerInputActions.Game.Roll.canceled -= OnRoll;

        playerInputActions.Disable();
    }

    private void OnThrust(InputAction.CallbackContext context)
    {
        thrustEvent.Invoke(context.ReadValue<Vector3>());
    }

    private void OnRotate(InputAction.CallbackContext context)
    {
        rotateEvent.Invoke(context.ReadValue<Vector2>());
    }

    private void OnRoll(InputAction.CallbackContext context)
    {
        rollEvent.Invoke(context.ReadValue<float>());
    }
}
