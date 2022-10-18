using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(LineRenderer))]
public class Body : MonoBehaviour
{
    public float Mass { get { return rb.mass; } }

    [SerializeField] private Vector3 initialVelocity;
    [SerializeField] private Vector3 initialAngularVelocity;

    public Vector3 Velocity { get { return rb.velocity; } }

    public Vector3 Position { get { return rb.position; } }

    protected Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        rb.velocity = initialVelocity;

        rb.angularVelocity = initialAngularVelocity * Mathf.Deg2Rad;
    }

    public virtual void UpdateVelocity(Vector3 acceleration, float deltaTime) => rb.velocity += acceleration * deltaTime;
}
