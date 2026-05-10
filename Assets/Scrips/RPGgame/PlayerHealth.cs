using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int health;
    public int coins;
    private RPGLevelManager levelManager;
    private SoundManager soundManager;
    void Start()
    {
        health = PlayerPrefs.GetInt("playerHealth");
        levelManager = FindAnyObjectByType<RPGLevelManager>();
        soundManager = FindAnyObjectByType<SoundManager>();
        coins = levelManager.numOfEnemies;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Coin")
        {
            coins += 1;
            Debug.Log("Player Coins: " + coins);
            soundManager.PlayPickupSound();
            Destroy(other.gameObject);
        }
    }
}
