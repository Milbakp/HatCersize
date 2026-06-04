using UnityEngine;

public class LevelLoadContainer : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private LevelEditorManager levelEditorManager;
    public Camera camera;
    private Tile tile;
    public GameObject collisionDetectorChild;
    private collisionDetector cd;
    void Start()
    {
        levelEditorManager = FindObjectOfType<LevelEditorManager>();
        camera = Camera.main;
        tile = GetComponent<Tile>();
        tile.enabled = false;
        tile.enabled = true;
        cd = collisionDetectorChild.GetComponent<collisionDetector>();
        levelEditorManager.SetMode(LevelEditorManager.editState.LoadingLevel);
        levelEditorManager.previewObject.SetActive(false);
        //setPreview();
    }

    // Update is called once per frame
    void Update()
    {
        preview();
        if (Input.GetMouseButtonDown(0) && isCollision())
        {
            levelEditorManager.SetMode(LevelEditorManager.editState.Editting);
            collisionDetectorChild.SetActive(false);
            this.enabled = false; // Disables this script so the object is no longer following the mouse.
        }
        if(Input.GetKeyDown(KeyCode.L))
        {
            ResizeColliderToFitChildren();
        }
    }
    private void preview()
    {
        Vector3 mousePos = Input.mousePosition;
        // Sets distance of the object relative to the camera so the object appears under the mouse cursor
        mousePos.z = camera.transform.position.y;
        Vector3 worldPosition = camera.ScreenToWorldPoint(mousePos);
        //worldPosition.y = 0; 
        gameObject.transform.position =  new Vector3 (worldPosition.x, gameObject.transform.position.y, worldPosition.z);
    }

    private bool isCollision()
    {
        if (cd.isColliding || cd.isOnMap ) {
            levelEditorManager.displayErrorMessage("Level is overlapping with other objects.");
            Debug.Log("Cannot place object here!");
            return false;
        }
        return true;
    }
    public void ResizeColliderToFitChildren()
    {
        BoxCollider parentCollider = collisionDetectorChild.GetComponent<BoxCollider>();
        // Get all SpriteRenderers or MeshRenderers from children
        // (Change to MeshRenderer if you are making a 3D game)
        Renderer[] childRenderers = GetComponentsInChildren<Renderer>();

        if (childRenderers.Length == 0)
        {
            // If no children, reset collider to zero or a default size
            parentCollider.size = Vector3.zero;
            parentCollider.center = Vector3.zero;
            return;
        }

        // 2. Initialize the bounds using the first child's bounds
        Bounds combinedBounds = childRenderers[0].bounds;

        // 3. Loop through the rest of the children and expand the bounds to fit them
        for (int i = 1; i < childRenderers.Length; i++)
        {
            // Skip the parent's own renderer if it has one, so it doesn't skew the math
            if (childRenderers[i].gameObject == gameObject) continue;

            combinedBounds.Encapsulate(childRenderers[i].bounds);
        }

        // 4. Convert global World Bounds back into the Parent's Local Space
        // Crucial: BoxCollider center/size are relative to the GameObject it is attached to!
        Vector3 localCenter = transform.InverseTransformPoint(combinedBounds.center);
        
        // LossyScale prevents distortion if the parent object has been scaled
        Vector3 localSize = new Vector3(
            combinedBounds.size.x / transform.lossyScale.x,
            combinedBounds.size.y / transform.lossyScale.y,
            combinedBounds.size.z / transform.lossyScale.z
        );

        // 5. Apply the calculated math to the BoxCollider
        parentCollider.center = localCenter;
        parentCollider.size = localSize;
        IgnoreChildren();
    }

    public void IgnoreChildren()
    {
        // 1. Get the collider on the parent object
        Collider parentCollider = collisionDetectorChild.GetComponent<Collider>();
        
        if (parentCollider == null) return;

        // 2. Get all colliders on the children
        Collider[] childColliders = GetComponentsInChildren<Collider>();

        // 3. Loop through and tell Physics to ignore the pairing
        foreach (Collider childCollider in childColliders)
        {
            // Skip if it accidentally grabs the parent's own collider
            if (childCollider == parentCollider) continue;

            // Fuses the two colliders into a mutual ignore state
            Physics.IgnoreCollision(parentCollider, childCollider, true);
        }
    }
}
