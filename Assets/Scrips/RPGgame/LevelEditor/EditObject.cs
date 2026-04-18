using UnityEngine;

public class EditObject : MonoBehaviour
{
    public Vector3 originalPosition;
    public Quaternion originalRotation;
    public bool isEditing;
    private Camera camera;
    private LevelEditorManager levelEditorManager;
    private collisionDetector cd;

    void Start()
    {
        isEditing = false;
        camera = Camera.main;
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        levelEditorManager = FindAnyObjectByType<LevelEditorManager>();
        cd = GetComponent<collisionDetector>();
    }

    // Update is called once per frame
    void Update()
    {
        // Makes sure action only happens in edit mode
        if(levelEditorManager.getMode() != LevelEditorManager.editState.Editting)
        {
            return;
        }
        if (isEditing)
        {
            edit();
        } 
        if (Input.GetMouseButtonDown(1) && isEditing) 
        {
            isEditing = false;
            transform.position = originalPosition;
            transform.rotation = originalRotation;
            levelEditorManager.isEditingObject = false;
        }
        if (Input.GetKeyDown(KeyCode.X) && isEditing)
        {
            levelEditorManager.isEditingObject = false;
            Destroy(gameObject);
        }
        if (Input.GetKeyDown(KeyCode.R) && isEditing)
        {
            transform.Rotate(0, 90, 0);
        }
    }
    private void OnMouseDown() {
        if(levelEditorManager.getMode() != LevelEditorManager.editState.Editting)
        {
            return;
        }
        if (isEditing && isCollision())
        {
            isEditing = false;
            originalPosition = transform.position;
            originalRotation = transform.rotation;
            levelEditorManager.isEditingObject = false;
        }
        else if (!levelEditorManager.isEditingObject)
        {
            isEditing = true;
            levelEditorManager.isEditingObject = true;
        }
    }
    private void edit()
    {
        Vector3 mousePos = Input.mousePosition;
        // Sets distance of the object relative to the camera so the object appears under the mouse cursor
        mousePos.z = camera.transform.position.y;
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePos);
        transform.position =  new Vector3 (worldPosition.x, transform.position.y, worldPosition.z);
    }

    private bool isCollision()
    {
        if(gameObject.CompareTag("Tile"))
        {
            if (cd.isOnMap || cd.isColliding)
            {
                levelEditorManager.displayErrorMessage("Cannot place tile here!");
                Debug.Log("Can not place Tile");
                return false;
            } 
            levelEditorManager.displayErrorMessage("Tile placed.");
            Debug.Log("Placing Tile");
            return true;
        }
        if (cd.isColliding || !cd.isOnMap ) {
            levelEditorManager.displayErrorMessage("Cannot place object here!");
            Debug.Log("Cannot place object here!");
            return false;
        }
        return true;
    }
}
