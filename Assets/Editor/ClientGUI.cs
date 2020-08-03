using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TCPClient))]
public class ClientGUI : Editor
{
    TCPClient client;

    public override void OnInspectorGUI()
    {
        client = (TCPClient)target;
        client.host = EditorGUILayout.TextField("Host: ", client.host);
        client.port = EditorGUILayout.IntField("Port: ", client.port);

        client.message = EditorGUILayout.TextField("Message: ", client.message);
        client.msgType = EditorGUILayout.TextField("Message Type: ", client.msgType);

        EditorGUILayout.Space();
        SerializedProperty pcdWriter = serializedObject.FindProperty("pcdWriter");
        EditorGUILayout.PropertyField(pcdWriter, new GUIContent("PCDWriter"), true);
        serializedObject.ApplyModifiedProperties();

        if (GUILayout.Button("Connect to server"))
        {
            client.ConnectToServerAsync();
        }

        if (GUILayout.Button("Send message"))
        {
            client.SendMessageToNet(client.message, client.msgType);
        }

        if (GUILayout.Button("Send PCD"))
        {
            client.SendPointCloud();
        }

        if (GUILayout.Button("Disconnect"))
        {
            client.Disconnect();
        }
    }
    
}