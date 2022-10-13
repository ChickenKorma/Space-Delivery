using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(LineRenderer))]
public class Body : MonoBehaviour
{
    [SerializeField] private float mass;

    public float Mass { get { return mass; } }

    [SerializeField] private Vector3 initialVelocity;

    protected Vector3 velocity;

    public Vector3 Velocity { get { return velocity; } }

    private Vector3 position;

    public Vector3 Position { get { return position; } }

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.mass = mass;

        position = rb.position;

        velocity = initialVelocity;
    }

    public virtual void UpdateVelocity(Vector3 acceleration, float deltaTime) => velocity += acceleration * deltaTime;

    public void UpdatePosition(float deltaTime)
    {
        position += velocity * deltaTime;

        rb.MovePosition(position - Gravity.Instance.ReferenceBody.Position);
    }
}
