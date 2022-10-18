using UnityEngine;

public class PlayerController : Spacecraft
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
    }

    private void OnDisable()
    {
        InputManager.Instance.thrustEvent -= OnThrustInput;
        InputManager.Instance.rotateEvent -= OnRotateInput;
        InputManager.Instance.rollEvent -= OnRollInput;
    }

    private void OnThrustInput(Vector3 input) => currentThrust = input;

    private void OnRotateInput(Vector2 input) => currentRotate = new Vector3(input.y, input.x, currentRotate.z);

    private void OnRollInput(float input) => currentRotate = new Vector3(currentRotate.x, currentRotate.y, input);
}
