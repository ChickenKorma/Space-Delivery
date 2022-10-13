using UnityEngine;

public class OrbitDraw : MonoBehaviour
{
    [SerializeField] private bool draw;

    [SerializeField] private float totalTime;
    [SerializeField] private float timeStep;

    private int steps;

    private VirtualBody[] vBodies;

    private VirtualBody referenceBody;

    private void Awake()
    {
        steps = (int) (totalTime / timeStep);

        vBodies = new VirtualBody[Gravity.Instance.Bodies.Length];

        for (int i = 0; i < vBodies.Length; i++)
        {
            Body body = Gravity.Instance.Bodies[i];

            bool draw = !body.Equals(Gravity.Instance.ReferenceBody);

            vBodies[i] = new VirtualBody(body, steps, draw);

            if (body.Equals(Gravity.Instance.ReferenceBody))
            {
                referenceBody = vBodies[i];
            }
        }
    }

    private void FixedUpdate()
    {
        if (draw)
        {
            DrawPaths();

            draw = false;
        }
    }

    private void DrawPaths()
    {
        foreach (VirtualBody vBody in vBodies)
        {
            vBody.Reset();
        }

        for (int step = 0; step < steps; step++)
        {
            foreach(VirtualBody vBody in vBodies)
            {
                vBody.UpdateVelocity(CalculateGravityAcceleration(vBody), timeStep);
            }

            referenceBody.UpdatePosition(referenceBody, timeStep, step);

            foreach (VirtualBody vBody in vBodies)
            {
                if (!vBody.Equals(referenceBody))
                {
                    vBody.UpdatePosition(referenceBody, timeStep, step);
                }
            }
        }

        foreach (VirtualBody vBody in vBodies)
        {
            vBody.DrawPath();
        }
    }

    private Vector3 CalculateGravityAcceleration(VirtualBody thisBody)
    {
        Vector3 acceleration = Vector3.zero;

        foreach (VirtualBody otherBody in vBodies)
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

class VirtualBody
{
    private Body body;

    private Vector3 position;

    public Vector3 Position { get { return position; } }

    private Vector3 velocity;

    private Vector3[] orbitPoints;

    private float mass;

    public float Mass { get { return mass; } }

    private LineRenderer lineRenderer;

    private bool draw;

    public VirtualBody(Body body, int steps, bool draw)
    {
        this.body = body;

        lineRenderer = body.GetComponent<LineRenderer>();

        orbitPoints = new Vector3[steps];
        lineRenderer.positionCount = orbitPoints.Length;

        this.draw = draw;
    }

    public void Reset()
    {
        position = body.Position;
        velocity = body.Velocity;

        mass = body.Mass;
    }

    public void UpdateVelocity(Vector3 acceleration, float deltaTime) => velocity += acceleration * deltaTime;

    public void UpdatePosition(VirtualBody referenceBody, float deltaTime, int drawStep)
    {
        position += velocity * deltaTime;

        orbitPoints[drawStep] = position - referenceBody.Position;
    }

    public void DrawPath()
    {
        if (draw)
        {
            lineRenderer.SetPositions(orbitPoints);
        }
    }
}
