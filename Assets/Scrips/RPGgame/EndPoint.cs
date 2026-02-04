using UnityEngine;

public class EndPoint : MonoBehaviour
{
    Collider parentCollider;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        parentCollider = GetComponentInParent<Collider>();
        // This manually tells Physics to ignore collisions between this object 
        // and whatever layer or specific collider you want to avoid.
        Physics.IgnoreCollision(GetComponent<Collider>(), parentCollider);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            RPGLevelManager levelManager = FindAnyObjectByType<RPGLevelManager>();
            if (levelManager != null)
            {
                levelManager.LevelCompleted();
            }
        }
    }
}
