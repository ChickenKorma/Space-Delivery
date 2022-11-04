using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(LineRenderer))]
public class CelestialBody : MonoBehaviour
{
    protected Rigidbody _rb;

    [Header("Body Data")]
    [SerializeField] private float _surfaceGravity;

    public float Radius { get { return transform.localScale.x / 2; } }
    public float Mass { get { return _rb.mass; } }

    [Header("Body State")]
    [SerializeField] private Vector3 _initialVelocity;
    [SerializeField] private Vector3 _initialAngularVelocity;

    public Vector3 Velocity { get; private set; }

    public Vector3 Position { get { return _rb.position; } }
    

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();

        // Find and set the mass needed for the given surface gravity at the body's radius
        _rb.mass = _surfaceGravity * Radius * Radius / Gravity.Instance.G;

        _rb.angularVelocity = _initialAngularVelocity * Mathf.Deg2Rad;

        Velocity = _initialVelocity;
    }

    public virtual void UpdateVelocity(Vector3 acceleration) => Velocity += acceleration * Time.fixedDeltaTime;

    public virtual void UpdatePosition() => _rb.MovePosition(Position + (Velocity * Time.fixedDeltaTime));
}
