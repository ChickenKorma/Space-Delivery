using UnityEngine;

public class ReferenceCorrection : MonoBehaviour
{
    private void FixedUpdate()
    {
        // Loop through every rigidbody and offset relative to reference body
        Vector3 origin = Gravity.Instance.ReferenceBodyRB.position;

        foreach (Rigidbody obj in Gravity.Instance.Objects)
        {
            obj.MovePosition(obj.position - origin);
        }
    }
}
