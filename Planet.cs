using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Planet : MonoBehaviour
{
    [SerializeField] private float semiMajor;
    [SerializeField] private float semiMinor;

    [SerializeField] private float inclination;

    [SerializeField] private float speed;

    [SerializeField] private float referenceAngle;

    private float currentAngle;

    private LineRenderer orbitRenderer;

    [SerializeField] private int renderPoints;

    private Vector3 orbitPlane;
    private Vector3 orbitTangent;
    private Vector3 focusPosition;

    private void Awake()
    {
        orbitRenderer = GetComponent<LineRenderer>();
    }

    private void Start()
    {
        CalculateOrbit();

        DrawOrbit();
    }

    private void Update()
    {
        CalculateOrbit();
        DrawOrbit();

        transform.position = GetOrbitPosition(currentAngle);

        currentAngle += speed * Time.deltaTime;

        
    }

    // Calculates the orbit plane and tangent vectors
    private void CalculateOrbit()
    {
        Quaternion referenceRot = Quaternion.AngleAxis(referenceAngle, Vector3.up);

        orbitPlane = referenceRot * new Vector3(Cos(inclination), Sin(inclination), 0);
        orbitTangent = referenceRot * Vector3.forward;

        float focalLength = Mathf.Sqrt((semiMajor * semiMajor) - (semiMinor * semiMinor));
        focusPosition = orbitPlane.normalized * focalLength;
    }

    // Calculates and returns the position along the orbit at the given orbit angle
    private Vector3 GetOrbitPosition(float angle)
    {
        return focusPosition + (semiMajor * orbitPlane * Cos(angle)) + (semiMinor * orbitTangent * Sin(angle));
    }

    // Calculates and sets the render positions of the line renderer along the path of the orbit
    private void DrawOrbit()
    {
        float angleStep = 360f / renderPoints;

        Vector3[] points = new Vector3[renderPoints];

        for(int i = 0; i < renderPoints; i++)
        {
            float angle = angleStep * i;

            points[i] = GetOrbitPosition(angle);
        }

        orbitRenderer.positionCount = renderPoints;
        orbitRenderer.SetPositions(points);
    }

    // Calculates and returns the cosine of the given degrees value
    private float Cos(float value)
    {
        return Mathf.Cos(value * Mathf.Deg2Rad);
    }

    // Calculates and returns the sine of the given degrees value
    private float Sin(float value)
    {
        return Mathf.Sin(value * Mathf.Deg2Rad);
    }
}