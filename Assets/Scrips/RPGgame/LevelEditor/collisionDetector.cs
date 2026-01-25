using UnityEngine;

public class collisionDetector : MonoBehaviour
{
    public bool isColliding;
    public bool isOnMap;
    private Rigidbody rb;
    void Start()
    {
        Collider collider = GetComponent<Collider>();
        collider.isTrigger = true;
        isColliding = false;
        rb = GetComponent<Rigidbody>();
        isOnMap = false;
        rb.constraints = RigidbodyConstraints.FreezeAll; // Freezing all movemnt when in editor.
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

}
