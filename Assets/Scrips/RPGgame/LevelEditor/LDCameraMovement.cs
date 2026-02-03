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
}
