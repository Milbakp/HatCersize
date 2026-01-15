using UnityEngine;

public class EditObject : MonoBehaviour
{
    public Vector3 originalPosition;
    public bool isEditing;
    private Camera camera;

    void Start()
    {
        isEditing = false;
        camera = Camera.main;
        originalPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (isEditing)
        {
            edit();
        } 
        if (Input.GetMouseButtonDown(1) && isEditing) 
        {
            isEditing = false;
            transform.position = originalPosition;
        }
    }
    private void OnMouseDown() {
        if (isEditing)
        {
            isEditing = false;
            originalPosition = transform.position;
        }
        else
        {
            isEditing = true;
        }
    }
    private void edit()
    {
        Vector3 mousePos = Input.mousePosition;
        // Sets distance of the object relative to the camera so the object appears under the mouse cursor
        mousePos.z = camera.transform.position.y;
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePos);
        worldPosition.y = 0; 
        transform.position = worldPosition;
    }
}
