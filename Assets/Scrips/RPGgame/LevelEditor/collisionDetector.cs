using UnityEngine;

public class collisionDetector : MonoBehaviour
{
    public bool isColliding;
    void Start()
    {
        Collider collider = GetComponent<Collider>();
        collider.isTrigger = true;
        isColliding = false;
    }
    void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Map"))
        {

            return;
        }
        Debug.Log("Collision");
        isColliding = true;
    }

    void OnTriggerExit(Collider other) {
        isColliding = false;
    }

}
