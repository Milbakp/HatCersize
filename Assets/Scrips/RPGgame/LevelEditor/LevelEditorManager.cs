using UnityEngine;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;
using TMPro;
public class LevelEditorManager : MonoBehaviour
{
    public List<GameObject> Tiles  = new List<GameObject>();
    public int currentTile;
    private Tile tileComponent;
    public List<GameObject> Objects = new List<GameObject>();
    public int currentObject;
    private string savePath;
    public GameObject previewObject;
    public Camera camera;
    public int MapType = 0;
    public List<GameObject> Maps = new List<GameObject>();
    public List<Button> previewButtons = new List<Button>();
    public TMP_Text editBttonText;
    public enum editState
    {
        Setting,
        Editting
    }
    editState currentEditState;
    void Start()
    {     
        savePath = Path.Combine(Application.persistentDataPath, "level.json");
        setPreview();
        foreach (Button btn in previewButtons)
        {
            int index = previewButtons.IndexOf(btn);
            btn.onClick.AddListener(() => {
                currentObject = index;
                setPreview();
            });
        }
        currentEditState = editState.Setting;
        editBttonText.SetText("Edit");
    }

    // Update is called once per frame
    void Update()
    {
        // if (Input.GetKeyDown(KeyCode.Space)  && tileComponent.tileIsFilled != true)
        // {
        //     placeObject();
        // }
        // else if (Input.GetKeyDown(KeyCode.Space)  && tileComponent.tileIsFilled == true)
        // {
        //     Debug.Log("Tile is taken");
        // }
        // if (Input.GetKeyDown(KeyCode.D)  && tileComponent.tileIsFilled == true)
        // {
        //     destroyObject();
        // }
        if (Input.GetKeyDown(KeyCode.S) && Input.GetKey(KeyCode.LeftControl))
        {
            ExportCurrentScene();
        }
        if (Input.GetMouseButtonDown(0) && currentEditState == editState.Setting)
        {
            spawnOnMousePosition();
        }
        if (Input.GetMouseButtonDown(1))
        {
            setPreview();
        }
        if(currentEditState == editState.Setting)
        {
            preview();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            currentEditState = editState.Editting;
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
    // Json file funcs
    public void SaveLevel(LevelData data)
    {
        // 1. Convert the object to a JSON string
        string json = JsonUtility.ToJson(data, true); // 'true' makes it pretty-printed

        // 2. Write that string to a file
        File.WriteAllText(savePath, json);

        Debug.Log("Level saved to: " + savePath);
    }

    public LevelData LoadLevel()
    {
        if (File.Exists(savePath))
        {
            // 1. Read the text from the file
            string json = File.ReadAllText(savePath);

            // 2. Convert the text back into a LevelData object
            LevelData data = JsonUtility.FromJson<LevelData>(json);
            return data;
        }
        else
        {
            Debug.LogError("Save file not found!");
            return null;
        }
    }
    public void ExportCurrentScene()
    {
        LevelData myLevel = new LevelData();

        // Imagine you have a bunch of tiles with a "Tile" tag
        GameObject[] tilesInScene = GameObject.FindGameObjectsWithTag("Tile");
        GameObject[] EnemiesInScene = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject[] EnvInScene = GameObject.FindGameObjectsWithTag("Environment");

        foreach (GameObject t in tilesInScene)
        {
            TileData td = new TileData();
            td.x = (int)t.transform.position.x;
            td.z = (int)t.transform.position.z;
            td.tileID = 1; // You'd get this from a script on the tile
            
            myLevel.tiles.Add(td);
        }
        foreach (GameObject t in EnemiesInScene)
        {
            TileData td = new TileData();
            td.x = (int)t.transform.position.x;
            td.z = (int)t.transform.position.z;
            td.tileID = 2; // You'd get this from a script on the tile
            
            myLevel.tiles.Add(td);
        }
        foreach (GameObject t in EnvInScene)
        {
            TileData td = new TileData();
            td.x = (int)t.transform.position.x;
            td.z = (int)t.transform.position.z;
            td.tileID = 3; // You'd get this from a script on the tile
            
            myLevel.tiles.Add(td);
        }
        OnMap om = Maps[MapType].GetComponent<OnMap>();
        myLevel.MapSize = om.MapSize;

        SaveLevel(myLevel);
    }
    private void spawnOnMousePosition() {
        collisionDetector cd = previewObject.GetComponent<collisionDetector>();
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = camera.transform.position.y;
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePos);
        worldPosition.y = 0; 

        GameObject tmp;
        //OnMap om = Map.GetComponent<OnMap>();
        if(previewObject.CompareTag("Tile"))
        {
            if (cd.isOnMap)
            {
                Debug.Log("Can not place Tile");
                return;
            } 
            tmp = Instantiate(Objects[currentObject], worldPosition, Quaternion.identity);
            tmp.AddComponent<EditObject>();
            Debug.Log("Placing Tile");
            return;
        }
        if (cd.isColliding || !cd.isOnMap ) {
            Debug.Log("Cannot place object here!");
            return;
        }
        tmp = Instantiate(Objects[currentObject], worldPosition, Quaternion.identity);
        tmp.AddComponent<EditObject>();
    }

    public void setPreview()
    {
        GameObject tmp = previewObject;
        previewObject = Instantiate(Objects[currentObject], previewObject.transform.position, Quaternion.identity);
        Destroy(tmp);
        previewObject.AddComponent<collisionDetector>();
    }

    private void preview()
    {
        Vector3 mousePos = Input.mousePosition;
        // Sets distance of the object relative to the camera so the object appears under the mouse cursor
        mousePos.z = camera.transform.position.y;
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePos);
        worldPosition.y = 0; 
        previewObject.transform.position = worldPosition;
    }

    //Buton Functions are below this line
    public void SetMode()
    {
        if(currentEditState == editState.Setting)
        {
            currentEditState = editState.Editting;
            editBttonText.SetText("Set");
            previewObject.SetActive(false);
        }
        else
        {
            currentEditState = editState.Setting;
            editBttonText.SetText("Edit");
            previewObject.SetActive(true);
        }
    }

}
