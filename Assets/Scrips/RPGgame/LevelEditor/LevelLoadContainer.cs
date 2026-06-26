using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelLoadContainer : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private LevelEditorManager levelEditorManager;
    public Camera camera;
    private Tile tile;
    //public GameObject collisionDetectorChild;
    //private collisionDetector cd;
    List<collisionDetector> CDchild = new List<collisionDetector>();
    List<collisionDetector> CDTiles = new List<collisionDetector>();
    void Start()
    {
        levelEditorManager = FindObjectOfType<LevelEditorManager>();
        camera = Camera.main;
        tile = GetComponent<Tile>();
        tile.enabled = false;
        tile.enabled = true;
        //cd = collisionDetectorChild.GetComponent<collisionDetector>();
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
            levelEditorManager.SaveAction(gameObject, true);
            levelEditorManager.SetMode(LevelEditorManager.editState.Editting);
            //collisionDetectorChild.SetActive(false);
            this.enabled = false; // Disables this script so the object is no longer following the mouse.
        }
        if (Input.GetMouseButtonDown(1))
        {
            levelEditorManager.SetMode(LevelEditorManager.editState.Editting);
            Destroy(gameObject);
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
        //collisionDetector[] CDchild = GetComponentsInChildren<collisionDetector>();
        
        // foreach (collisionDetector cd in CDchild)
        // {
        //     if (cd.isColliding || !cd.isOnMap ) {
        //         levelEditorManager.displayErrorMessage("Level Object is overlapping with other objects.");
        //         Debug.Log("Cannot place object here!");
        //         Debug.LogError("Colliding with: " + cd.gameObject.name);
        //         return false;
        //     }
        //     // Skip the parent containers
        //     // if (renderer.gameObject == gameObject || (collisionDetectorChild != null && renderer.gameObject == collisionDetectorChild)) 
        //     //     continue;

        //     // // If the child tile doesn't have a collider, add one that fits it perfectly
        //     // if (!renderer.gameObject.TryGetComponent<Collider>(out var childCollider))
        //     // {
        //     //     // BoxCollider automatically sizes itself to a MeshFilter/MeshRenderer when added
        //     //     renderer.gameObject.AddComponent<BoxCollider>();
        //     // }
        // }
        foreach (collisionDetector cd in CDTiles)
        {
            if (cd.isOnMap) {
                levelEditorManager.displayErrorMessage("Level tile is overlapping with other objects.");
                Debug.Log("Cannot place object here!");
                Debug.LogError("Colliding with: " + cd.gameObject.name);
                return false;
            }
        }
        // if (cd.isColliding || cd.isOnMap ) {
        //     levelEditorManager.displayErrorMessage("Level is overlapping with other objects.");
        //     Debug.Log("Cannot place object here!");
        //     return false;
        // }
        return true;
    }
    public void ResizeColliderToFitChildren()
    {
        //GameObject[] childObjects = GetComponentsInChildren<GameObject>();
        foreach (Transform childObject in transform)
        {
            collisionDetector cd = childObject.gameObject.GetComponent<collisionDetector>();
            if(childObject.gameObject.CompareTag("Tile") && cd != null)
            {
                CDTiles.Add(cd);
                continue;
            }
            if (cd != null)
            {
                CDchild.Add(cd);
            }
        }
        // BoxCollider parentCollider = collisionDetectorChild.GetComponent<BoxCollider>();
        // // Get all SpriteRenderers or MeshRenderers from children
        // // (Change to MeshRenderer if you are making a 3D game)
        // Renderer[] childRenderers = GetComponentsInChildren<Renderer>();

        // if (childRenderers.Length == 0)
        // {
        //     // If no children, reset collider to zero or a default size
        //     parentCollider.size = Vector3.zero;
        //     parentCollider.center = Vector3.zero;
        //     return;
        // }

        // // Initialize the bounds using the first child's bounds
        // Bounds combinedBounds = childRenderers[0].bounds;

        // // Loop through the rest of the children and expand the bounds to fit them
        // for (int i = 1; i < childRenderers.Length; i++)
        // {
        //     // Skip the parent's own renderer if it has one, so it doesn't skew the math
        //     if (childRenderers[i].gameObject == gameObject) continue;

        //     combinedBounds.Encapsulate(childRenderers[i].bounds);
        // }

        // // Convert global World Bounds back into the Parent's Local Space
        // // Crucial: BoxCollider center/size are relative to the GameObject it is attached to!
        // Vector3 localCenter = transform.InverseTransformPoint(combinedBounds.center);
        
        // // LossyScale prevents distortion if the parent object has been scaled
        // Vector3 localSize = new Vector3(
        //     combinedBounds.size.x / transform.lossyScale.x,
        //     combinedBounds.size.y / transform.lossyScale.y,
        //     combinedBounds.size.z / transform.lossyScale.z
        // );

        // // Apply the calculated math to the BoxCollider
        // parentCollider.center = localCenter;
        // parentCollider.size = localSize;
        // IgnoreChildren();
    }

    // public void IgnoreChildren()
    // {
    //     // Get the collider on the parent object
    //     Collider parentCollider = collisionDetectorChild.GetComponent<Collider>();
        
    //     if (parentCollider == null) return;

    //     // Get all colliders on the children
    //     Collider[] childColliders = GetComponentsInChildren<Collider>();

    //     // Loop through and tell Physics to ignore the pairing
    //     foreach (Collider childCollider in childColliders)
    //     {
    //         // Skip if it accidentally grabs the parent's own collider
    //         if (childCollider == parentCollider) continue;

    //         // Fuses the two colliders into a mutual ignore state
    //         Physics.IgnoreCollision(parentCollider, childCollider, true);
    //     }
    // }
}
