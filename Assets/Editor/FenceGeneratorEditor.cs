using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(FenceGenerator))]
public class FenceGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw the default inspector fields (prefabs, width, etc.)
        DrawDefaultInspector();

        FenceGenerator fenceGen = (FenceGenerator)target;

        // Add a nice spacing
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Editor Controls", EditorStyles.boldLabel);

        // Create an interactive slider for the length
        EditorGUI.BeginChangeCheck();
        float newLength = EditorGUILayout.Slider("Interactive Length", fenceGen.fenceLength, 1f, 100f);
        
        // If the slider moves, update the value and regenerate the fence instantly
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(fenceGen, "Change Fence Length");
            fenceGen.fenceLength = newLength;
            fenceGen.GenerateFence();
        }

        // Manual override buttons just in case
        if (GUILayout.Button("Force Regenerate"))
        {
            fenceGen.GenerateFence();
        }

        if (GUILayout.Button("Clear Fence"))
        {
            fenceGen.ClearFence();
        }
        
    }
}
