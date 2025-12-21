using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int health;
    public int coins;
    void Start()
    {
        health = PlayerPrefs.GetInt("playerHealth");
    }

    // Update is called once per frame
    void Update()
    {

    }
    void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag == "Enemy")
        {
            //health -= 1;
            //Debug.Log("Player Health: " + health);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Coin")
        {
            coins += 1;
            Debug.Log("Player Coins: " + coins);
            Destroy(other.gameObject);
        }
    }
}
