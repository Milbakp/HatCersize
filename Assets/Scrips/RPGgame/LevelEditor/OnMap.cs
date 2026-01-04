using UnityEngine;

public class OnMap : MonoBehaviour
{
    public bool isOnMap = false;
    void OnMouseEnter() {
        isOnMap = true;
    }
    void OnMouseExit()
    {
        isOnMap = false;
    }
}
