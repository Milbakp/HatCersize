using System.Runtime.Serialization;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public int tileID;
    public bool tileIsFilled;
    public GameObject currentObject;
    [SerializeField] LevelEditorManager levelEditorManager;
    public bool connectedToMap;

    public void Start()
    {
        connectedToMap = false;
        tileIsFilled = false;
        levelEditorManager = FindAnyObjectByType<LevelEditorManager>();
    }
    private void OnMouseDown() {
        levelEditorManager.setCurrentTile(tileID);
    }
    // private void OnCollisionEnter(Collision other) {
    //     if(other.gameObject.CompareTag("Tile")||other.gameObject.CompareTag("Map"))
    //     {
    //         connectedToMap = true;
    //     }
    // }
    // private void OnCollisionExit(Collision other) {
    //     if(other.gameObject.CompareTag("Tile")||other.gameObject.CompareTag("Map"))
    //     {
    //         connectedToMap = false;
    //     }
    // }
}
