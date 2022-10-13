using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    private PlayerInputActions playerInputActions;

    public Action<Vector3> thrustEvent = delegate { };

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
    }

    private void OnDisable()
    {
        playerInputActions.Game.Thrust.started -= OnThrust;
        playerInputActions.Game.Thrust.performed -= OnThrust;
        playerInputActions.Game.Thrust.canceled -= OnThrust;

        playerInputActions.Disable();
    }

    private void OnThrust(InputAction.CallbackContext context)
    {
        thrustEvent.Invoke(context.ReadValue<Vector3>());
    }
}
