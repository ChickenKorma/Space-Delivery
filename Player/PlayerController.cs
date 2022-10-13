using UnityEngine;

public class PlayerController : Spacecraft
{
    private void OnEnable()
    {
        InputManager.Instance.thrustEvent += OnThrustInput;
    }

    private void OnDisable()
    {
        InputManager.Instance.thrustEvent -= OnThrustInput;
    }

    private void OnThrustInput(Vector3 input) => currentThrust = input;
}
