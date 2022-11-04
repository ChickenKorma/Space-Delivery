using UnityEngine;

public class OrbitDraw : MonoBehaviour
{
    [SerializeField] private float _totalTime;
    [SerializeField] private float _timeStep;

    private int _steps;

    private VirtualBody[] _vBodies;

    private VirtualBody _vReferenceBody;

    private void Awake()
    {
        // Calculate total number of steps in the simulation
        _steps = (int) (_totalTime / _timeStep);

        _vBodies = new VirtualBody[Gravity.Instance.Bodies.Length];

        for (int i = 0; i < _vBodies.Length; i++)
        {
            CelestialBody body = Gravity.Instance.Bodies[i];

            bool isReferenceBody = body.Equals(Gravity.Instance.ReferenceBody);

            _vBodies[i] = new VirtualBody(body, _steps, !isReferenceBody);

            if (isReferenceBody)
            {
                _vReferenceBody = _vBodies[i];
            }
        }
    }

    private void Start()
    {
        DrawOrbits();
    }

    // Draws the orbit paths of each celestial body
    private void DrawOrbits()
    {
        foreach (VirtualBody vBody in _vBodies)
        {
            vBody.Reset();
        }

        for (int step = 0; step < _steps; step++)
        {
            foreach(VirtualBody vBody in _vBodies)
            {
                vBody.UpdateVelocity(CalculateGravityAcceleration(vBody), _timeStep);
            }

            // Updates reference body position first so all other relative positions are accurate
            _vReferenceBody.UpdatePosition(_vReferenceBody, _timeStep, step);

            foreach (VirtualBody vBody in _vBodies)
            {
                if (!vBody.Equals(_vReferenceBody))
                {
                    vBody.UpdatePosition(_vReferenceBody, _timeStep, step);
                }
            }
        }

        foreach (VirtualBody vBody in _vBodies)
        {
            vBody.DrawPath();
        }
    }

    // Calculate the acceleration of 'thisBody' due to gravity by all other bodies
    private Vector3 CalculateGravityAcceleration(VirtualBody thisBody)
    {
        Vector3 acceleration = Vector3.zero;

        foreach (VirtualBody otherBody in _vBodies)
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

// Constructed from a celestial body, stores state of body for virtual simulation and draws positions via line renderer
class VirtualBody
{
    private readonly CelestialBody _celestialBody;

    public Vector3 Position { get; private set; }

    private Vector3 _velocity;

    private Vector3[] _orbitPoints;

    public float Mass { get; private set; }

    private readonly LineRenderer _lineRenderer;

    private readonly bool _draw;

    public VirtualBody(CelestialBody celestialBody, int steps, bool draw)
    {
        _celestialBody = celestialBody;

        _lineRenderer = _celestialBody.GetComponent<LineRenderer>();

        _orbitPoints = new Vector3[steps];
        _lineRenderer.positionCount = _orbitPoints.Length;

        _draw = draw;
    }

    // Sets virtual body to equal current state of celestial body
    public void Reset()
    {
        Position = _celestialBody.Position;
        _velocity = _celestialBody.Velocity;

        Mass = _celestialBody.Mass;
    }

    public void UpdateVelocity(Vector3 acceleration, float deltaTime) => _velocity += acceleration * deltaTime;

    public void UpdatePosition(VirtualBody vReferenceBody, float deltaTime, int drawStep)
    {
        Position += _velocity * deltaTime;

        // Makes position relative to reference body
        _orbitPoints[drawStep] = Position - vReferenceBody.Position;
    }

    // Sets the points of the line renderer equal to the stored positions if drawing is enabled for this body
    public void DrawPath()
    {
        if (_draw)
        {
            _lineRenderer.SetPositions(_orbitPoints);
        }
    }
}
