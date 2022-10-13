using UnityEngine;

public class Spacecraft : Body
{
    [SerializeField] private float thrustStrength;

    protected Vector3 currentThrust;

    public override void UpdateVelocity(Vector3 acceleration, float deltaTime)
    {
        base.UpdateVelocity(acceleration, deltaTime);

        velocity += (currentThrust * thrustStrength) * deltaTime;
    }
}
