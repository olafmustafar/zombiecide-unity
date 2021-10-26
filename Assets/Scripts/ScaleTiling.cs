using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleTiling : MonoBehaviour
{
    Renderer rend;
    Transform t;

    void Start()
    {
        t = GetComponent<Transform>();
        rend = GetComponent<Renderer>();
    }

    void Update()
    {
        //TOD
        Vector3 scale = t.lossyScale;
        rend.material.mainTextureScale = new Vector2(15, 1);
    }
}
