using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MeshOperator))]
public class MeshOperatorEditor : Editor
{
    MeshOperator myTarget;
    public override void OnInspectorGUI()
    {
        myTarget = (MeshOperator)target;
        myTarget.vertices_visualize = EditorGUILayout.LongField("Vertices visualize: ", myTarget.vertices_visualize);
        myTarget.minimum_edge_length = EditorGUILayout.FloatField("Minimal edge length ", myTarget.minimum_edge_length);

        SerializedProperty targetMesh = serializedObject.FindProperty("target");

        EditorGUILayout.PropertyField(targetMesh, new GUIContent("Target: "), true);

        if (GUILayout.Button("Show Vertices"))
        {
            myTarget.ShowVertices();
        }
        if (GUILayout.Button("Upsample"))
        {
            myTarget.Upsample();
        }
        if (GUILayout.Button("Normals"))
        {
            myTarget.RecalculateNormals();
        }

        serializedObject.ApplyModifiedProperties();
    }
}