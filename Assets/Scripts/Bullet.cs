using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameObject hitEffect;

    void OnCollisionEnter(Collision collision){
        // GameObject obj = Instantiate( hitEffect, transform.position, Quaternion.identity );
        // Destroy( obj, 5f );
        Destroy( gameObject );
    }
}
