using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR;

public class WallDrawer : MonoBehaviour
{
    GameObject wall;
    Mesh wallMesh;
    List<Vector3> vertices;
    List<int> triangles;

    bool primaryButtonPressed;
    bool drawing = false;
    bool finished = false;

    public float snapDistance = 0.1f;
    public float wallHeight = 2;

    [SerializeField]
    private XRNode xrNode = XRNode.RightHand;

    private List<InputDevice> devices = new List<InputDevice>();

    private InputDevice device;

    void GetDevice()
    {
        InputDevices.GetDevicesAtXRNode(xrNode, devices);
        device = devices.FirstOrDefault();
    }

    void OnEnable()
    {
        if (!device.isValid)
        {
            GetDevice();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        vertices = new List<Vector3>();
        wallMesh = new Mesh { name = "Procedural Mesh" };
        wall = new GameObject { name = "Wall" };
        wall.AddComponent<MeshFilter>();
        wall.AddComponent<MeshRenderer>();
        wall.GetComponent<MeshFilter>().mesh = wallMesh;
        wall.GetComponent<MeshRenderer>().material = new Material(Shader.Find("Sprites/Default"));
        wall.GetComponent<MeshRenderer>().sharedMaterial.SetColor("_Color", new Color(0f, 1f, 1f, 0.55f));

        Vector3 newPosition;
        GameObject XRRig = GameObject.FindGameObjectWithTag("Special Tag");
        Debug.Log($"XR Rig Position: {XRRig.transform.position.ToString()}");
        InputDevices.GetDeviceAtXRNode(XRNode.CenterEye).TryGetFeatureValue(CommonUsages.centerEyePosition, out newPosition);
        Debug.Log($"CenterEye location {newPosition.ToString()}");
        wall.transform.position = XRRig.transform.position;

        wall.tag = "Wall";

        // DontDestroyOnLoad(this.wall);
        // DontDestroyOnLoad(gameObject);
        triangles = new List<int>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!device.isValid)
        {
            GetDevice();
        }

        // GameObject XRRig = GameObject.FindGameObjectWithTag("Special Tag");
        // Debug.Log($"Updated XRRig position: {XRRig.transform.position.ToString()}");
        // Vector3 newPosition;
        // InputDevices.GetDeviceAtXRNode(XRNode.CenterEye).TryGetFeatureValue(CommonUsages.centerEyePosition, out newPosition);
        // Debug.Log($"CenterEye location {newPosition.ToString()}");

        // Clear lines if B button is pressed
        bool secondaryButtonValue;
        device.TryGetFeatureValue(CommonUsages.secondaryButton, out secondaryButtonValue);
        if (secondaryButtonValue)
        {
            triangles.Clear();
            vertices.Clear();
            wallMesh.SetVertices(vertices.ToArray());
            wallMesh.triangles = triangles.ToArray();
            drawing = false;
            finished = false;
        }

        if (finished)
        {
            return;
        }

        InputFeatureUsage<Vector3> devicePosition = CommonUsages.devicePosition;
        InputFeatureUsage<bool> primaryButton = CommonUsages.primaryButton;

        bool primaryButtonValue;
        device.TryGetFeatureValue(primaryButton, out primaryButtonValue);

        if (!primaryButtonPressed && primaryButtonValue && !drawing) // Start drawing for the first time
        {
            Debug.Log("Starting line");
            primaryButtonPressed = true;
            drawing = true;
            Vector3 top;
            Vector3 bottom;
            device.TryGetFeatureValue(devicePosition, out top);
            bottom = top;

            top.y = wallHeight;
            bottom.y = 0;
            vertices.Add(bottom);
            vertices.Add(top);
            vertices.Add(bottom);
            vertices.Add(top);

            // GameObject corner = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            

            triangles.Add(0);
            triangles.Add(1);
            triangles.Add(2);
            triangles.Add(2);
            triangles.Add(1);
            triangles.Add(3);

            wallMesh.vertices = vertices.ToArray();
            wallMesh.triangles = triangles.ToArray();
           
            wallMesh.RecalculateBounds();
            wallMesh.RecalculateNormals();
        }

        if (!primaryButtonPressed && primaryButtonValue && drawing) // Adding additional point
        {
            Debug.Log("adding additional point");
            primaryButtonPressed = true;
            Vector3 pointToAdd;
            device.TryGetFeatureValue(devicePosition, out pointToAdd);

            Vector3 firstPoint = vertices.ElementAt(0);
            bool lastPoint = Mathf.Abs(pointToAdd.x - firstPoint.x) <= snapDistance && Mathf.Abs(pointToAdd.z - firstPoint.z) <= snapDistance;
            if (lastPoint)
            {
                triangles.Add(vertices.Count - 3);
                triangles.Add(vertices.Count - 2);
                triangles.Add(0);
                triangles.Add(0);
                triangles.Add(vertices.Count - 2);
                triangles.Add(1);

                wallMesh.triangles = triangles.ToArray();

                wallMesh.RecalculateBounds();
                wallMesh.RecalculateNormals();

                drawing = false;
                finished = true;
                
                return;
            }

            pointToAdd.y = 0;
            vertices.Add(pointToAdd);
            triangles.Add(vertices.Count - 3);
            triangles.Add(vertices.Count - 2);
            triangles.Add(vertices.Count - 1);

            pointToAdd.y = wallHeight;
            vertices.Add(pointToAdd);
            triangles.Add(vertices.Count - 2);
            triangles.Add(vertices.Count - 3);
            triangles.Add(vertices.Count - 1);

            wallMesh.SetVertices(vertices.ToArray());
            wallMesh.triangles = triangles.ToArray();

            wallMesh.RecalculateBounds();
            wallMesh.RecalculateNormals();
        }

        if (drawing) // Update line to show current device position
        {
            Vector3 firstPoint = vertices.ElementAt(0);
            Vector3 endPoint;
            device.TryGetFeatureValue(devicePosition, out endPoint);
            bool lastPoint = Mathf.Abs(endPoint.x - firstPoint.x) <= snapDistance && Mathf.Abs(endPoint.z - firstPoint.z) <= snapDistance;
            if (lastPoint)
            {
                endPoint = firstPoint;
            }

            vertices.RemoveAt(vertices.Count - 1);
            vertices.RemoveAt(vertices.Count - 1);
            endPoint.y = 0;
            vertices.Add(endPoint);
            endPoint.y = wallHeight;
            vertices.Add(endPoint);

            wallMesh.SetVertices(vertices.ToArray());
            wallMesh.RecalculateBounds();
            wallMesh.RecalculateNormals();
        }

        if (primaryButtonPressed && !primaryButtonValue) // Primary button stops being pressed
        {
            primaryButtonPressed = false;
        }
    }

    public List<Vector3> GetVertices() 
    {
        return vertices;
    }

    public List<int> GetTriangles() 
    {
        return triangles;
    }
}