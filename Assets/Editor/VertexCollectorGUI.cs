using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(VertexCollector))]
public class VertexCollectorGUI : Editor
{
    VertexCollector collector;

    public override void OnInspectorGUI()
    {
        SerializedProperty parent = serializedObject.FindProperty("parentNode");
        EditorGUILayout.PropertyField(parent, new GUIContent("Parent node"), true);
        serializedObject.ApplyModifiedProperties();
        collector = (VertexCollector)target;
        if (GUILayout.Button("Collect vertices"))
        {
            collector.Collect();
        }
        if (GUILayout.Button("Draw"))
        {
            collector.Draw();
        }
    }
}
