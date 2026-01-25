using System;
using System.Runtime.Serialization;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public int tileID;
    public bool tileIsFilled;
    public GameObject currentObject;
    [SerializeField] LevelEditorManager levelEditorManager;
    public bool connectedToMap;
    private float tileLockWidth = 10.0f;

    public void Start()
    {
        connectedToMap = false;
        tileIsFilled = false;
        levelEditorManager = FindAnyObjectByType<LevelEditorManager>();
    }
    private void OnMouseDown() {
        levelEditorManager.setCurrentTile(tileID);
    }
    public void Update()
    {
        transform.position = new Vector3(RoundToNearestMultiple(transform.position.x, tileLockWidth
        ), transform.position.y, RoundToNearestMultiple(transform.position.z, tileLockWidth) );
    }

    public static float RoundToNearestMultiple(float value, float multiple)
    {
        if (multiple == 0)
        {
            return value;
        }

        // Divide the number by the multiple, round it, and multiply back by the multiple
        return (float)Math.Round(value / multiple) * multiple;
    }
}
