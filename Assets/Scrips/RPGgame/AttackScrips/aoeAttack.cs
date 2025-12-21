using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aoeAttack : MonoBehaviour
{
    public List<Collider> TriggerList = new List<Collider>();
    public float AttackSpeed;
    bool readyToAttack;
    void Start()
    {   
        readyToAttack = true;
    }

    // Update is called once per frame
    void Update()
    {
        // if (Input.GetKeyDown(KeyCode.Space))
        // {
        //     aoeDamage(1);
        // }
    }
    //called when something enters the trigger
    void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Object Enters");
        if(other.gameObject.name == "Player" || other.gameObject.name == "PlayerObject")
        {
            return;
        }
        //if the object is not already in the list
        if(!TriggerList.Contains(other) && other.gameObject.tag == "Enemy")
        {
            //add the object to the list
            TriggerList.Add(other);
        }
    }

    //called when something exits the trigger
    void OnTriggerExit(Collider other)
    {
        //if the object is in the list
        if(TriggerList.Contains(other))
        {
            //remove it from the list
            TriggerList.Remove(other);
        }
    }
    public void aoeDamage(int damage)
    {
        if (!readyToAttack)
        {
            Debug.LogError("AOE Attack on Cooldown");
            return;
        }
        readyToAttack = false;
        StartCoroutine(aoeAttackCooldown());

        List<Collider> TMPTriggerList = new List<Collider>();
        foreach (Collider col in TriggerList)
        {
            col.gameObject.GetComponent<Enemy>().lowerHealth(damage);
            Debug.Log("Dealt " + damage + " to " + col.gameObject.name);
            if(col.gameObject.GetComponent<Enemy>().health <= 0)
            {
                col.gameObject.GetComponent<Enemy>().Die();
                continue;
            }
            TMPTriggerList.Add(col);
        }
        TriggerList = TMPTriggerList;
    }

    public IEnumerator aoeAttackCooldown()
    {
        yield return new WaitForSeconds(AttackSpeed);
        readyToAttack = true;
    }

    public bool isReady()
    {
        return readyToAttack;
    }
}
