using UnityEngine.AI;
using UnityEngine;

public class Enemy : MonoBehaviour, Agent
{
    public float health;
    public float damage;
    public float attackCooldown;
    public float velocity;
    public GameObject deathEffect;
    public GameObject powerUp;

    //Agent interface
    [HideInInspector] public Vector2 targetPosition { get; set; }
    [HideInInspector] public Vector2 currentPosition { get; set; }
    [HideInInspector] public float pBest { get; set; }
    [HideInInspector] public Vector2 pBestPosition { get; set; }
    [HideInInspector] public Vector2 inertia { get; set; }

    Player player;
    Rigidbody rb;
    NavMeshPath path;
    Camera camera;

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
        camera = GameObject.FindObjectOfType<Camera>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    void FixedUpdate()
    {
        currentPosition = VectorConverter.Convert(rb.position);
        Move();
        Rotate();
        updateCooldown();
        RotateSprite();
    }

    public float getFitness()
    {
        if (!player.isActiveAndEnabled)
        {
            return 0.0f;
        }

        float distance = Vector2.Distance(player.position, currentPosition);

        float visionScore = 0;
        {
            RaycastHit hit;
            if (Physics.Raycast(rb.position, player.rb.position - rb.position, out hit) && hit.collider.CompareTag("Player"))
            {
                visionScore = Mathf.Max(50 - distance, 0);
            }
        }

        float soundScore = 0;

        {
            Vector3 target = player.rb.position - rb.position;
            RaycastHit[] raycastHits = Physics.RaycastAll(rb.position, target.normalized, target.magnitude);

            int nonPlayerObjects = 0;
            foreach (RaycastHit hit in raycastHits)
            {
                if (!hit.collider.CompareTag("Player"))
                {
                    nonPlayerObjects++;
                }
            }

            soundScore = Mathf.Max(player.soundLevel - distance - (nonPlayerObjects * 10), 0);
        }

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

    void RotateSprite()
    {
        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
        Vector3 rbAngle = rb.rotation.eulerAngles;
        float rbAbsoluteAngle = (rbAngle.y + 360) % 360;

        Vector3 camAngle = camera.transform.rotation.eulerAngles;
        float camAbsoluteAngle = (camAngle.y + 360) % 360;

        sr.flipX = ((rbAbsoluteAngle + camAbsoluteAngle) % 360) <= 180;
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
                HandleDeath();
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

    void HandleDeath()
    {
        GameObject effect = Instantiate(deathEffect, gameObject.transform.position, Quaternion.identity);

        if( powerUp && Random.Range(0f,1f) < 0.20){
            DropPowerUp();
        }

        GameObject.Find("GameManager").GetComponent<GameManager>().IncreaseScore(10);
        FindObjectOfType<AudioManager>().Play("EnemyDeath");

        Destroy(effect, 5.0f);
        Destroy(gameObject);
    }

    void DropPowerUp(){
        if ( powerUp ){
            GameObject effect = Instantiate(powerUp, gameObject.transform.position, Quaternion.identity);
        }
    }
}
