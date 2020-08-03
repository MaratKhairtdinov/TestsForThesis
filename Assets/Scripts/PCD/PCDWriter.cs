using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using System.Text;
using UnityEngine;

public class PCDWriter : MonoBehaviour
{
    public string outputFileLocation;
    public VertexCollector collector;
    string[] lines;
    public string header;
    public void ConvertFile()
    {
        header = "# .PCD v.7 - Point Cloud Data file format" + Environment.NewLine +
                "VERSION .7" + Environment.NewLine +
                "FIELDS x y z normal_x normal_y normal_z" + Environment.NewLine +
                "SIZE 4 4 4 4 4 4" + Environment.NewLine +
                "TYPE F F F F F F" + Environment.NewLine +
                "COUNT 1 1 1 1 1 1" + Environment.NewLine;

        List<Vector3> vertices = collector.GetVertices();
        List<Vector3> normals = collector.GetNormals();
        int points = vertices.Count;
        header += ("WIDTH "+ points + Environment.NewLine);
        header += ("HEIGHT 1" + Environment.NewLine);
        header += ("VIEWPOINT 0 0 0 1 0 0 0" + Environment.NewLine);
        header += ("POINTS " + points + Environment.NewLine);
        header += ("DATA ascii");
        
        for (int i = 0; i< vertices.Count; i++)
        {
            header += (Environment.NewLine + vertices[i].x.ToString() + " " + vertices[i].y.ToString() + " " + vertices[i].z.ToString() + 
                " " + normals[i].x.ToString() + " " + (normals[i].z).ToString() + " " + (-normals[i].y).ToString());
        }

        //lines = Regex.Split(header, Environment.NewLine);

        //File.WriteAllLines(outputFileLocation, lines);
    }

}
