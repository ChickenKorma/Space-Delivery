using UnityEngine;

public class GravitySimulation : MonoBehaviour
{
    private void FixedUpdate()
    {
        foreach (CelestialBody body in Gravity.Instance.Bodies)
        {
            body.UpdateVelocity(CalculateGravityAcceleration(body.Position, body));
        }

        // Updating positions must be done seperately for accurate gravity calculations
        foreach (CelestialBody body in Gravity.Instance.Bodies)
        {
            body.UpdatePosition();
        }
    }

    // Calculates and returns the acceleration at 'position' due to gravity from all bodies except the given 'ignoreBody'
    public static Vector3 CalculateGravityAcceleration(Vector3 position, CelestialBody ignoreBody = null)
    {
        Vector3 acceleration = Vector3.zero;

        foreach (CelestialBody body in Gravity.Instance.Bodies)
        {
            if (body != ignoreBody)
            {
                acceleration += CalculateBodyAcceleration(position, body);
            }
        }   

        return acceleration;
    }

    // Calculates and returns the acceleration at 'position' due to gravity from 'body'
    public static Vector3 CalculateBodyAcceleration(Vector3 position, CelestialBody body)
    {
        Vector3 direction = body.Position - position;
        float magnitude = Gravity.Instance.G * body.Mass / Vector3.SqrMagnitude(direction);

        return direction.normalized * magnitude;
    }
}
