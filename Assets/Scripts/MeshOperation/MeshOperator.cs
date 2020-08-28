using System.Collections.Generic;
using Diagnostics=System.Diagnostics;
using UnityEngine;
public class MeshOperator : MonoBehaviour
{
    List<GameObject> targets = new List<GameObject>();
    List<Mesh> meshes;
    //Mesh mesh;
    GameObject dot;
    List<GameObject> dots = new List<GameObject>();

    public GameObject target;
    public long vertices_visualize = 5000000000;
    public float minimum_edge_length;

    int vertices_count = 0;


    void RecursiveChildren(Transform transform)
    {
        if (transform.GetComponent<MeshFilter>()) { targets.Add(transform.gameObject); }
        if (transform.childCount == 0) { return; }
        foreach (Transform child in transform)
        {
            RecursiveChildren(child);
        }
    }


    private void Init()
    {
        targets.Clear();
        RecursiveChildren(target.transform);

        int vertices_count = 0;
        foreach (GameObject target in targets)
        {
            if (target.GetComponent<MeshFilter>() && target.activeInHierarchy)
            {
                var mesh = target.GetComponent<MeshFilter>().sharedMesh;
                vertices_count += mesh.vertexCount;
                target.GetComponent<MeshFilter>().mesh = mesh;
            }
        }
        this.vertices_count = vertices_count;
    }

    public void Upsample()
    {
        Init();
        Upsampler.threshold = minimum_edge_length;
        foreach (GameObject target in targets)
        {
            if (target.GetComponent<MeshFilter>() && target.activeInHierarchy)
            {
                Upsampler.Subdivide(target.GetComponent<MeshFilter>().sharedMesh, 2);
                target.GetComponent<MeshFilter>().sharedMesh.RecalculateNormals();
            }
        }
        Init();
        Debug.Log(vertices_count + "\tVertices");
    }

    bool vertices_visible = false;
    public void ShowVertices()
    {
        Init();
        if (!vertices_visible && targets.Count > 0)
        {
            if (this.vertices_count < vertices_visualize)
            {
                foreach (GameObject target in targets)
                {
                    if (target.GetComponent<MeshFilter>() && target.activeInHierarchy)
                    {
                        Mesh mesh = target.GetComponent<MeshFilter>().sharedMesh;
                        foreach (Vector3 vertex in mesh.vertices)
                        {
                            var dot = new GameObject();
                            dot.AddComponent<LineRenderer>();
                            var lr = dot.GetComponent<LineRenderer>();
                            lr.material = new Material(Shader.Find("Sprites/Default"));
                            lr.startColor = Color.red; lr.endColor = Color.red;
                            lr.startWidth = 0.1f; lr.endWidth = 0.1f;
                            dot.transform.parent = target.transform;
                            dot.transform.localPosition = vertex;
                            lr.SetPosition(0, target.transform.TransformPoint(vertex));
                            lr.SetPosition(1, target.transform.TransformPoint(vertex) + Vector3.up * .1f);
                            dots.Add(dot);
                        }                        
                    }
                }                
            }
            else
            {
                Debug.Log(vertices_count + "\tVertices");
            }
            vertices_visible = true;
        }
        else
        {
            foreach (GameObject dot in dots)
            {
                DestroyImmediate(dot);
            }
            dots.Clear();
            targets.Clear();
            vertices_visible = false;
        }
    }


    bool normalsshown = false;
    List<GameObject> normals = new List<GameObject>();
    public void RecalculateNormals()
    {
        Init();
        
        if (!normalsshown && targets.Count > 0)
        {
            if (this.vertices_count < vertices_visualize)
            {
                foreach (GameObject target in targets)
                {
                    if (target.GetComponent<MeshFilter>() && target.activeInHierarchy)
                    {
                        Mesh mesh = target.GetComponent<MeshFilter>().sharedMesh;
                        mesh.RecalculateNormals();
                        for (int i = 0; i < mesh.vertexCount; i++)
                        {
                            var normal = new GameObject();
                            normal.AddComponent<LineRenderer>();
                            var lr = normal.GetComponent<LineRenderer>();
                            lr.material = new Material(Shader.Find("Sprites/Default"));
                            lr.startColor = Color.red; lr.endColor = Color.red;
                            lr.startWidth = 0.03f; lr.endWidth = 0.03f;
                            normal.transform.parent = target.transform;
                            normal.transform.localPosition = mesh.vertices[i];
                            lr.SetPosition(0, target.transform.TransformPoint(mesh.vertices[i]));
                            lr.SetPosition(1, target.transform.TransformPoint(mesh.vertices[i]) + target.transform.TransformDirection(mesh.normals[i]));
                            normals.Add(normal);
                        }
                    }
                }
            }
            else
            {
                Debug.Log(vertices_count + "\tVertices");
            }
            normalsshown = true;
        }
        else
        {
            foreach (GameObject normal in normals)
            {
                DestroyImmediate(normal);
            }
            normals.Clear();
            targets.Clear();
            normalsshown = false;
        }
    }
}