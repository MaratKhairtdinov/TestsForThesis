using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Xml;
using Dummiesman;
using System;

public class IfcLoader : MonoBehaviour {

    public string fileName, filePath;
    public GameObject loadedOBJ;
    private XmlDocument loadedXML;
    string fileToParse;
    public VertexCollector vertexCollector;


    public GameObject root;
    public GameObject FilteredRoot;
    public MeshOperator meshOperator;


    //[SerializeField]
    //public Dictionary<string, List<GameObject>> gameObjects = new Dictionary<string, List<GameObject>>();
    [SerializeField]
    public string[] names;

    public List<GameObject> hiddenObjects = new List<GameObject>(); 

    private void OnValidate()
    {
        //if (hiddenObjects==null) { hiddenObjects }
        meshOperator = GetComponent<MeshOperator>();
    }


    public void ConvertIFC()
    {
        fileToParse = Application.dataPath;
        fileToParse += ("/" + filePath);
        fileToParse += ("/" + fileName);

        string exeToRun = Application.dataPath + "/Scripts/IFC/IfcConverter/IfcConvert.exe";
        
        if (!File.Exists(fileToParse + ".obj"))
        {
            System.Diagnostics.Process readIFC = new System.Diagnostics.Process
            {
                StartInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = exeToRun,
                    Arguments = fileToParse + ".ifc " + fileToParse +".obj --use-element-guids",
                    UseShellExecute = true,
                    RedirectStandardOutput = false,
                    CreateNoWindow = false
                }
            };
            System.Diagnostics.Process readXML = new System.Diagnostics.Process
            {
                StartInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = exeToRun,
                    Arguments = fileToParse + ".ifc " + fileToParse + ".xml --use-element-guids",
                    UseShellExecute = true,
                    RedirectStandardOutput = false,
                    CreateNoWindow = false
                }
            };
            readIFC.Start();
            readIFC.WaitForExit();
            
