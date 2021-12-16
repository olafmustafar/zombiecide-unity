using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Rigidbody rb;
    public float movementSpeed;
    public float health;
    public Camera cam;
    // public float hint;
    [HideInInspector] public float soundLevel = 0;
    [HideInInspector] public Vector2 position;

    Vector2 movement;
    Plane plane;

    void Awake()
    {
        position = new Vector2(rb.position.x, rb.position.y);
    }

    void Start()
    {
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        plane = new Plane(Vector3.up, -rb.position.y);
    }

    void FixedUpdate()
    {
        position.Set(rb.position.x, rb.position.z);
        Move();
        Rotate();
        MakeSound();
    }

    private void MakeSound()
    {
        soundLevel = (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
            ? 50
            : 50;
    }

    void Rotate()
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

    void Move()
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

        rb.velocity = (forward + right).normalized * movementSpeed;
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
