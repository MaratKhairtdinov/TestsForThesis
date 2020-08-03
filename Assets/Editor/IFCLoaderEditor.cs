﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(IfcLoader))]
public class IFCLoaderEditor : Editor
{
    IfcLoader loader;
    public override void OnInspectorGUI()
    {
        loader = (IfcLoader)target;
        //loader.LoadObj();
        SerializedProperty vertexCollector = serializedObject.FindProperty("vertexCollector");
        EditorGUILayout.PropertyField(vertexCollector, new GUIContent("VertexCollector: "), true);
        EditorGUILayout.Space();
        loader.filePath = EditorGUILayout.TextField("File path: ", loader.filePath);
        loader.fileName = EditorGUILayout.TextField("File name" + ": ", loader.fileName);
        

        if (GUILayout.Button("Convert IFC"))
        {
            loader.ConvertIFC();
        }
        /*
        if (GUILayout.Button("Load *.OBJ"))
        {
            loader.LoadObj();
        }

        if (GUILayout.Button("Load *.XML"))
        {
            loader.LoadXML();
        }
        */
        if (GUILayout.Button("Filter nodes"))
        {
            loader.Filter();
        }


        SerializedProperty FilteredRoot = serializedObject.FindProperty("FilteredRoot");
        SerializedProperty list = serializedObject.FindProperty("names");
        
        EditorGUILayout.PropertyField(list, new GUIContent("Names filter: "), true);
        EditorGUILayout.PropertyField(FilteredRoot, new GUIContent("Filtered Root: "), true);
        
        serializedObject.ApplyModifiedProperties();
    }
}