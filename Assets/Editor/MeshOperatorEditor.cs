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
    }
}