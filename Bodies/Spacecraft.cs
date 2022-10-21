using UnityEngine;

public class Spacecraft : MonoBehaviour
{
    [SerializeField] private float thrustStrength;
    [SerializeField] private float rotateSpeed;

    protected Vector3 thrustAcc;
    private Vector3 matchingAcc;
    private Vector3 totalEngineAcc;

    protected Vector3 torqueAcc;

    private Rigidbody rb;

    public bool autoAlignActive { get; protected set; }

    private static float minAutoAlignDist = 300f;

    public CelestialBody target { get; private set; }
    public CelestialBody lockedTarget { get; private set; }

    private static float maxTargetAngle = 70f;

    public bool matchVelocityActive { get; protected set; }

    [SerializeField] private ParticleSystem[] xPosEngines;
    [SerializeField] private ParticleSystem[] xNegEngines;
    [SerializeField] private ParticleSystem[] yPosEngines;
    [SerializeField] private ParticleSystem[] yNegEngines;
    [SerializeField] private ParticleSystem[] zPosEngines;
    [SerializeField] private ParticleSystem[] zNegEngines;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        autoAlignActive = false;
    }

    private void Update()
    {
        UpdateEngines();
    }

    private void FixedUpdate()
    {
        CelestialBody nearestBody = GetNearestBody();

        if (autoAlignActive && nearestBody != null)
        {
            AutoAlign(nearestBody);
        }
        else
        {
            UpdateRotation();
        }

        target = GetViewedBody();

        UpdateMatchingAcceleration();

        UpdatePhysics();
    }

    private void UpdatePhysics()
    {
        totalEngineAcc = ClampAxes(matchingAcc + (rb.rotation * thrustAcc), 1f);

        Vector3 totalAcceleration = GravitySimulation.CalculateGravityAcceleration(rb.position) + totalEngineAcc * thrustStrength;

        rb.AddForce(totalAcceleration, ForceMode.Acceleration);
    }

    private void UpdateRotation()
    {
        Quaternion deltaRotation = Quaternion.Euler(torqueAcc * rotateSpeed * Time.fixedDeltaTime);

        rb.MoveRotation(rb.rotation * deltaRotation);
    } 

    private void AutoAlign(CelestialBody nearestBody)
    {
        Vector3 bodyUp = rb.position - nearestBody.Position;
        Vector3 spacecraftUp = transform.up;

        Quaternion alignRotation = Quaternion.FromToRotation(spacecraftUp, bodyUp);

        rb.MoveRotation(Quaternion.Lerp(rb.rotation, rb.rotation * alignRotation, rotateSpeed * Time.fixedDeltaTime / 50));
    }

    private void UpdateMatchingAcceleration()
    {
        if (matchVelocityActive && lockedTarget != null)
        {
            Vector3 relativeVelocity = lockedTarget.Velocity - rb.velocity;

            Vector3 gravityAcc = GravitySimulation.CalculateBodyAcceleration(rb.position, lockedTarget);

            matchingAcc = (relativeVelocity * Mathf.Pow((1 + relativeVelocity.magnitude), 2)) - gravityAcc;
        }
        else
        {
            matchingAcc = Vector3.zero;
        }   
    }

    private void UpdateEngines()
    {
        Vector3 engineThrustLocal = Quaternion.Inverse(rb.rotation) * totalEngineAcc;

        UpdateEngineDirection(xPosEngines, xNegEngines, engineThrustLocal.x);
        UpdateEngineDirection(yPosEngines, yNegEngines, engineThrustLocal.y);
        UpdateEngineDirection(zPosEngines, zNegEngines, engineThrustLocal.z);
    }

    private void UpdateEngineDirection(ParticleSystem[] posEngines, ParticleSystem[] negEngines, float thrust)
    {
        if (thrust > 0.0001f)
        {
            SetLifetimes(posEngines, thrust);
            ToggleEngines(posEngines, true);

            ToggleEngines(negEngines, false);
        }
        else if (thrust < -0.0001f)
        {
            ToggleEngines(posEngines, false);

            SetLifetimes(negEngines, -thrust);
            ToggleEngines(negEngines, true);
        }
        else
        {
            ToggleEngines(posEngines, false);
            ToggleEngines(negEngines, false);
        }
    }

    private void ToggleEngines(ParticleSystem[] engines, bool turnOn)
    {
        foreach(ParticleSystem engine in engines)
        {
            if (turnOn)
            {
                engine.Play();
            }
            else
            {
                engine.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            }
        }
    }

    private void SetLifetimes(ParticleSystem[] engines, float lifetime)
    {
        foreach(ParticleSystem engine in engines)
        {
            ParticleSystem.MainModule main = engine.main;

            main.startLifetime = lifetime;
        }
    }

    private CelestialBody GetNearestBody()
    {
        CelestialBody nearestBody = null;

        float nearestSqrDist = Mathf.Infinity;

        foreach (CelestialBody body in Gravity.Instance.Bodies)
        {
            float sqrDist = Vector3.SqrMagnitude(rb.position - body.Position);

            if (sqrDist < nearestSqrDist && sqrDist < Mathf.Pow(minAutoAlignDist + (body.Radius / 2), 2))
            {
                nearestSqrDist = sqrDist;
                nearestBody = body;
            }
        }

        return nearestBody;
    }

    private CelestialBody GetViewedBody()
    {
        CelestialBody viewedBody = null;

        float smallestAngle = Mathf.Infinity;

        foreach (CelestialBody body in Gravity.Instance.Bodies)
        {
            float angle = AngleToBody(body);

            if (angle < smallestAngle && angle < maxTargetAngle)
            {
                smallestAngle = angle;
                viewedBody = body;
            }
        }

        return viewedBody;
    }

    private float AngleToBody(CelestialBody body)
    {
        Vector3 direction = body.Position - rb.position;

        return Vector3.Angle(transform.forward, direction);
    }

    protected void AutoAlignToggle()
    {
        autoAlignActive = !autoAlignActive;
    }

    protected void TryLockTarget()
    {
        if(target == lockedTarget)
        {
            lockedTarget = null;
        }
        else
        {
            lockedTarget = target;
        }
    }

    private Vector3 ClampAxes(Vector3 vector, float maxValue)
    {
        float x = Mathf.Clamp(vector.x, -maxValue, maxValue);
        float y = Mathf.Clamp(vector.y, -maxValue, maxValue);
        float z = Mathf.Clamp(vector.z, -maxValue, maxValue);

        return new Vector3(x, y, z);
    }
}
