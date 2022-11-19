using UnityEngine;

public class Spacecraft : MonoBehaviour
{
    private Rigidbody _rb;

    public Vector3 Velocity { get { return _rb.velocity; } }

    private Vector3 _totalEngineAcc;
    protected Vector3 _thrustAcc;
    private Vector3 _matchingAcc;

    public Vector3 RelativeVelocity { get; private set; }

    protected Vector3 _torqueAcc;

    public CelestialBody Target { get; private set; }
    public CelestialBody LockedTarget { get; private set; }

    public bool AutoAlignActive { get; protected set; }
    public bool MatchVelocityActive { get; protected set; }

    [Header("Spacecraft Stats")]
    [SerializeField] private float _thrustStrength;
    [SerializeField] private float _rotateSpeed;

    private static readonly float _minAutoAlignDist = 300f;
    private static readonly float _maxTargetAngle = 70f;

    [Header("Engine Particle Systems")]
    [SerializeField] private ParticleSystem[] _xPosEnginePSs;
    [SerializeField] private ParticleSystem[] _xNegEnginePSs;
    [SerializeField] private ParticleSystem[] _yPosEnginePSs;
    [SerializeField] private ParticleSystem[] _yNegEnginePSs;
    [SerializeField] private ParticleSystem[] _zPosEnginePSs;
    [SerializeField] private ParticleSystem[] _zNegEnginePSs;

    private Camera _cam;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();

        AutoAlignActive = false;

