using UnityEngine;

public class MapBounds : MonoBehaviour
{
    public GameObject colliderChild;
    public bool borderingTile;
    void Start()
    {
        borderingTile = false;
        colliderChild.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision other)
    {
        Debug.Log("Collison is happening");
        borderingTile = true;
        colliderChild.SetActive(false);
    }

    void OnCollisionExit(Collision other)
    {
        borderingTile = false;
        colliderChild.SetActive(true);
    }

    // void OnCollisionStay(Collision other)
    // {
    //     borderingTile = true;
    //     colliderChild.SetActive(false);
    // }


}
