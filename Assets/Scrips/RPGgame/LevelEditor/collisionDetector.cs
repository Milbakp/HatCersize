using UnityEngine;

public class collisionDetector : MonoBehaviour
{
    public bool isColliding;
    public bool isOnMap;
    private Collider triggerCollider;
    public bool isFullyInside;
    void Start()
    {
        triggerCollider = GetComponent<Collider>();
        triggerCollider.isTrigger = true;
        isColliding = false;
        isOnMap = false;
        isFullyInside = false;

    }
    void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Map") || other.gameObject.CompareTag("Tile"))
        {
            isOnMap = true;
            return;
        }
        Debug.Log("Collision");
        isColliding = true;
    }
    void OnTriggerExit(Collider other) {
        if (other.gameObject.CompareTag("Map") || other.gameObject.CompareTag("Tile"))
        {
            isOnMap = false;
        }
        isColliding = false;
    }

    public bool IsFullyInside(Collider container, Collider target)
    {
        // Get the bounding boxes for both
        Bounds containerBounds = container.bounds;
        Bounds targetBounds = target.bounds;

        bool xInside = targetBounds.min.x >= containerBounds.min.x && targetBounds.max.x <= containerBounds.max.x;
        bool zInside = targetBounds.min.z >= containerBounds.min.z && targetBounds.max.z <= containerBounds.max.z;

        // Check if the container's bounds completely encapsulate the target's bounds
        return xInside && zInside;
    }

}
