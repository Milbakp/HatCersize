using UnityEngine;
using UnityEngine.EventSystems;

public class LDCameraMovement : MonoBehaviour
{
    public Camera camera;
    public float cameraSpeed = 30f;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject()) {
        // The mouse is over a UI element
            return;
        }
        // Move camera with WASD keys
        if(Input.GetKey(KeyCode.A))
        {
            camera.transform.position += new Vector3(-1, 0, 0) * Time.deltaTime * cameraSpeed;
        }
        if(Input.GetKey(KeyCode.D))
        {
            camera.transform.position += new Vector3(1, 0, 0) * Time.deltaTime * cameraSpeed;
        }
        if(Input.GetKey(KeyCode.W))
        {
            camera.transform.position += new Vector3(0, 0, 1) * Time.deltaTime * cameraSpeed;
        }
        if(Input.GetKey(KeyCode.S))
        {
            camera.transform.position += new Vector3(0, 0, -1) * Time.deltaTime * cameraSpeed;
        }

        float scrollDelta = Input.GetAxis("Mouse ScrollWheel");
        if(scrollDelta > 0)
        {
            camera.transform.position += new Vector3(0, -1, 0) * Time.deltaTime * 200;
        }
        if(scrollDelta < 0)
        {
            camera.transform.position += new Vector3(0, 1, 0) * Time.deltaTime * 200;
        }
    }
    // Function to adjust the camera zoom based on the size of a loaded level.
    public void AdjustZoomForObject(GameObject targetObject)
    {
        // Get all renderers attached to the parent and its children
        Renderer[] renderers = targetObject.GetComponentsInChildren<Renderer>();
        
        if (renderers.Length == 0) return;

        // Start with the bounds of the first renderer
        Bounds completeBounds = renderers[0].bounds;

        // Encapsulate (expand) the bounds to include every other renderer
        for (int i = 1; i < renderers.Length; i++)
        {
            completeBounds.Encapsulate(renderers[i].bounds);
        }

        // Use the combined size for your calculations
        float objectSize = Mathf.Max(completeBounds.size.x, completeBounds.size.z);

        float padding = 1.5f;
        float distance = (objectSize * padding) / Mathf.Tan(camera.fieldOfView * 0.5f * Mathf.Deg2Rad);
            
        Vector3 newPosition = transform.position;
        // Optional: You can also center the camera on the combined bounds center
        newPosition.x = completeBounds.center.x;
        newPosition.z = completeBounds.center.z;
        newPosition.y = completeBounds.center.y + distance; 
        transform.position = newPosition;
    }
}
