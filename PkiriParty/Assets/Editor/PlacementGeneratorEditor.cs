using UnityEditor;
using UnityEngine;

// Custom Editor for PlacementGenerator
[CustomEditor(typeof(PlacementGenerator))]
public class PlacementGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw the default inspector UI
        DrawDefaultInspector();

        // Get the target script instance
        PlacementGenerator script = (PlacementGenerator)target;

        // Add a separator for better UI organization
        GUILayout.Space(10);
        EditorGUILayout.LabelField("Poisson Disk Sampling Controls", EditorStyles.boldLabel);

        // Add a button to generate points
        if (GUILayout.Button("Generate Trees"))
        {
            script.ClearTrees();  // Clear existing trees
            script.GeneratePoints();  // Generate new trees
        }

        // Add a button to clear all trees
        if (GUILayout.Button("Clear Trees"))
        {
            script.ClearTrees();
        }
    }
}