            readXML.Start();
            readXML.WaitForExit();
            /*
            System.Diagnostics.Process ifcRead = System.Diagnostics.Process.Start(exeToRun, " " + fileToParse + ".ifc " + fileToParse + ".obj --use-element-guids");
            ifcRead.WaitForExit();
            */
        }        
        LoadObj();
        LoadXML();
        //Filter();
    }

    public void LoadObj()
    {
        fileToParse = Application.dataPath;
        fileToParse += ("/" + filePath);
        fileToParse += ("/" + fileName+".obj");
        loadedOBJ = new OBJLoader().Load(fileToParse);
        if (loadedOBJ != null)
        {
            loadedOBJ.transform.Rotate(-90, 0, 0);
        }
    }


    public void LoadXML()
    {
        fileToParse = Application.dataPath;
        fileToParse += ("/" + filePath);
        fileToParse += ("/" + fileName + ".xml");

        loadedXML = new XmlDocument();
        loadedXML.Load(fileToParse);
        string basePath = @"//ifc/decomposition";
        root = new GameObject();
        root.name = fileName+" (IFC)";

        vertexCollector.parentNode = root.transform;
        meshOperator.target = root;
        vertexCollector.Collect();

        foreach (XmlNode node in loadedXML.SelectNodes(basePath + "/IfcProject"))
        {
            AddElements(node, root);
        }

        FindStorey(root.transform);
    }


    public void FindStorey(Transform node)
    {
        if (node.GetComponent<IFCData>() && node.GetComponent<IFCData>().IFCClass.ToLower().Contains("storey"))
        {
            FilteredRoot = node.gameObject;            
            return;
        }
        foreach (Transform child in node)
        {
            FindStorey(child);
        }
    }

    private void AddElements(XmlNode node, GameObject parent)
    {
        if (node.Attributes.GetNamedItem("id") != null)
        {
            Debug.Log(string.Format("{0} => {1}", node.Attributes.GetNamedItem("id").Value, node.Attributes.GetNamedItem("Name").Value));
            // Search an existing GameObject with this name
            // This would apply only to elements which have
            // a geometric representation and which are
            // extracted from the 3D file.
            string searchPath = fileName + "/" +
                node.Attributes.GetNamedItem("id").Value;
            GameObject goElement = null;
            goElement = GameObject.Find(searchPath);

            // What if we can't find any? We need to create
            // a new empty object
            if (goElement == null)
                goElement = new GameObject();

            if (goElement != null)
            {
                // Set name from the IFC Name field
                goElement.name = node.Attributes.GetNamedItem("Name").Value;

                //SaveObjectLinks(goElement.name, goElement);

                // Link the object to the parent we received
                if (parent != null)
                    goElement.transform.SetParent(parent.transform);
                if (node.ParentNode!=null)
                {
                    AddProperties(node, goElement);
                }
                // Go through children (recursively)
                foreach (XmlNode child in node.ChildNodes)
                    AddElements(child, goElement);
            }
        } // end if "id" attribute
    }

    private void AddProperties(XmlNode node, GameObject go)
    {
        IFCData ifcData = go.AddComponent(typeof(IFCData)) as IFCData;

        ifcData.IFCClass = node.Name;
        ifcData.STEPId = node.Attributes.GetNamedItem("id").Value;
        ifcData.STEPName = node.Attributes.GetNamedItem("Name").Value;
        // Initialize PropertySets and QuantitySets
        if (ifcData.propertySets == null)
            ifcData.propertySets = new List<IFCPropertySet>();
        if (ifcData.quantitySets == null)
            ifcData.quantitySets = new List<IFCPropertySet>();


        // Go through Properties (and Quantities and ...)
        foreach (XmlNode child in node.ChildNodes)
        {
            switch (child.Name)
            {
                case "IfcPropertySet":
                case "IfcElementQuantity":
                    // we only receive a link beware of # character
                    string link = child.Attributes.GetNamedItem("xlink:href").Value.TrimStart('#');
                    string path = String.Format("//ifc/properties/IfcPropertySet[@id='{0}']", link);
                    if (child.Name == "IfcElementQuantity")
                        path = String.Format("//ifc/quantities/IfcElementQuantity[@id='{0}']", link);
                    XmlNode propertySet = loadedXML.SelectSingleNode(path);
                    if (propertySet != null)
                    {
                        Debug.Log(
                            string.Format("PropertySet = {0}",
                                          propertySet.Attributes.GetNamedItem("Name").Value));

                        // initialize this propertyset (Name, Id)
                        IFCPropertySet myPropertySet = new IFCPropertySet();
                        myPropertySet.propSetName = propertySet.Attributes.GetNamedItem("Name").Value;
                        myPropertySet.propSetId = propertySet.Attributes.GetNamedItem("id").Value;
                        if (myPropertySet.properties == null)
                            myPropertySet.properties = new List<IFCProperty>();

                        // run through property values
                        foreach (XmlNode property in propertySet.ChildNodes)
                        {
                            string propName, propValue = "";
                            IFCProperty myProp = new IFCProperty();
                            propName = property.Attributes.GetNamedItem("Name").Value;

                            if (property.Name == "IfcPropertySingleValue")
                                propValue = property.Attributes.GetNamedItem("NominalValue").Value;
                            if (property.Name == "IfcQuantityLength")
                                propValue = property.Attributes.GetNamedItem("LengthValue").Value;
                            if (property.Name == "IfcQuantityArea")
                                propValue = property.Attributes.GetNamedItem("AreaValue").Value;
                            if (property.Name == "IfcQuantityVolume")
                                propValue = property.Attributes.GetNamedItem("VolumeValue").Value;
                            // Write property (name & value)
                            myProp.propName = propName;
                            myProp.propValue = propValue;
                            myPropertySet.properties.Add(myProp);
                        }

                        // add propertyset to IFCData
                        if (child.Name == "IfcPropertySet")
                            ifcData.propertySets.Add(myPropertySet);
                        if (child.Name == "IfcElementQuantity")
                            ifcData.quantitySets.Add(myPropertySet);

                    } // end if PropertySet
                    break;
                default:
                    // all the rest...
                    break;
            } // end switch
        } // end foreach
          // initialize this propertyset (Name, Id)

        //IFCPropertySet myPropertySet = new IFCPropertySet();
    }

    bool filtered = false;
    public void Filter()
    {
        if (!filtered)
        {
            hiddenObjects = new List<GameObject>();
            filtered = true;
            FilterNodes(FilteredRoot.transform);
        }
        else
        {
            filtered = false;
            foreach(GameObject obj in hiddenObjects)
            {
                obj.SetActive(true);
            }
            hiddenObjects.Clear();
        }
    }

    public void FilterNodes(Transform node)
    {
        bool match = false;
        foreach(string name in names)
        {
            string IFCClass = node.GetComponent<IFCData>().IFCClass.ToLower();
            if (IFCClass.Contains(name.ToLower())) 
            { 
                match = true; 
                break; 
            }
        }

        if (!match && node.gameObject!=FilteredRoot) 
        {
            hiddenObjects.Add(node.gameObject);
            node.gameObject.SetActive(false);
            //DestroyImmediate(node.gameObject);
        }

        if (node.childCount == 0) 
        { 
            return; 
        }
        foreach(Transform child in node)
        {
            if (child != node)
            {
                FilterNodes(child);
            }
        }
    }

}

