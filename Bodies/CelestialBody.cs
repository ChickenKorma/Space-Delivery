using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(LineRenderer))]
public class CelestialBody : MonoBehaviour
{
    [SerializeField] private float surfaceGravity;

    public float Radius { get { return transform.localScale.x; } }

    public float Mass { get { return rb.mass; } }

    [SerializeField] private Vector3 initialVelocity;
    [SerializeField] private Vector3 initialAngularVelocity;

    private Vector3 velocity;

    public Vector3 Velocity { get { return velocity; } }

    public Vector3 Position { get { return rb.position; } }

    protected Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        rb.mass = surfaceGravity * Radius * Radius / Gravity.Instance.G;

        velocity = initialVelocity;

        rb.angularVelocity = initialAngularVelocity * Mathf.Deg2Rad;
    }

    public virtual void UpdateVelocity(Vector3 acceleration) => velocity += acceleration * Time.fixedDeltaTime;

    public virtual void UpdatePosition() => rb.MovePosition(rb.position + (velocity * Time.fixedDeltaTime));
}
