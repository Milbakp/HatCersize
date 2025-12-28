using System.Runtime.Serialization;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public int tileID;
    public bool tileIsFilled;
    public GameObject currentObject;
    [SerializeField] LevelEditorManager levelEditorManager;

    public void Start()
    {
        tileIsFilled = false;
        levelEditorManager = FindAnyObjectByType<LevelEditorManager>();
    }
    private void OnMouseDown() {
        levelEditorManager.setCurrentTile(tileID);
    }
}
