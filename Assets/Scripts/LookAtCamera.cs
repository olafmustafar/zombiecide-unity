using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{

    Transform cameraTransform;

    void Start()
    {
        cameraTransform = FindObjectOfType<Camera>().GetComponent<Transform>();
    }

    void LateUpdate()
    {
        GetComponent<Transform>().rotation = Quaternion.LookRotation( - cameraTransform.forward, cameraTransform.up);
    }
}
