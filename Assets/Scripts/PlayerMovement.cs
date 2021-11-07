using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody rb;
    public float movementSpeed;
    public float health;
    public Camera cam;

    Vector2 movement;
    Plane plane;

    void Start()
    {
        plane = new Plane(Vector3.up, -rb.position.y);
    }

    void FixedUpdate()
    {
        MovePlayer();
        RotatePlayer();
    }

    void RotatePlayer()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        float distanceToPlane;

        if (plane.Raycast(ray, out distanceToPlane))
        {
            Vector3 lookDir = ray.GetPoint(distanceToPlane);
            lookDir -= rb.position;

            float angle = -Mathf.Atan2(lookDir.z, lookDir.x) * Mathf.Rad2Deg;
            rb.MoveRotation(Quaternion.Euler(new Vector3(0, angle, 0)));
        }
    }

    void MovePlayer()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        Vector3 forward = cam.transform.forward;
        Vector3 right = cam.transform.right;

        forward.y = 0;
        forward.Normalize();

        right.y = 0;
        right.Normalize();

        forward *= movement.y;
        right *= movement.x;

        rb.MovePosition(rb.position + (forward + right).normalized * movementSpeed * Time.fixedDeltaTime);
    }

    public void TakeDamage(float damage)
    {
        print("damage taken");
        health -= damage;
        if (health <= 0)
        {
            Destroy(gameObject);
            print("Player is dead");
        }
    }

}
