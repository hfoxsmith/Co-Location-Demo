using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

public class Origin : MonoBehaviour
{
    public Transform originLocation;
    public Transform xrCamera;
    public Transform xrOrigin;
    public Transform cameraOffset;
    public Transform anchor;

    public void SetOrigin() 
    {
        Debug.Log("Origin Set!");
        originLocation.position = new Vector3(xrCamera.position.x, 0, xrCamera.position.z);
    }

    public void SetAnchor() 
    {
        Debug.Log("Anchor Set!");
        anchor.position = new Vector3(xrCamera.position.x, 0, xrCamera.position.z);
    }

    public void Teleport(InputAction.CallbackContext context) 
    {
        if(context.started) {
            //TrackedPoseDriver poseDriver = xrCamera.GetComponent<TrackedPoseDriver>();
            // poseDriver.enabled = false;
            Debug.Log("Teleporting to Origin!");
            xrOrigin.position = originLocation.position;
            //xrCamera.position = originLocation.position;
            xrOrigin.LookAt(anchor);

            //sets xrorigin rotation to 0, y, 0
           // cameraOffset.localPosition = Vector3.zero;
            //cameraOffset.LookAt(anchor);
            //xrCamera.localPosition = new Vector3(0, xrCamera.position.y, 0);
            //xrCamera.localRotation = new Quaternion(0, 0, 0, 0);

            //poseDriver.enabled = true;
        }
    }
}
