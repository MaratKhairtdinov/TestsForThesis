using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PCDWriter))]
public class PCDConverterGUI : Editor
{
    PCDWriter converter;
    public override void OnInspectorGUI()
    {
        converter = (PCDWriter)target;

        converter.outputFileLocation = EditorGUILayout.TextField("Output file path: ", converter.outputFileLocation);
        EditorGUILayout.Space();
        SerializedProperty collector = serializedObject.FindProperty("collector");
        EditorGUILayout.PropertyField(collector, new GUIContent("Vertex collector"), true);
        serializedObject.ApplyModifiedProperties();

        if (GUILayout.Button("Convert"))
        {
            converter.ConvertFile();
        }
    }
}
