using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour, Agent
{
    public float health;
    public float damage;
    public float attackCooldown;
    public float velocity;

    //Agent interface
    [HideInInspector] public Vector2 targetPosition { get; set; }
    [HideInInspector] public Vector2 currentPosition { get; set; }
    [HideInInspector] public float pBest { get; set; }
    [HideInInspector] public Vector2 pBestPosition { get; set; }
    [HideInInspector] public Vector2 inertia { get; set; }

    Player player;
    Rigidbody rb;
    GameObject targetInstance;
    NavMeshPath path;

    private float cooldown = 0;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        currentPosition = new Vector2(rb.position.x, rb.position.z);
        targetPosition = new Vector2(rb.position.x, rb.position.z);
        pBest = -1;
        path = new NavMeshPath();
    }

    void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
    }

    void FixedUpdate()
    {
        currentPosition = VectorConverter.Convert(rb.position);
        Move();
        Rotate();
        updateCooldown();
    }

    public float getFitness()
    {
        float distance = Vector2.Distance(player.position, currentPosition);
        return Mathf.Max(player.soundLevel - distance, 0);
    }

    void Move()
    {
        NavMesh.CalculatePath(VectorConverter.Convert(currentPosition), VectorConverter.Convert(targetPosition), NavMesh.AllAreas, path);
        // NavMesh.CalculatePath(currentPosition, targetPosition, NavMesh.AllAreas, path);

        // print(VectorConverter.Convert(targetPosition));      

        Vector2 targetVector;
        if (path.corners.Length > 0)
        {
            print( $"path.corners.Length > {path.corners.Length}" );
            targetVector = (VectorConverter.Convert(path.corners[1]) - currentPosition);
            Debug.DrawLine(VectorConverter.Convert(currentPosition, 2), VectorConverter.Convert((targetVector + currentPosition), 2), Color.blue );
        }
        else
        {
            targetVector = (targetPosition - currentPosition);
            Debug.DrawLine(VectorConverter.Convert(currentPosition, 2), VectorConverter.Convert((targetVector + currentPosition), 2), Color.red );
        }


        if (path.corners.Length <= 2 && targetVector.sqrMagnitude < (velocity * velocity))
        {
            rb.velocity = VectorConverter.Convert(targetVector);
        }
        else
        {
            rb.velocity = VectorConverter.Convert(targetVector.normalized * velocity);
        }
    }

    void Rotate()
    {
        Vector2 lookDir = targetPosition - currentPosition;
        float angle = -Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;
        rb.MoveRotation(Quaternion.Euler(new Vector3(0, angle, 0)));
    }

    void updateCooldown()
    {
        if (cooldown > 0)
        {
            cooldown -= Time.fixedDeltaTime;
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Bullet")
        {
            health -= 10;
            if (health <= 0)
            {
                Destroy(gameObject);
            }

        }

    }

    void OnCollisionStay(Collision collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            if (cooldown <= 0)
            {
                Player pm = collider.gameObject.GetComponent<Player>();
                pm.TakeDamage(damage);
                cooldown += attackCooldown;
            }
        }
    }
}
