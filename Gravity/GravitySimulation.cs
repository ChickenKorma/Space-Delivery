using UnityEngine;

public class GravitySimulation : MonoBehaviour
{
    private void FixedUpdate()
    {
        foreach (CelestialBody body in Gravity.Instance.Bodies)
        {
            body.UpdateVelocity(CalculateGravityAcceleration(body.Position, body));
        }

        foreach (CelestialBody body in Gravity.Instance.Bodies)
        {
            body.UpdatePosition();
        }
    }

    private void LateUpdate()
    {
        Vector3 origin = Gravity.Instance.ReferenceBody.position;

        foreach(Transform obj in Gravity.Instance.Objects)
        {
            obj.position -= origin;
        }
    }

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

    public static Vector3 CalculateBodyAcceleration(Vector3 position, CelestialBody body)
    {
        Vector3 direction = body.Position - position;
        float magnitude = Gravity.Instance.G * body.Mass / Vector3.SqrMagnitude(direction);

        return direction.normalized * magnitude;
    }
}
