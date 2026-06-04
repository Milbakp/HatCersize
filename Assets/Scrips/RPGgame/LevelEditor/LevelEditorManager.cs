using UnityEngine;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;
using TMPro;
using static TileRegistry;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System;


//using UnityEditor;
#if ENABLE_WINMD_SUPPORT
using Windows.Storage;
using Windows.Storage.Pickers;
using System;
using System.Threading.Tasks; // Added for Task support
#endif

public class LevelEditorManager : MonoBehaviour
{
    public List<GameObject> Tiles  = new List<GameObject>();
    public int currentObject;
    private string savePath;
    public GameObject previewObject;
    public Camera camera;
    public List<Button> previewButtons = new List<Button>();
    public TMP_Text editBttonText;
    public TileRegistry registry;
    public TMP_Text errorMessages;
    public enum editState
    {
        Setting,
        Editting,
        LoadingLevel
    }
    editState currentEditState;
    public bool isEditingObject = false;
    public NavMeshManager navMeshManager;
    public GameObject playerLocationIndicator, destinationIndicator, buttonPrefab;
    public GameManager gameManager;
    public TMP_FontAsset newFont;
    private LevelData leveldata;
    private bool loadingLevel = false;
    private Vector3 lastTileSpawnPosition = Vector3.zero;
    public float tileLockWidth = 10.0f;
    public event Action Preview; // To turn off slider UI of fence generator when previewing a new object.
    public class undoClass
    {
        public Vector3 savedPosition;
        public Quaternion savedRotation;
        public GameObject activeGameObject;
        public bool placeAction = false;
    }
    private FixedSizeStack<undoClass> undoClassList = new FixedSizeStack<undoClass>();
    private undoClass mostRecentAction;
    public GameObject LevelLoadContainerPrefab;
    void Start()
    {    
        setPreview();
        // Setting up the object preview buttons
        CreatePreviewButtons();
        foreach (Button btn in previewButtons)
        {
            int index = btn.GetComponent<ButtonRegistry>().id;
            btn.onClick.AddListener(() => {
                currentObject = index;
                SetMode(editState.Setting);
                setPreview();
            });
        }
        SetMode(editState.Editting);
        editBttonText.SetText("Edit");
        gameManager = FindAnyObjectByType<GameManager>();
        // Set the game state to Menu when in the level editor to prevent unintended interactions with other game systems
        GameManager.Instance.SetGameState(GameManager.GameState.Menu);
        Debug.LogError("CurrentState: " + GameManager.Instance.CurrentState);

        errorMessages.raycastTarget = false; // UI element doesn't block objects so that it can be placed at the bottom of the screen
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S) && Input.GetKey(KeyCode.LeftControl))
        {
            ExportCurrentScene();
        }

        if (Input.GetMouseButtonDown(0) && currentEditState == editState.Setting && !previewObject.CompareTag("Tile"))
        {
            if (!EventSystem.current.IsPointerOverGameObject())// Makes sure that objects are not placed while hover over UI elements.
            {
                spawnOnMousePosition();
            }
        }
        // Special case for tiles
        if (Input.GetMouseButton(0) && currentEditState == editState.Setting && previewObject.CompareTag("Tile"))
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                float distance = Vector3.Distance(previewObject.transform.position, lastTileSpawnPosition);
                if (distance > tileLockWidth) // Only spawn if the position has changed significantly
                {
                    spawnOnMousePosition();
                }
            }
        }

        if(currentEditState == editState.Setting)
        {
            preview();
        }
        // cancel selection and go back into edit mode.
        if(Input.GetMouseButtonDown(1) && currentEditState == editState.Setting)
        {
            SetMode(editState.Editting);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            previewObject.transform.Rotate(0, 90, 0);
        }
        if (loadingLevel)
        {
            loadingLevel = false;
            constructLevel(leveldata);
        }
        // Undo action
        if (Input.GetKeyDown(KeyCode.Z) && Input.GetKey(KeyCode.LeftControl))
        {
            undoAction();
        }
    }

    // Json file funcs
    public async void SaveLevel(LevelData data)
    {
        #if ENABLE_WINMD_SUPPORT
        UnityEngine.WSA.Application.InvokeOnUIThread(async () =>
        {
            try 
            {
                FileSavePicker savePicker = new FileSavePicker();
                savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
                
                // Ensure the extension matches exactly
                savePicker.FileTypeChoices.Add("JSON File", new List<string>() { ".json" });
                savePicker.SuggestedFileName = "NewLevel";

                // This line requires the 'async' keyword in the method signature
                StorageFile file = await savePicker.PickSaveFileAsync();

                if (file != null)
                {
                    string json = JsonUtility.ToJson(data, true);
                    await FileIO.WriteTextAsync(file, json);
                    Debug.Log("Saved successfully: " + file.Path);
                }
                else
                {
                    Debug.Log("User cancelled the picker.");
                }
            }
            catch (Exception ex)
            {
                // This will tell you the EXACT error (e.g., Access Denied or Threading error)
                Debug.LogError("UWP Picker Exception: " + ex.Message);
            }
        }, true);
    #else
        Debug.LogError("This function only works on UWP builds!");
    #endif
    }
    #region Loading Level to Edit
    public async void playerMadeLevel(){
        #if ENABLE_WINMD_SUPPORT
        UnityEngine.WSA.Application.InvokeOnUIThread(async () =>
        {
        try{
            // Initialize the Picker
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.List;
            openPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;

            // Filter for the file types you want to show
            openPicker.FileTypeFilter.Add(".json");

            // Open the picker and wait for the user to select a file
            StorageFile file = await openPicker.PickSingleFileAsync();
        
            if (file != null)
            {
                // Read the file content
                string json = await FileIO.ReadTextAsync(file);

                // Convert JSON back into your LevelData object
                LevelData data = JsonUtility.FromJson<LevelData>(json);
                // Ignoring files that aren't LevelData (e.g., CampaignData)
                if (data.fileType != "LevelData")
                {
                    displayErrorMessage("Selected file is not a level");
                    throw new Exception("Selected file is not a level");
                }
                leveldata = data;

                Debug.Log("Level loaded successfully: " + file.Name);
                // Can't construct the level here because Unity doesn't allow async calls to modify the scene, so we set a flag and do it in the Update function
                loadingLevel = true;
            }
            else
            {
                Debug.Log("Load operation cancelled.");
            }
        }
        catch (Exception ex)
            {
                // This will tell you the EXACT error (e.g., Access Denied or Threading error)
                Debug.LogError("UWP Picker Exception: " + ex.Message);
            }
        }, true);
    #else
        Debug.LogError("This function only works on UWP builds!");
    #endif
    }

    public void constructLevel(LevelData data)
    {
        LevelData newLevel = data;
        if (newLevel == null)
        {
            Debug.LogError("Could not load level");
            return;
        }
        GameObject levelContainer = Instantiate(LevelLoadContainerPrefab, Vector3.zero, Quaternion.identity);
        foreach(TileData td in newLevel.tiles)
        {
            GameObject prefab = registry.GetPrefab(td.tileID);
            if (prefab != null)
            {
                GameObject instance = Instantiate(prefab, new Vector3(td.x, td.y, td.z), Quaternion.Euler(0, td.rotationY, 0));
                instance.AddComponent<collisionDetector>();
                instance.AddComponent<EditObject>();
                if(instance.CompareTag("Tile"))
                {
                    Tiles.Add(instance);
                }
                if (instance.CompareTag("Fence"))
                {
                    FenceGenerator fenceGenerator = instance.GetComponent<FenceGenerator>();
                    fenceGenerator.fenceLength = td.fenceLength;
                    fenceGenerator.GenerateFence();
                    fenceGenerator.setIsPreview(true);
                }
                instance.transform.SetParent(levelContainer.transform);
            }
        }
        levelContainer.GetComponent<LevelLoadContainer>().ResizeColliderToFitChildren();
        // Removing the end and start points to prevent duplicates since it is possible to have placed an end point already.
        // GameObject player = Instantiate(playerLocationIndicator);
        // player.transform.position =  new Vector3(newLevel.playerStartPosition.x, player.transform.position.y, newLevel.playerStartPosition.z);
        // player.transform.rotation = Quaternion.Euler(0, newLevel.playerRotationY, 0);
        // player.AddComponent<collisionDetector>();
        // player.AddComponent<EditObject>();
        
        // GameObject destination = Instantiate(destinationIndicator);
        // destination.transform.position = newLevel.destinationPosition;
        // destination.transform.rotation = Quaternion.Euler(0, newLevel.destinationRotationY, 0);
        // destination.AddComponent<collisionDetector>();
        // destination.AddComponent<EditObject>();
    }
    #endregion

    public void ExportCurrentScene()
    {
        GameObject[] startPoints = GameObject.FindGameObjectsWithTag("StartPosition");
        GameObject[] endPoints = GameObject.FindGameObjectsWithTag("EndPosition");
        if(!(startPoints.Length >=1) || !(endPoints.Length >= 1))
        {
            displayErrorMessage("Cannot export level, start point or end point not yet set.");
            Debug.Log("Cannot export level, start point or end point not yet set.");
            return;
        }
        if (!AllTilesConnected())
        {
            displayErrorMessage("Cannot export level, not all tiles are connected.");
            Debug.Log("Cannot export level, not all tiles are connected");
            return;
        }
        navMeshManager.createNavMesh();
        if (navMeshManager.IsPathAvailable() == false)
        {
            displayErrorMessage("Cannot export level, Path doesn't exist");
            Debug.Log("Cannot export level, Path doesn't exist");
            return;
        }

        LevelData myLevel = new LevelData();

        // Finds everything with the LevelObjectInfo script, regardless of Tag
        LevelObjectInfo[] allObjects = FindObjectsOfType<LevelObjectInfo>();

        foreach (LevelObjectInfo info in allObjects)
        {
            // Ignore the start postion and endposition objects
            if (info.CompareTag("StartPosition"))
            {
                GameObject playerStart = info.transform.gameObject;
                myLevel.playerStartPosition = playerStart.transform.position;
                myLevel.playerRotationY = playerStart.transform.rotation.eulerAngles.y;
                continue;
            }
            else if (info.CompareTag("EndPosition"))
            {
                GameObject destination = info.transform.gameObject;
                myLevel.destinationPosition = destination.transform.position;
                myLevel.destinationRotationY = destination.transform.rotation.eulerAngles.y; 
                continue;
            }
            if (!info.gameObject.activeSelf)
            {
                return; // Object is deleted
            }

            TileData td = new TileData();
            if (info.CompareTag("Fence"))
            {
                FenceGenerator fenceGenerator = info.GetComponent<FenceGenerator>();
                td.fenceLength = fenceGenerator.fenceLength;
            }
            td.x = info.transform.position.x;
            td.y = info.transform.position.y;
            td.z = info.transform.position.z;
            td.rotationY = info.transform.rotation.eulerAngles.y;
            td.tileID = info.tileID;
            myLevel.tiles.Add(td);
        }
        myLevel.fileType = "LevelData"; // Set the file type for identification when loading
        SaveLevel(myLevel);
        displayErrorMessage("Level Saved");
    }

    private void spawnOnMousePosition() {
        // makes sure you don't place more than one start and end point.
        if(previewObject.CompareTag("StartPosition"))
        {
            GameObject[] startPoints = GameObject.FindGameObjectsWithTag("StartPosition");
            if(startPoints.Length >= 2)
            {
                displayErrorMessage("Start point already exist");
                Debug.Log("Start point already exist");
                return;
            }
        }
        if (previewObject.CompareTag("EndPosition"))
        {
            GameObject[] endPoints = GameObject.FindGameObjectsWithTag("EndPosition");
            if(endPoints.Length >= 2)
            {
                displayErrorMessage("End point already exist");
                Debug.Log("End point already exist");
                return;
            }
        }
        
        collisionDetector cd = previewObject.GetComponent<collisionDetector>();
        GameObject tmp;
        GameObject prefab = registry.GetPrefab(currentObject);

        if(previewObject.CompareTag("Tile"))
        {
            if (cd.isOnMap)
            {
                displayErrorMessage("Can not place Tile");
                Debug.Log("Can not place Tile");
                return;
            } 
            tmp = Instantiate(prefab, previewObject.transform.position, Quaternion.identity);
            tmp.AddComponent<collisionDetector>();
            tmp.AddComponent<EditObject>();
            //Debug.Log("Placing Tile");
            Tiles.Add(tmp);
            lastTileSpawnPosition = tmp.transform.position;
            SaveAction(tmp, true);
            return;
        }
        if (cd.isColliding || !cd.isOnMap ) {
            displayErrorMessage("Cannot place object here!");
            Debug.Log("Cannot place object here!");
            return;
        }
        tmp = Instantiate(prefab, previewObject.transform.position, previewObject.transform.rotation);
        tmp.AddComponent<collisionDetector>();
        tmp.AddComponent<EditObject>();

        if (previewObject.CompareTag("Fence"))
        {
            FenceGenerator fenceGenerator = tmp.GetComponent<FenceGenerator>();
            fenceGenerator.setIsPreview(true);
            fenceGenerator.setFenceController();
        }
        SaveAction(tmp, true);
        // SetMode(editState.Editting); // Commenting this out so that user can continue to place objects.
    }

    public void setPreview()
    {
        Preview?.Invoke();
        GameObject tmp = previewObject;
        GameObject prefab = registry.GetPrefab(currentObject);
        previewObject = Instantiate(prefab, previewObject.transform.position, Quaternion.identity);
        Destroy(tmp);
        previewObject.AddComponent<collisionDetector>();
        LevelObjectInfo info = previewObject.GetComponent<LevelObjectInfo>();
        if (info != null)
        {
            Destroy(info); // Removes LevelObjectInfor from the preview object so it doesn't get saved.
        }
        // Makes sure the preview object does not effect the path finding.
        previewObject.layer = LayerMask.NameToLayer("UI");
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
                displayErrorMessage("Not all tiles are connected");
                Debug.Log("Not all tiles are connected");
                return false;
            }
        }
        return true;
    }
    // Returns distance needed to lift object above y = 0
    public float LiftAboveZero(GameObject liftGameObject)
    {
        // Get the Bounds of the object (includes all children)
        Bounds combinedBounds = GetTargetBounds(liftGameObject);

        // Calculate how far the bottom of the bounds is from y = 0
        float bottomY = combinedBounds.min.y;

        if (bottomY < 0)
        {
            // Lift the object by the difference
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
    // Overloaded funciton for internal use.
    public void SetMode(editState state)
    {
        if(state == editState.Editting)
        {
            currentEditState = editState.Editting;
            editBttonText.SetText("Set");
            previewObject.SetActive(false);
        }
        else if (state == editState.Setting)
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
        FloatingHint hintsManager = FindAnyObjectByType<FloatingHint>();

        foreach(TileRegistry.TileEntry te in registry.entries)
        {
            Debug.Log("Creating button for: " + te.prefab.name);
            // Create the Button Root
            GameObject btnObj = Instantiate(buttonPrefab, contentParent);
            btnObj.name = te.prefab.name + "_Button";
            
            // Add UI Visuals (Buttons need an Image to be clickable!)
            btnObj.AddComponent<CanvasRenderer>();
            btnObj.AddComponent<Image>(); 
            btnObj.AddComponent<PassScrollToParent>();
            ButtonRegistry registryComponent = btnObj.AddComponent<ButtonRegistry>();
            Button btn = btnObj.AddComponent<Button>();

            // Set the ID of the button to match its registry object
            registryComponent.id = te.id;

            // Create a Child Object for the Text
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(btnObj.transform, false);

            // Resize the text Object
            RectTransform rectTransform = textObj.AddComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(50, 25);
            
            // Use TextMeshProUGUI, NOT TMP_Text
            TextMeshProUGUI btnText = textObj.AddComponent<TextMeshProUGUI>();
            btnText.font = newFont;
            btnText.text = te.prefab.name;
            btnText.fontSize = 10;
            btnText.alignment = TextAlignmentOptions.Center;
            btnText.color = Color.black;

            // Floating hint text
            HintText hint = textObj.AddComponent<HintText>();
            hint.hintMessage = te.description;//hintMessages[registry.entries.IndexOf(te)];

            // Setting Highlight color
            ColorBlock cb = btn.colors;
            cb.highlightedColor = new Color(0.4f, 1f, 1f); // Light gray highlight
            btn.colors = cb; // Apply the modified color block

            previewButtons.Add(btn);

            // Setting event trigger functions
            EventTrigger trigger = btnObj.GetComponent<EventTrigger>();
            if (trigger == null) trigger = btnObj.AddComponent<EventTrigger>();

            // Create a new Entry for a PointerEnter event type
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerEnter;
            entry.callback.AddListener((data) => { hintsManager.StartFloatingHint((BaseEventData)data); });
            trigger.triggers.Add(entry);

            // Create a new Entry for a PointerExist event type
            EventTrigger.Entry entry2 = new EventTrigger.Entry();
            entry2.eventID = EventTriggerType.PointerExit;
            entry2.callback.AddListener((data) => { hintsManager.StopFloatingHint(); });
            trigger.triggers.Add(entry2);
        }
    }

    public void PlayTestLevel()
    {
        SceneManager.LoadScene("TestLoadLevel");
    }
    // Functions for displaying error messages related to the level editor
    private Coroutine messageRoutine;
    public void displayErrorMessage(string message)
    {
        if (messageRoutine != null)
        {
            StopCoroutine(messageRoutine);
        }
        
        errorMessages.SetText(message);
        messageRoutine = StartCoroutine(errorMessageTime());
    }
    
    public System.Collections.IEnumerator errorMessageTime()
    {
        yield return new WaitForSeconds(5);
        errorMessages.SetText("");
    }
    // Undo Related functions
    public void SaveAction(GameObject saveObject, bool placeAction = false)
    {
        Debug.Log("Saving Action " + saveObject.transform.position);
        undoClass action = new undoClass();
        action.savedPosition = saveObject.transform.position;
        action.savedRotation = saveObject.transform.rotation;
        action.activeGameObject = saveObject;

        if(mostRecentAction == null)
        {
            mostRecentAction = action;
            return;
        }
        if (undoClassList.atCapacity())
        {
            Debug.Log("At capacity");
            GameObject tmp = undoClassList.lastItem().activeGameObject;
            if (!tmp.activeSelf)
            {
                Destroy(tmp);
            }
        }
        if (placeAction)
        {
            action.placeAction = true;
        }
        undoClassList.Push(action);
        //mostRecentAction = action;
    }

    public void undoAction()
    {
        if (undoClassList.isEmpty())
        {
            displayErrorMessage("No more actions to undo");
            return;
        }
        if (isEditingObject)
        {
            displayErrorMessage("Can't undo while editting object.");
            return;
        }
        undoClass action = undoClassList.Pop();
        action.activeGameObject.transform.position = action.savedPosition;
        action.activeGameObject.transform.rotation = action.savedRotation;
        if (!action.activeGameObject.activeSelf)
        {
            action.activeGameObject.SetActive(true);
        }

        if (action.placeAction)
        {
            Destroy(action.activeGameObject);
        }
    }
}
