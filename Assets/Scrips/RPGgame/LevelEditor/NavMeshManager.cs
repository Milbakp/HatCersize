using UnityEngine;
using Unity.AI.Navigation;

public class NavMeshManager : MonoBehaviour
{
    public NavMeshSurface surface;
    public GameObject startPoint;
    public GameObject endPoint;
    void Start()
    {
        startPoint = new GameObject();
        endPoint = new GameObject();
    }
    void Update()
    {
        // Testing
        if (Input.GetKeyDown(KeyCode.N))
        {
            createNavMesh();
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            IsPathAvailable();
        }
    }

    public void createNavMesh()
    {
        surface.BuildNavMesh();
    }
    public bool setStartAndEnd()
    {
        startPoint = GameObject.FindWithTag("StartPosition");
        endPoint = GameObject.FindWithTag("EndPosition");
        return startPoint != null && endPoint != null;
    }

    public bool IsPathAvailable()
    {
        if (!setStartAndEnd())
        {
            Debug.Log("Start or End point not set");
            return false;
        }
        UnityEngine.AI.NavMeshPath path = new UnityEngine.AI.NavMeshPath();
        
        // Calculate the path from A to B
        // The 'true' parameter allows for partial paths (if B is unreachable but a nearby point is)
        Debug.Log(startPoint.transform.position);
        Debug.Log(endPoint.transform.position);
        UnityEngine.AI.NavMesh.CalculatePath(startPoint.transform.position, endPoint.transform.position, UnityEngine.AI.NavMesh.AllAreas, path);

        // Check if the path status is "PathComplete"
        if (path.status == UnityEngine.AI.NavMeshPathStatus.PathComplete)
        {
            Debug.Log("Path exists");
            return true;
        }
        Debug.Log("No Path exists");
        return false;
    }
}
