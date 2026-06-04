using System.Collections.Generic;
using UnityEngine;

public class FenceGenerator : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject postPrefab;
    public GameObject panelPrefab;

    [Header("Settings")]
    [Tooltip("The total desired length of the fence.")]
    public float fenceLength = 4f;
    
    [Tooltip("The exact physical width of your panel prefab.")]
    public float panelWidth = 2f;

    // Keep track of spawned pieces so we can clean them up
    private List<GameObject> spawnedPieces = new List<GameObject>();

    [Header("Collider Settings")]
    public bool autoGenerateCollider = true;
    [Tooltip("How thick the collider box should be on the Z axis.")]
    public float colliderThickness = 0.2f;
    [Tooltip("How high the collider box should be on the Y axis.")]
    public float colliderHeight = 2.0f;
    private FenceUIController fenceUIController;
    private bool isPreview;
    void Awake()
    {
        fenceUIController = FindAnyObjectByType<FenceUIController>();
        isPreview = false;
    }

    // This is called automatically when values change in the Inspector
    public void GenerateFence()
    {
        ClearFence();

        if (postPrefab == null || panelPrefab == null || panelWidth <= 0) return;

        // Calculate how many full panels fit into the length
        int numberOfPanels = Mathf.Max(1, Mathf.RoundToInt(fenceLength / panelWidth));

        for (int i = 0; i < numberOfPanels; i++)
        {
            float currentX = i * panelWidth;

            // Spawn Post
            GameObject post = Instantiate(postPrefab, transform);
            post.transform.localPosition = new Vector3(currentX, 0, 0);
            spawnedPieces.Add(post);

            // Spawn Panel
            GameObject panel = Instantiate(panelPrefab, transform);
            panel.transform.localPosition = new Vector3(currentX, 2, 0);

            GameObject panel2 = Instantiate(panelPrefab, transform);
            panel2.transform.localPosition = new Vector3(currentX, 4, 0);

            GameObject panel3 = Instantiate(panelPrefab, transform);
            panel3.transform.localPosition = new Vector3(currentX, 6, 0);

            GameObject panel4 = Instantiate(panelPrefab, transform);
            panel4.transform.localPosition = new Vector3(currentX, 8, 0);
            
            // Optional: Scale the last panel if you want it to perfectly match a precise float length
            // For simplicity, this guide snaps to the nearest full panel width.
            
            spawnedPieces.Add(panel);
            spawnedPieces.Add(panel2);
            spawnedPieces.Add(panel3);
            spawnedPieces.Add(panel4);
        }

        // Spawn the final closing Post
        GameObject finalPost = Instantiate(postPrefab, transform);
        finalPost.transform.localPosition = new Vector3(numberOfPanels * panelWidth, 0, 0);
        spawnedPieces.Add(finalPost);

        if (autoGenerateCollider)
        {
            UpdateBoxCollider();
        }
    }

    public void ClearFence()
    {
        // Delete existing pieces safely depending on whether we are in the Editor or Playing
        for (int i = spawnedPieces.Count - 1; i >= 0; i--)
        {
            if (spawnedPieces[i] != null)
            {
                if (Application.isPlaying)
                {
                    Destroy(spawnedPieces[i]); // Safe for Runtime gameplay
                }
                else
                {
                    DestroyImmediate(spawnedPieces[i]); // Safe for Editor design
                }
            }
        }
        spawnedPieces.Clear();

        // Backup cleanup to catch stray children
        List<GameObject> children = new List<GameObject>();
        foreach (Transform child in transform) children.Add(child.gameObject);
        
        foreach (GameObject child in children)
        {
            if (Application.isPlaying) Destroy(child);
            else DestroyImmediate(child);
        }

        // Reset collider if it exists
        BoxCollider boxCollider = GetComponent<BoxCollider>();
        if (boxCollider != null)
        {
            boxCollider.center = Vector3.zero;
            boxCollider.size = Vector3.zero;
        }
    }

    public void UpdateBoxCollider()
    {
        // Get or add the BoxCollider component on this root object
        BoxCollider boxCollider = GetComponent<BoxCollider>();
        if (boxCollider == null)
        {
            boxCollider = gameObject.AddComponent<BoxCollider>();
        }

        // If the fence has no length, clear the collider sizes
        if (fenceLength <= 0)
        {
            boxCollider.center = Vector3.zero;
            boxCollider.size = Vector3.zero;
            return;
        }

        // Calculate the total actual width spanned by the panels
        int numberOfPanels = Mathf.Max(1, Mathf.RoundToInt(fenceLength / panelWidth));
        float totalActualLength = numberOfPanels * panelWidth;

        // CENTER CALCULATION:
        // Because the pivot point is at (0,0,0) on the first post, the center of the 
        // collider must be pushed out by half of the total length on the X axis.
        // We lift the Y center by half the height so the bottom aligns with the ground.
        Vector3 center = new Vector3(totalActualLength / 2f, colliderHeight / 2f, 0f);

        // SIZE CALCULATION:
        Vector3 size = new Vector3(totalActualLength, colliderHeight, colliderThickness);

        // Apply the values
        boxCollider.center = center;
        boxCollider.size = size;
    }
    private void OnMouseDown() {
        if (isPreview)
        {
            setFenceController();
        }
    }
    public void setIsPreview(bool isPreviewInput)
    {
        this.isPreview = isPreviewInput;
    }

    public void setFenceController()
    {
        // Setting Slider UI to the selected fence.
        fenceUIController.fenceGenerator = this;
        fenceUIController.sliderUI.SetActive(true);
        // Setting the slider values to the current fence.
        fenceUIController.lengthSlider.value = fenceUIController.fenceGenerator.fenceLength;
        fenceUIController.UpdateTextDisplay(fenceUIController.fenceGenerator.fenceLength);
    }

}