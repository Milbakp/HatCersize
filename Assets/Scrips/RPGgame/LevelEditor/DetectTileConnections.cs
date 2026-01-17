using UnityEngine;

public class DetectTileConnections : MonoBehaviour
{
    public bool isConnected;
    public float distanceToTile;

    void Start()
    {
        isConnected = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerExit(Collider other) {
        isConnected = false;
    }
    private void OnTriggerStay(Collider other) {
        isConnected = true;
    }
}
