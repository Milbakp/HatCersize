using UnityEngine;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;
using TMPro;
using static TileRegistry;
using UnityEngine.EventSystems;

public class LevelEditorManager : MonoBehaviour
{
    public List<GameObject> Tiles  = new List<GameObject>();
    public int currentTile;
    private Tile tileComponent;
    //public List<GameObject> Objects = new List<GameObject>();
    public int currentObject;
    private string savePath;
    public GameObject previewObject;
    public Camera camera;
    public int MapType = 0;
    public List<GameObject> Maps = new List<GameObject>();
    public List<Button> previewButtons = new List<Button>();
    public TMP_Text editBttonText;
    public TileRegistry registry;
    public enum editState
    {
        Setting,
        Editting
    }
    editState currentEditState;
    public bool isEditingObject = false;
    //public Bounds TileBounds;
    public List<Collider> tileColliders = new List<Collider>();
    void Start()
    {     
        savePath = Path.Combine(Application.persistentDataPath, "level.json");
        setPreview();
        CreatePreviewButtons();
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
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                spawnOnMousePosition();
            }
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
        //tileComponent.currentObject = Instantiate(Objects[currentObject], Tiles[currentTile].transform.position, Quaternion.identity);
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
        if (!AllTilesConnected())
        {
            Debug.Log("Cannot export level, not all tiles are connected");
            return;
        }
        LevelData myLevel = new LevelData();

        // Finds everything with the LevelObjectInfo script, regardless of Tag
        LevelObjectInfo[] allObjects = FindObjectsOfType<LevelObjectInfo>();

        foreach (LevelObjectInfo info in allObjects)
        {
            TileData td = new TileData();
            td.x = (int)info.transform.position.x;
            td.z = (int)info.transform.position.z;
            td.tileID = info.tileID;
            
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
        GameObject prefab = registry.GetPrefab(currentObject + 1);
        //OnMap om = Map.GetComponent<OnMap>();
        if(previewObject.CompareTag("Tile"))
        {
            if (cd.isOnMap)
            {
                Debug.Log("Can not place Tile");
                return;
            } 
            tmp = Instantiate(prefab, worldPosition, Quaternion.identity);
            tmp.AddComponent<collisionDetector>();
            tmp.AddComponent<EditObject>();
            Debug.Log("Placing Tile");
            Tiles.Add(tmp);
            //updateTileBounds();
            return;
        }
        if (cd.isColliding || !cd.isOnMap) {
            Debug.Log("Cannot place object here!");
            return;
        }
        tmp = Instantiate(prefab, worldPosition, Quaternion.identity);
        tmp.AddComponent<collisionDetector>();
        tmp.AddComponent<EditObject>();
    }

    public void setPreview()
    {
        GameObject tmp = previewObject;
        GameObject prefab = registry.GetPrefab(currentObject + 1);
        previewObject = Instantiate(prefab, previewObject.transform.position, Quaternion.identity);
        Destroy(tmp);
        previewObject.AddComponent<collisionDetector>();
        LevelObjectInfo info = previewObject.GetComponent<LevelObjectInfo>();
        if (info != null)
        {
            Destroy(info); // Removes LevelObjectInfor from the preview object so it doesn't get saved.
        }
        if(previewObject.CompareTag("Tile"))
        {
            previewObject.transform.Find("TileDetector").gameObject.SetActive(false);
        }
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

    // Tile logic functions
    private bool AllTilesConnected()
    {
        foreach (GameObject t in Tiles)
        {
            GameObject tmp = t.transform.Find("TileDetector").gameObject;
            DetectTileConnections dtc = tmp.GetComponentInChildren<DetectTileConnections>();
            if (!dtc.isConnected)
            {
                Debug.Log("Not all tiles are connected");
                return false;
            }
        }
        return true;
    }
    // This might be inefficint as it clears and repopulates the list every time, fix this later
    // public void updateTileBounds()
    // {
    //     tileColliders.Clear();
    //     foreach (GameObject t in Tiles)
    //     {
    //         Collider col = t.GetComponent<Collider>();
    //         tileColliders.Add(col);
    //     }
    //     if (tileColliders.Count == 0)
    //     {
    //         TileBounds = new Bounds(Vector3.zero, Vector3.zero);
    //         return;
    //     }
    //     TileBounds = tileColliders[0].bounds;
    //     for (int i = 1; i < tileColliders.Count; i++)
    //     {
    //         TileBounds.Encapsulate(tileColliders[i].bounds);
    //     }
    // }

    //Button Functions are below this line
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
    public editState getMode()
    {
        return currentEditState;
    }

    public void CreatePreviewButtons()
    {
        // Find the content parent once to save performance
        Transform contentParent = GameObject.Find("Content").transform;

        foreach(TileRegistry.TileEntry te in registry.entries)
        {
            // 1. Create the Button Root
            GameObject btnObj = new GameObject(te.prefab.name + "_Button");
            btnObj.transform.SetParent(contentParent, false);
            
            // 2. Add UI Visuals (Buttons need an Image to be clickable!)
            btnObj.AddComponent<CanvasRenderer>();
            btnObj.AddComponent<Image>(); 
            Button btn = btnObj.AddComponent<Button>();

            // 3. Create a Child Object for the Text
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(btnObj.transform, false);
            
            // Use TextMeshProUGUI, NOT TMP_Text
            TextMeshProUGUI btnText = textObj.AddComponent<TextMeshProUGUI>();
            btnText.text = te.prefab.name;
            btnText.fontSize = 24;
            btnText.alignment = TextAlignmentOptions.Center;
            btnText.color = Color.black;

            previewButtons.Add(btn);
        }
    }

}
