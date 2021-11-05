using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameObject hitEffect;
    //TODO
    void OnCollisionEnter2D(Collision2D collision){
        GameObject obj = Instantiate( hitEffect, transform.position, Quaternion.identity );
        Destroy( obj, 5f );
        Destroy( gameObject );
    }
}