        _cam = Camera.main;
    }

    private void Update()
    {
        UpdateEngines();
    }

    private void FixedUpdate()
    {
        CelestialBody nearestBody = GetNearestBody();

        if (AutoAlignActive && nearestBody != null)
        {
            AutoAlign(nearestBody);
        }
        else
        {
            UpdateRotation();
        }

        Target = GetViewedBody();

        UpdateRelativeVelocity();

        UpdatePhysics();
    }

    // Calculates and applies the total acceleration to the attatched rigidbody, sum of the engien thrust and gravity
    private void UpdatePhysics()
    {
        _totalEngineAcc = _thrustAcc - (MatchVelocityActive ? RelativeVelocity : Vector3.zero);
        _totalEngineAcc = _rb.rotation * ClampAxes(_totalEngineAcc, 1f);

        Vector3 totalAcceleration = GravitySimulation.CalculateGravityAcceleration(_rb.position) + _totalEngineAcc * _thrustStrength;

        _rb.AddForce(totalAcceleration, ForceMode.Acceleration);
    }

    // Rotates attatched rigidbody by the current torque and spacecraft rotation speed
    private void UpdateRotation()
    {
        Quaternion deltaRotation = Quaternion.Euler(_torqueAcc * _rotateSpeed * Time.fixedDeltaTime);

        _rb.MoveRotation(_rb.rotation * deltaRotation);
    } 

    // Rotates attatched rigidbody to align the up direction with the direction from the nearest body to the rigidbody
    private void AutoAlign(CelestialBody nearestBody)
    {
        Vector3 bodyUp = _rb.position - nearestBody.Position;
        Vector3 spacecraftUp = transform.up;

        Quaternion alignRotation = Quaternion.FromToRotation(spacecraftUp, bodyUp);
        
        _rb.MoveRotation(Quaternion.Lerp(_rb.rotation, _rb.rotation * alignRotation, _rotateSpeed * Time.fixedDeltaTime / 50));
    }

    // Calculates the relative velocity between the 
    private void UpdateRelativeVelocity()
    {
        if(LockedTarget != null)
        {
            Vector3 forward = (LockedTarget.Position - transform.position).normalized; // pointed towards the locked target
            Vector3 right = Vector3.Cross(forward, transform.up).normalized; // perpendicular to forward direction and spacecraft up
            Vector3 up = Vector3.Cross(forward, right).normalized; // perpendicular to forward direction and spacecraft right

            Vector3 relativeVelocityWorld = LockedTarget.Velocity - Velocity;

            // Calculate the contribution to the relative velocity in each axis
            float x = Vector3.Dot(relativeVelocityWorld, right);
            float y = Vector3.Dot(relativeVelocityWorld, up);
            float z = -Vector3.Dot(relativeVelocityWorld, forward);

            RelativeVelocity = new Vector3(x, y, z);
        }
        else
        {
            RelativeVelocity = Vector3.zero;
        }
    }

    // Updates the state of all engine particle systems relative to the engine thrust strength and direction
    private void UpdateEngines()
    {
        Vector3 engineThrustLocal = Quaternion.Inverse(_rb.rotation) * _totalEngineAcc;

        UpdateEngineAxis(_xPosEnginePSs, _xNegEnginePSs, engineThrustLocal.x);
        UpdateEngineAxis(_yPosEnginePSs, _yNegEnginePSs, engineThrustLocal.y);
        UpdateEngineAxis(_zPosEnginePSs, _zNegEnginePSs, engineThrustLocal.z);
    }

    // Updates the state of all engine particle systems in one axis relative to the given axis thrust
    private void UpdateEngineAxis(ParticleSystem[] posEnginePSs, ParticleSystem[] negEnginePSs, float thrust)
    {
        if (thrust > 0.02f)
        {
            SetLifetimes(posEnginePSs, thrust);
            ToggleEngines(posEnginePSs, true);

            ToggleEngines(negEnginePSs, false);
        }
        else if (thrust < -0.02f)
        {
            ToggleEngines(posEnginePSs, false);

            SetLifetimes(negEnginePSs, -thrust);
            ToggleEngines(negEnginePSs, true);
        }
        // Engine particle systems are stopped at very small values
        else
        {
            ToggleEngines(posEnginePSs, false);
            ToggleEngines(negEnginePSs, false);
        }
    }

    // Plays or stops the given engine particle systems depending on 'turnOn'
    private void ToggleEngines(ParticleSystem[] enginePSs, bool turnOn)
    {
        foreach(ParticleSystem enginePS in enginePSs)
        {
            if (turnOn)
            {
                enginePS.Play();
            }
            else
            {
                enginePS.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            }
        }
    }

    // Sets the given engine particle systems' lifetimes
    private void SetLifetimes(ParticleSystem[] enginePSs, float lifetime)
    {
        foreach(ParticleSystem enginePS in enginePSs)
        {
            // Main module must be stored before being accessed??
            ParticleSystem.MainModule mainModule = enginePS.main;

            mainModule.startLifetime = lifetime;
        }
    }

    // Determines and returns the nearest celestial body to the attatched rigidbody
    private CelestialBody GetNearestBody()
    {
        CelestialBody nearestBody = null;

        float nearestSqrDist = Mathf.Infinity;

        foreach (CelestialBody body in Gravity.Instance.Bodies)
        {
            float sqrDist = Vector3.SqrMagnitude(_rb.position - body.Position);

            if (sqrDist < nearestSqrDist && sqrDist < Mathf.Pow(_minAutoAlignDist + (body.Radius / 2), 2))
            {
                nearestSqrDist = sqrDist;
                nearestBody = body;
            }
        }

        return nearestBody;
    }

    // Determines and returns the celestial body closest to the forward direction of the main camera
    private CelestialBody GetViewedBody()
    {
        CelestialBody viewedBody = null;

        float smallestAngle = Mathf.Infinity;

        foreach (CelestialBody body in Gravity.Instance.Bodies)
        {
            float angle = AngleToBody(body);

            if (angle < smallestAngle && angle < _maxTargetAngle)
            {
                smallestAngle = angle;
                viewedBody = body;
            }
        }

        return viewedBody;
    }

    // Calculates and returns the angle to the given celestial body from the camera forward direction
    private float AngleToBody(CelestialBody body)
    {
        Vector3 direction = body.Position - _cam.transform.position;

        return Vector3.Angle(_cam.transform.forward, direction);
    }

    protected void AutoAlignToggle()
    {
        AutoAlignActive = !AutoAlignActive;
    }

    protected void TryLockTarget()
    {
        if(Target == LockedTarget)
        {
            LockedTarget = null;
        }
        else
        {
            LockedTarget = Target;
        }
    }

    // Calculates and returns the given vector with each axis' magnitude clamped to max value
    // Unlike Vector3.ClampMagnitude the vector axes are independently clamped
    private Vector3 ClampAxes(Vector3 vector, float maxValue)
    {
        float x = Mathf.Clamp(vector.x, -maxValue, maxValue);
        float y = Mathf.Clamp(vector.y, -maxValue, maxValue);
        float z = Mathf.Clamp(vector.z, -maxValue, maxValue);

        return new Vector3(x, y, z);
    }
}
