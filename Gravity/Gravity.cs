using System.Collections.Generic;
using UnityEngine;

public class Gravity : MonoBehaviour
{
    public static Gravity Instance;

    public Rigidbody ReferenceBody { get; private set; }

    public CelestialBody[] Bodies { get; private set; }

    public List<Rigidbody> Objects { get; private set; }

    [Header("Gravity Settings")]
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
