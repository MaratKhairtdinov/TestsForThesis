using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VertexCollector : MonoBehaviour
{
    public Transform parentNode;

    [SerializeField]
    List<Vector3> vertices = new List<Vector3>();
    List<Vector3> normals = new List<Vector3>();
    List<GameObject> pointcloud = new List<GameObject>();
    bool collected = true;

    bool drawn = false;
    public void Draw()
    {
        if (!drawn)
        {
            drawn = true;
            foreach (Vector3 vertex in vertices)
            {
                var dot = new GameObject();
                dot.AddComponent<LineRenderer>();
                var lr = dot.GetComponent<LineRenderer>();
                lr.material = new Material(Shader.Find("Sprites/Default"));
                lr.startColor = Color.red; lr.endColor = Color.red;
                lr.startWidth = 0.05f; lr.endWidth = 0.05f;

                dot.transform.parent = transform;
                dot.transform.position = vertex;

                lr.SetPosition(0, vertex);
                lr.SetPosition(1, vertex + Vector3.up * .05f);
                pointcloud.Add(dot);
            }
        }
        else
        {
            drawn = false;
            foreach(GameObject dot in pointcloud)
            {
                DestroyImmediate(dot);
            }
            pointcloud.Clear();
        }        
    }

    public void Collect()
    {
        vertices.Clear();
        normals.Clear();
        Traverce(parentNode);
        Debug.Log(vertices.Count);
    }

    public void Traverce(Transform root)
    {
        if (root.GetComponent<MeshFilter>() && root.gameObject.activeInHierarchy)
        {
            Matrix4x4 wrldMat = root.localToWorldMatrix;
            Mesh mesh = root.GetComponent<MeshFilter>().sharedMesh;            
            foreach(Vector3 vertex in mesh.vertices)
            {
                vertices.Add(root.TransformPoint(vertex));                
            }
            foreach(Vector3 normal in mesh.normals)
            {
                normals.Add(root.TransformDirection(normal).normalized);
            }
        }
        if (root.transform.childCount == 0) 
        { 
            return; 
        }
        foreach(Transform node in root)
        {
            Traverce(node);            
        }
    }

    public List<Vector3> GetVertices()
    {
        return vertices;
    }
    public List<Vector3> GetNormals()
    {
        return normals;
    }
}
