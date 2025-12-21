using UnityEngine;
using TMPro;

public class PlayerUi : MonoBehaviour
{
    public TMP_Text healthText;
    public TMP_Text coinText;
    [SerializeField] PlayerHealth playerHealth;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        healthText.text = "Health: " + playerHealth.health;
        coinText.text = "Coins: " + playerHealth.coins;
    }
}
