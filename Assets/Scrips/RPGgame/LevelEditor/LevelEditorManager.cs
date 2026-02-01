using UnityEngine;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;
using TMPro;
using static TileRegistry;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Security.Cryptography;

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
    public NavMeshManager navMeshManager;
    void Start()
    {     
        savePath = Path.Combine(Application.persistentDataPath, "level.json");
        setPreview();
        // Setting up the object preview buttons
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

        // if (Input.GetKeyDown(KeyCode.E))
        // {
        //     currentEditState = editState.Editting;
        // }
        if (Input.GetKeyDown(KeyCode.R))
        {
            previewObject.transform.Rotate(0, 90, 0);
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
        navMeshManager.createNavMesh();
        if (navMeshManager.IsPathAvailable() == false)
        {
            Debug.Log("Cannot export level");
            return;
        }
        LevelData myLevel = new LevelData();

        // Finds everything with the LevelObjectInfo script, regardless of Tag
        LevelObjectInfo[] allObjects = FindObjectsOfType<LevelObjectInfo>();

        foreach (LevelObjectInfo info in allObjects)
        {
            TileData td = new TileData();
            td.x = info.transform.position.x;
            td.y = info.transform.position.y;
            td.z = info.transform.position.z;
            td.rotationY = info.transform.rotation.eulerAngles.y;
            td.tileID = info.tileID;
            
            myLevel.tiles.Add(td);
        }
        Vector3 playerStartPos = GameObject.FindWithTag("StartPosition").transform.position;
        myLevel.playerStartPosition = playerStartPos;

        Vector3 destinationPos = GameObject.FindWithTag("EndPosition").transform.position;
        myLevel.destinationPosition = destinationPos;
        SaveLevel(myLevel);
    }
    private void spawnOnMousePosition() {
        collisionDetector cd = previewObject.GetComponent<collisionDetector>();
        //Vector3 mousePos = Input.mousePosition;
        //mousePos.z = camera.transform.position.y;
        //Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePos);

        //previewObject.transform.position = new Vector3(previewObject.transform.position.x, previewObject.transform.position.y, previewObject.transform.position.z);

        GameObject tmp;
        GameObject prefab = registry.GetPrefab(currentObject + 1);

        if(previewObject.CompareTag("Tile"))
        {
            if (cd.isOnMap)
            {
                Debug.Log("Can not place Tile");
                return;
            } 
            tmp = Instantiate(prefab, previewObject.transform.position, Quaternion.identity);
            tmp.AddComponent<collisionDetector>();
            tmp.AddComponent<EditObject>();
            Debug.Log("Placing Tile");
            Tiles.Add(tmp);
            return;
        }
        if (cd.isColliding || !cd.isOnMap ) {
            Debug.Log("Cannot place object here!");
            return;
        }
        tmp = Instantiate(prefab, previewObject.transform.position, previewObject.transform.rotation);
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
        previewObject.transform.position = new Vector3 (previewObject.transform.position.x, 0, previewObject.transform.position.z);
        previewObject.transform.position = new Vector3(previewObject.transform.position.x, LiftAboveZero(previewObject), previewObject.transform.position.z);
    }

    private void preview()
    {
        Vector3 mousePos = Input.mousePosition;
        // Sets distance of the object relative to the camera so the object appears under the mouse cursor
        mousePos.z = camera.transform.position.y;
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePos);
        //worldPosition.y = 0; 
        previewObject.transform.position =  new Vector3 (worldPosition.x, previewObject.transform.position.y, worldPosition.z);
    }

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
    // Returns distance needed to lift object above y = 0
    public float LiftAboveZero(GameObject liftGameObject)
    {
        // 1. Get the Bounds of the object (includes all children)
        Bounds combinedBounds = GetTargetBounds(liftGameObject);

        // 2. Calculate how far the bottom of the bounds is from y = 0
        float bottomY = combinedBounds.min.y;

        if (bottomY < 0)
        {
            // 3. Lift the object by the difference
            float distanceToLift = Mathf.Abs(bottomY);
            //transform.position += new Vector3(0, distanceToLift, 0);
            Debug.Log($"{gameObject.name} lifted by {distanceToLift} units.");
            return distanceToLift;
        }
        else
        {
            Debug.Log($"{gameObject.name} is already above zero.");
        }
        return 0.0f;
    }
    private Bounds GetTargetBounds(GameObject obj)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        
        if (renderers.Length == 0) return new Bounds(obj.transform.position, Vector3.zero);

        // Initialize bounds with the first renderer
        Bounds b = renderers[0].bounds;

        // Encapsulate all other renderers (for objects with multiple parts)
        for (int i = 1; i < renderers.Length; i++)
        {
            b.Encapsulate(renderers[i].bounds);
        }
        return b;
    }

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

    public void PlayTestLevel()
    {
        SceneManager.LoadScene("TestLoadLevel");
    }

}
