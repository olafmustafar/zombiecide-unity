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
        currentPosition = VectorConverter.Convert(rb.position);
        targetPosition = VectorConverter.Convert(rb.position);
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

        float soundScore = Mathf.Max(player.soundLevel - distance, 0);

        RaycastHit hit;
        float visionScore = 0;
        if ( Physics.Raycast(rb.position, player.rb.position - rb.position, out hit ) && hit.collider.CompareTag("Player") ) {
            visionScore = Mathf.Max(50 - distance , 0);
        }

        print( $"{GetInstanceID()}: visionScore {visionScore} + soundScore {soundScore}" );

        return visionScore + soundScore;
    }

    void Move()
    {
        Vector2 targetVector;
        targetVector = (targetPosition - currentPosition);
        rb.velocity = VectorConverter.Convert(targetVector.normalized * Mathf.Min(velocity, targetVector.magnitude));
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
