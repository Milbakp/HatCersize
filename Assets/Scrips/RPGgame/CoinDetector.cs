using UnityEngine;

public class CoinDetector : MonoBehaviour
{
    public PlayerHealth playerHealth;
    public RPGLevelManager levelManager;
    public int coinsNeeded;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerHealth = FindAnyObjectByType<PlayerHealth>();
        levelManager = FindAnyObjectByType<RPGLevelManager>();
        coinsNeeded = levelManager.numOfEnemies;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerEnter(Collider collision)
    {
        Debug.Log("Collided with: " + collision.gameObject.name);
        if (collision.gameObject.CompareTag("Player"))
        {
            if(playerHealth.coins >= coinsNeeded)
            {
                levelManager.openDoor();
                Debug.Log("Door opened!");
            }
            else
            {
                Debug.Log("Not enough coins to open the door.");
            }
        }
    }
}
