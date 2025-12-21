using UnityEngine;

public class EndPoint : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
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
