using System.Data;
using UnityEngine;

public class OnMap : MonoBehaviour
{
    public int MapSize;
    public bool isOnMap = false;
    void OnMouseEnter() {
        isOnMap = true;
    }
    void OnMouseExit()
    {
        isOnMap = false;
    }
}
