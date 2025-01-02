using UnityEditor;
using UnityEngine;

// Custom Editor for PlacementGenerator
[CustomEditor(typeof(PlacementGenerator))]
public class PlacementGeneratorEditor : Editor
{
    //public override void OnInspectorGUI()
    //{
    //    // Draw the default inspector UI
    //    DrawDefaultInspector();

    //    // Get the target script instance
    //    PlacementGenerator script = (PlacementGenerator)target;

    //    // Add a separator for better UI organization
    //    GUILayout.Space(10);
    //    EditorGUILayout.LabelField("Poisson Disk Sampling Controls", EditorStyles.boldLabel);

    //    // Add a button to generate points
    //    if (GUILayout.Button("Generate Trees"))
    //    {
    //        script.ClearObjects();  // Clear existing trees
    //        script.GeneratePoints();  // Generate new trees
    //    }

    //    // Add a button to clear all trees
    //    if (GUILayout.Button("Clear Trees"))
    //    {
    //        script.ClearObjects();
    //    }
    //}

    private SerializedProperty prefabCategories;

    private void OnEnable()
    {
        prefabCategories = serializedObject.FindProperty("prefabCategories");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.LabelField("Prefab Placement Settings", EditorStyles.boldLabel);

        if (GUILayout.Button("Add Prefab Category"))
        {
            prefabCategories.InsertArrayElementAtIndex(prefabCategories.arraySize);
        }

        for (int i = 0; i < prefabCategories.arraySize; i++)
        {
            SerializedProperty category = prefabCategories.GetArrayElementAtIndex(i);
            SerializedProperty name = category.FindPropertyRelative("Name");
            SerializedProperty prefab = category.FindPropertyRelative("Prefab");
            SerializedProperty count = category.FindPropertyRelative("Count");
            SerializedProperty spawnAreaMin = category.FindPropertyRelative("SpawnAreaMin");
            SerializedProperty spawnAreaMax = category.FindPropertyRelative("SpawnAreaMax");

            EditorGUILayout.Space();
            EditorGUILayout.LabelField($"Category {i + 1}", EditorStyles.boldLabel);

            name.stringValue = EditorGUILayout.TextField("Name", name.stringValue);
            prefab.objectReferenceValue = EditorGUILayout.ObjectField("Prefab", prefab.objectReferenceValue, typeof(GameObject), false);
            count.intValue = EditorGUILayout.IntField("Count", count.intValue);
            spawnAreaMin.vector2Value = EditorGUILayout.Vector2Field("Spawn Area Min", spawnAreaMin.vector2Value);
            spawnAreaMax.vector2Value = EditorGUILayout.Vector2Field("Spawn Area Max", spawnAreaMax.vector2Value);

            if (GUILayout.Button("Remove Category"))
            {
                prefabCategories.DeleteArrayElementAtIndex(i);
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}
