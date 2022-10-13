using UnityEngine;

public class GravitySimulation : MonoBehaviour
{
    private void FixedUpdate()
    {
        foreach (Body body in Gravity.Instance.Bodies)
        {
            body.UpdateVelocity(CalculateGravityAcceleration(body), Time.fixedDeltaTime);
        }

        Gravity.Instance.ReferenceBody.UpdatePosition(Time.fixedDeltaTime);

        foreach(Body body in Gravity.Instance.Bodies)
        {
            if (!body.Equals(Gravity.Instance.ReferenceBody))
            {
                body.UpdatePosition(Time.fixedDeltaTime);
            }
        }
    }

    private Vector3 CalculateGravityAcceleration(Body thisBody)
    {
        Vector3 acceleration = Vector3.zero;

        foreach (Body otherBody in Gravity.Instance.Bodies)
        {
            if (!thisBody.Equals(otherBody))
            {
                Vector3 direction = otherBody.Position - thisBody.Position;
                float magnitude = Gravity.Instance.G * otherBody.Mass / Vector3.SqrMagnitude(direction);

                acceleration += direction.normalized * magnitude;
            }
        }

        return acceleration;
    }
}
