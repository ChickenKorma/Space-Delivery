using UnityEngine;

public class Spacecraft : Body
{
    [SerializeField] private float thrustStrength;
    [SerializeField] private float rotateSpeed;

    protected Vector3 currentThrust;

    protected Vector3 currentRotate;


    private void FixedUpdate()
    {
        UpdateRotation(Time.fixedDeltaTime);
    }

    public override void UpdateVelocity(Vector3 acceleration, float deltaTime)
    {
        base.UpdateVelocity(acceleration, deltaTime);

        Vector3 thrust = Vector3.ClampMagnitude(currentThrust, 1f) * thrustStrength;

        base.UpdateVelocity(rb.rotation * thrust, deltaTime);
    }

    private void UpdateRotation(float deltaTime)
    {
        Quaternion deltaRotation = Quaternion.Euler(currentRotate * rotateSpeed * deltaTime);

        rb.MoveRotation(rb.rotation * deltaRotation);
    } 
}
