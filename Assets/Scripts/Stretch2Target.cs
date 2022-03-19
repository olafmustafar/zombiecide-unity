using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stretch2Target : MonoBehaviour
{
    public Vector3 target;

    void Start()
    {
        Transform transform = GetComponent<Transform>();

        Vector3 origin = transform.position;
        float distance = Vector3.Distance(origin, target);

        transform.LookAt(target);

        Vector3 newScale = transform.localScale;
        newScale.z = distance;
        transform.localScale = newScale;
    }
}
