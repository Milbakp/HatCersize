using UnityEngine;
using System;
using System.Collections.Generic;

public class Interact : MonoBehaviour
{
    public Transform cam;
    private Color rayColor = Color.red;
    public float attackRange;
    public Collider nearestObject;
    public List<Collider> TriggerList = new List<Collider>();
    public float rotationSpeed;
    public GameObject player;
    bool isTurning;
    void Start()
    {
        isTurning = false;
        nearestObject = new Collider();
    }

    void Update()
    {
        if (isTurning)
        {
            lockOn();
        }
        // Clean up destroyed enemies
        for (int i = TriggerList.Count - 1; i >= 0; i--)
        {
            if (TriggerList[i] == null) // destroyed object
            {
                TriggerList.RemoveAt(i);
            }
        }
    }
    public void interact()
    {
        //Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        Debug.DrawRay(cam.position, cam.forward * attackRange, rayColor, 0f);
        if (Physics.Raycast(cam.position,cam.forward, out hit, attackRange))
        {
            //Debug.Log("Hit " + hit.transform.name);
            if (hit.transform.gameObject.tag == "Interactable")
            {
                hit.transform.gameObject.GetComponent<InteractableObject>().interact();
            }
        }
        getNearest();
        Debug.LogError("Interacted");
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
        if(!TriggerList.Contains(other) && (other.gameObject.tag == "Interactable" ||other.gameObject.tag == "Enemy"))
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

    void getNearest()
    {
        if (TriggerList.Count <= 0)
        {
            return;
        }
        isTurning = true;
        float nearestDistance = 100;
        foreach (Collider col in TriggerList){
            float distanceToPlayer = Vector3.Distance(col.transform.position, transform.position);
            if (distanceToPlayer < nearestDistance)
            {
                nearestDistance = distanceToPlayer;
                nearestObject = col;
            }

        }
        Debug.Log("Nearest Object is " + nearestObject.gameObject.name);
        return;
    }

    void lockOn()
    {
        Vector3 direction = nearestObject.transform.position - transform.position;
        direction.y = 0f; // prevent tilting up/down

        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            player.transform.rotation = Quaternion.Slerp(
                player.transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
            // Check if finished rotating
            float angle = Quaternion.Angle(player.transform.rotation, targetRotation);

            if (angle < 1f) // <-- angle threshold
            {
                player.transform.rotation = targetRotation; // snap final
                isTurning = false; // stop rotating
            }
        }
        
    }

}
