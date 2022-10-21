using System.Collections.Generic;
using UnityEngine;

public class Gravity : MonoBehaviour
{
    public static Gravity Instance;

    [SerializeField] private float g;

    public float G { get { return g; } }

    [SerializeField] private Transform referenceBody;

    public Transform ReferenceBody { get { return referenceBody; } }

    private CelestialBody[] bodies;

    public CelestialBody[] Bodies { get { return bodies; } }

    private List<Transform> objects = new();

    public List<Transform> Objects { get { return objects; } }

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        bodies = FindObjectsOfType<CelestialBody>();

        foreach(CelestialBody body in bodies)
        {
            objects.Add(body.transform);
        }

        Spacecraft[] spacecrafts = FindObjectsOfType<Spacecraft>();

        foreach(Spacecraft spacecraft in spacecrafts)
        {
            objects.Add(spacecraft.transform);
        }
    }
}
