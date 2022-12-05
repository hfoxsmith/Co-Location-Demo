using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class JerusalemSceneLoader : MonoBehaviour
{
    public void loadNext()
    {
        // List<Vector3> diffList = new List<Vector3>();
        // List<Vector3> meshVertices = GameObject.FindGameObjectWithTag("Wall Creation").GetComponent<WallDrawer>().GetVertices();
        // Vector3 XRRigPosition = GameObject.FindGameObjectWithTag("Special Tag").transform.position;

        // Debug.Log(meshVertices.ToString());
        // foreach (Vector3 vertex in meshVertices)
        // {
        //     diffList.Add(vertex - XRRigPosition);
        // }
        SceneManager.LoadScene("Jerusalem Temple");
        // GameObject wall = GameObject.FindGameObjectWithTag("Wall");
        // GameObject XRRig = GameObject.FindGameObjectWithTag("Special Tag");
        // Debug.Log($"XRRig position: {XRRig.transform.position.ToString()}");
        // wall.transform.position = XRRig.transform.position;
        // Debug.Log($"New wall position: {wall.transform.position.ToString()}");
    }
}
