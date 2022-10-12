using UnityEngine;

public class SmallObject : MonoBehaviour
{
    [SerializeField] private float gravConst;

    [SerializeField] private static CelestialObject[] celestialObjects;

    protected virtual void Awake()
    {
        if(celestialObjects == null)
        {
            celestialObjects = FindObjectsOfType<CelestialObject>();
        }
    }

    // Calculates and returns the acceleration on the object due to gravity of planets
    protected Vector3 GetGravityAcceleration()
    {
        Vector3 acceleration = Vector3.zero;

        foreach(CelestialObject obj in celestialObjects)
        {
            Vector3 gravDirection = obj.transform.position - transform.position;
            float gravMag = gravConst * obj.Mass / Vector3.SqrMagnitude(gravDirection);

            Vector3 gravAcc = gravMag * gravDirection;
            acceleration += gravAcc;
        }

        return acceleration;
    }
}
