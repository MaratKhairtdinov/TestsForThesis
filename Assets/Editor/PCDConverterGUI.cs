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

        //converter.outputFileLocation = EditorGUILayout.TextField("Output file path: ", converter.outputFileLocation);

        converter.fileName = EditorGUILayout.TextField("File name: ", converter.fileName);
        EditorGUILayout.Space();
        converter.inner_surface = EditorGUILayout.Toggle("Inner surface", converter.inner_surface);
        SerializedProperty collector = serializedObject.FindProperty("collector");
        EditorGUILayout.PropertyField(collector, new GUIContent("Vertex collector"), true);
        serializedObject.ApplyModifiedProperties();

        if (GUILayout.Button("Convert"))
        {
            converter.ConvertFile();
        }
    }
}
