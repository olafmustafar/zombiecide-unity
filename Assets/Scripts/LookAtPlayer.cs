using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    public Transform playerTransform;
    [Range(0.01f, 1.0f)]
    public float smoothFactor = 0.5f;

    Vector3 cameraOffset;

    void Start()
    {
        cameraOffset = transform.position - playerTransform.position;
    }

    void LateUpdate()
    {
        Vector3 newPos = playerTransform.position + cameraOffset;
        transform.position = Vector3.Slerp(transform.position, newPos, smoothFactor);
    }
}
