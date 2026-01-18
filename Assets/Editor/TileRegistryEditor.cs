using UnityEngine;
using UnityEditor;
using System.Collections.Generic;


[CustomEditor(typeof(TileRegistry))]
public class TileRegistryEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TileRegistry registry = (TileRegistry)target;

        if (GUILayout.Button("Auto-Populate from Folder"))
        {
            PopulateRegistry(registry);
        }
    }

    private void PopulateRegistry(TileRegistry registry)
    {
        // Path to your prefabs folder (e.g., Assets/Prefabs/LevelObjects)
        string folderPath = "Assets/Prefabs"; 
        
        // Find all prefab GUIDs in that folder
        string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { folderPath });

        if(registry.entries.Count == 0 || registry.entries == null)
        {
            registry.entries = new List<TileRegistry.TileEntry>();
        }

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            LevelObjectInfo info = prefab.GetComponent<LevelObjectInfo>();

            if (info != null)
            {
                if (info.tileID == 0) 
                {
                    // Logic to find the next available ID
                    info.tileID = registry.entries.Count + 1; 
                    EditorUtility.SetDirty(prefab); // Save the ID to the prefab
                    registry.entries.Add(new TileRegistry.TileEntry 
                    { 
                        id = info.tileID, 
                        prefab = prefab 
                    });
                }
                
            }
        }

        EditorUtility.SetDirty(registry);
        AssetDatabase.SaveAssets();
        Debug.Log($"Registry updated with {registry.entries.Count} items.");
    }
}