using UnityEngine;

public class Player : MonoBehaviour
{
    public Rigidbody rb;
    public float maxVelocity;
    public float health;
    public Camera cam;
    // public float hint;
    [HideInInspector] public float soundLevel = 0;
    [HideInInspector] public Vector2 position;

    float velocity;
    Vector2 movement;
    Plane plane;

    void Awake()
    {
        velocity = maxVelocity;
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
        velocity = Mathf.Min(velocity + (maxVelocity * Time.fixedDeltaTime), maxVelocity);
        Move();
        Rotate();
        MakeSound();
    }

    private void MakeSound()
    {
        soundLevel = (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
            ? 50
            : 20;
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

        rb.velocity = (forward + right).normalized * velocity;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        velocity = 0;
        if (health <= 0)
        {
            Destroy(gameObject);
            print("Player is dead");
        }
    }

}
