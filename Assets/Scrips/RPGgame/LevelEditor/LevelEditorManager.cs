using UnityEngine;
using System.Collections.Generic;

public class LevelEditorManager : MonoBehaviour
{
    public List<GameObject> Tiles  = new List<GameObject>();
    public int currentTile;
    private Tile tileComponent;
    public List<GameObject> Objects = new List<GameObject>();
    public int currentObject;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)  && tileComponent.tileIsFilled != true)
        {
            placeObject();
        }
        else if (Input.GetKeyDown(KeyCode.Space)  && tileComponent.tileIsFilled == true)
        {
            Debug.Log("Tile is taken");
        }
        if (Input.GetKeyDown(KeyCode.D)  && tileComponent.tileIsFilled == true)
        {
            destroyObject();
        }
    }
    public void placeObject()
    {
        tileComponent.currentObject = Instantiate(Objects[currentObject], Tiles[currentTile].transform.position, Quaternion.identity);
        tileComponent.tileIsFilled = true;
    }

    public void destroyObject()
    {
        Destroy(tileComponent.currentObject);
        tileComponent.tileIsFilled = false;
    }

    public void setCurrentTile(int tile)
    {
        currentTile = tile;
        tileComponent = Tiles[currentTile].GetComponent<Tile>();
    }
}
