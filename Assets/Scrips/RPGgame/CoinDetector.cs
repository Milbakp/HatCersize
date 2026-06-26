using UnityEngine;

public class CoinDetector : MonoBehaviour
{
    public PlayerHealth playerHealth;
    public RPGLevelManager levelManager;
    public GameManager gameManager;
    public int coinsNeeded;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(gameManager.CurrentState != GameManager.GameState.InGame)
        {
            Debug.Log("CoinDetector: Game is not in InGame state. Disabling CoinDetector.");
            this.enabled = false;
            return;
        }
        playerHealth = FindAnyObjectByType<PlayerHealth>();
        levelManager = FindAnyObjectByType<RPGLevelManager>();
        coinsNeeded = levelManager.numOfEnemies;
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
