using UnityEngine;

public class CelestialObject : MonoBehaviour
{
    [SerializeField] private float mass;

    public float Mass { get { return mass; } }
}
