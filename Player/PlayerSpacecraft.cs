using UnityEngine;

public class PlayerSpacecraft : Spacecraft
{
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnEnable()
    {
        InputManager.Instance.thrustEvent += OnThrustInput;
        InputManager.Instance.rotateEvent += OnRotateInput;
        InputManager.Instance.rollEvent += OnRollInput;
        InputManager.Instance.autoAlignEvent += OnAutoAlignToggle;
        InputManager.Instance.targetLockEvent += OnTargetLock;
        InputManager.Instance.matchVelocityEvent += OnMatchVelocity;
    }

    private void OnDisable()
    {
        InputManager.Instance.thrustEvent -= OnThrustInput;
        InputManager.Instance.rotateEvent -= OnRotateInput;
        InputManager.Instance.rollEvent -= OnRollInput;
        InputManager.Instance.autoAlignEvent += OnAutoAlignToggle;
        InputManager.Instance.targetLockEvent -= OnTargetLock;
        InputManager.Instance.matchVelocityEvent -= OnMatchVelocity;
    }

    private void OnThrustInput(Vector3 input) => thrustAcc = input;

    private void OnRotateInput(Vector2 input) => torqueAcc = new Vector3(input.y, input.x, torqueAcc.z);

    private void OnRollInput(float input) => torqueAcc = new Vector3(torqueAcc.x, torqueAcc.y, input);

    private void OnAutoAlignToggle() => AutoAlignToggle();

    private void OnTargetLock() => TryLockTarget();

    private void OnMatchVelocity(bool state) => matchVelocityActive = state;
}
