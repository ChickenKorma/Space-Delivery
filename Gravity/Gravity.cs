using System.Collections.Generic;
using UnityEngine;

public class Gravity : MonoBehaviour
{
    public static Gravity Instance;

    [SerializeField] Rigidbody referenceBodyRB;
    public Rigidbody ReferenceBodyRB { get { return referenceBodyRB; } }

    public CelestialBody[] Bodies { get; private set; }

    public List<Rigidbody> Objects { get; private set; }

    [SerializeField] private float _g;

    public float G { get { return _g; } }

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

        Bodies = FindObjectsOfType<CelestialBody>();

        Objects = new();

        foreach(CelestialBody body in Bodies)
        {
            Objects.Add(body.GetComponent<Rigidbody>());
        }

        Spacecraft[] spacecrafts = FindObjectsOfType<Spacecraft>();

        foreach(Spacecraft spacecraft in spacecrafts)
        {
            Objects.Add(spacecraft.GetComponent<Rigidbody>());
        }
    }
}
