using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class Shop : MonoBehaviour
{
    public List<int> coinCost = new List<int>();
    public GameObject text;
    public TMP_Text coinText;
    public TMP_Text atkText;
    public TMP_Text hpText;
    void Start()
    {
        text.SetActive(false);
    }
    void Update()
    {
        coinText.text = "Coins: " + PlayerPrefs.GetInt("coins");
        atkText.text = "AtK: " + PlayerPrefs.GetInt("playerAttack");
        hpText.text = "HP: " + PlayerPrefs.GetInt("playerHealth");
    }
    public void increaseAtk()
    {
        if (checkCoinAmount())
        {
            PlayerPrefs.SetInt("playerAttack", PlayerPrefs.GetInt("playerAttack") + 1);
        }
    }
    public void increaseHp()
    {
        if (checkCoinAmount())
        {
            PlayerPrefs.SetInt("playerHealth", PlayerPrefs.GetInt("playerHealth") + 1);
        }
    }
    public bool checkCoinAmount()
    {
        int coinAmount = PlayerPrefs.GetInt("coins");
        if(coinAmount >= coinCost[0])
        {
            PlayerPrefs.SetInt("coins", coinAmount - coinCost[0]);
            return true;
        } else
        {
            text.SetActive(true);
            StartCoroutine(hideText());
            return false;
        }
    }

    IEnumerator hideText()
    {
        yield return new WaitForSeconds(2);
        text.SetActive(false);
    }
}
