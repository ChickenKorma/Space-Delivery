using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    private PlayerInputActions _playerInputActions;

    public Action<Vector3> thrustEvent = delegate { };

    public Action<Vector2> rotateEvent = delegate { };

    public Action<float> rollEvent = delegate { };

    public Action autoAlignEvent = delegate { };
    public Action targetLockEvent = delegate { };

    public Action<bool> matchVelocityEvent = delegate { };

    public Action<bool> recenterCameraEvent = delegate { };

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
        _playerInputActions = new PlayerInputActions();
        _playerInputActions.Enable();

        _playerInputActions.Game.Thrust.started += OnThrust;
        _playerInputActions.Game.Thrust.performed += OnThrust;
        _playerInputActions.Game.Thrust.canceled += OnThrust;

        _playerInputActions.Game.Rotate.started += OnRotate;
        _playerInputActions.Game.Rotate.performed += OnRotate;
        _playerInputActions.Game.Rotate.canceled += OnRotate;

        _playerInputActions.Game.Roll.started += OnRoll;
        _playerInputActions.Game.Roll.performed += OnRoll;
        _playerInputActions.Game.Roll.canceled += OnRoll;

        _playerInputActions.Game.AutoAlign.performed += OnAutoAlign;

        _playerInputActions.Game.TargetLock.performed += OnTargetLock;

        _playerInputActions.Game.MatchVelocity.started += OnMatchVelocity;
        _playerInputActions.Game.MatchVelocity.performed += OnMatchVelocity;
        _playerInputActions.Game.MatchVelocity.canceled += OnMatchVelocity;

        _playerInputActions.Game.RecenterCamera.performed += OnRecenterCamera;

        _playerInputActions.Game.Look.started += OnLook;
    }

    private void OnDisable()
    {
        _playerInputActions.Game.Thrust.started -= OnThrust;
        _playerInputActions.Game.Thrust.performed -= OnThrust;
        _playerInputActions.Game.Thrust.canceled -= OnThrust;

        _playerInputActions.Game.Rotate.started -= OnRotate;
        _playerInputActions.Game.Rotate.performed -= OnRotate;
        _playerInputActions.Game.Rotate.canceled -= OnRotate;

        _playerInputActions.Game.Roll.started -= OnRoll;
        _playerInputActions.Game.Roll.performed -= OnRoll;
        _playerInputActions.Game.Roll.canceled -= OnRoll;

        _playerInputActions.Game.AutoAlign.performed -= OnAutoAlign;

        _playerInputActions.Game.TargetLock.performed -= OnTargetLock;

        _playerInputActions.Game.MatchVelocity.started -= OnMatchVelocity;
        _playerInputActions.Game.MatchVelocity.performed -= OnMatchVelocity;
        _playerInputActions.Game.MatchVelocity.canceled -= OnMatchVelocity;

        _playerInputActions.Game.RecenterCamera.performed -= OnRecenterCamera;

        _playerInputActions.Game.Look.started -= OnLook;

        _playerInputActions.Disable();
    }

    private void OnThrust(InputAction.CallbackContext context) => thrustEvent.Invoke(context.ReadValue<Vector3>());

    private void OnRotate(InputAction.CallbackContext context) => rotateEvent.Invoke(context.ReadValue<Vector2>());

    private void OnRoll(InputAction.CallbackContext context) => rollEvent.Invoke(context.ReadValue<float>());

    private void OnAutoAlign(InputAction.CallbackContext context) => autoAlignEvent.Invoke();

    private void OnTargetLock(InputAction.CallbackContext context) => targetLockEvent.Invoke();

    private void OnMatchVelocity(InputAction.CallbackContext context) => matchVelocityEvent.Invoke(!context.canceled);

    private void OnRecenterCamera(InputAction.CallbackContext context) => recenterCameraEvent.Invoke(true);

    private void OnLook(InputAction.CallbackContext context) => recenterCameraEvent.Invoke(false);
}
