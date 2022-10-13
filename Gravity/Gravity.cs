using UnityEngine;

public class Gravity : MonoBehaviour
{
    public static Gravity Instance;

    [SerializeField] private float g;

    public float G { get { return g; } }

    [SerializeField] private Body referenceBody;

    public Body ReferenceBody { get { return referenceBody; } }

    private Body[] bodies;

    public Body[] Bodies { get { return bodies; } } 

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

        if(bodies == null)
        {
            bodies = FindObjectsOfType<Body>();
        }
    }
}
