using UnityEngine;

public class OrbitAround : MonoBehaviour
{
    public Transform target; // The object to rotate around
    public float rotationSpeed = 50f; // Degrees per second
    public Vector3 rotationAxis = Vector3.up; // Axis to rotate around (default: Y-axis)

    void Update()
    {
        if (target != null)
        {
            // Rotate around the target's position
            transform.RotateAround(target.position, rotationAxis, rotationSpeed * Time.deltaTime);
        }
    }
}
