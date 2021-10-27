using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody rb;
    public float movementSpeed;
    public Camera cam;

    Vector3 movement;

    void FixedUpdate()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = 0;
        movement.z = Input.GetAxisRaw("Vertical");

        Vector3 forward = cam.transform.forward;
        Vector3 right = cam.transform.right;
        
        forward.y = 0;
        forward.Normalize();
        
        right.y = 0;
        right.Normalize();

        forward *=  movement.z;
        right *= movement.x;
        
        print ( right );

        movement = forward + right;

        rb.MovePosition(rb.position + movement * movementSpeed * Time.fixedDeltaTime);
    }
}
