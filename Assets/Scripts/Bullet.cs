using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameObject hitEffect;

    void OnTriggerEnter(Collider collider)
    {
        GameObject effect = Instantiate(hitEffect, gameObject.transform.position, Quaternion.identity);
        Destroy(effect, 5.0f);
        Destroy(gameObject);
    }
}
