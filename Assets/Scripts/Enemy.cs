using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, Agent
{
    public float health;
    public float damage;
    public float attackCooldown;

    public GameObject target;

    //Agent interface
    [HideInInspector] public Vector2 targetPosition { get; set; }
    [HideInInspector] public Vector2 currentPosition { get; set; }
    [HideInInspector] public float pBest { get; set; }
    [HideInInspector] public Vector2 pBestPosition { get; set; }
    [HideInInspector] public Vector2 inertia { get; set; }

    Player player;
    Rigidbody rb;
    float lastSqrMag;
    GameObject targetInstance;

    private float cooldown = 0;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        currentPosition = new Vector2(rb.position.x, rb.position.z);
        targetPosition = new Vector2(rb.position.x, rb.position.z);
        lastSqrMag = Mathf.Infinity;
    }

    void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
    }

    void FixedUpdate()
    {

        currentPosition = new Vector2(rb.position.x, rb.position.z);

        if ( !targetInstance || !targetInstance.activeInHierarchy)
        {
            print("create");
            targetInstance = Instantiate( target,new Vector3( targetPosition.x, rb.position.y , targetPosition.y ) , Quaternion.identity);
        }

        Vector2 targetVector = (targetPosition - currentPosition);

        if (targetVector == Vector2.zero || targetVector.magnitude <= 0.1)
        {
            rb.position = new Vector3(targetPosition.x, rb.position.y, targetPosition.y);
            rb.velocity = Vector3.zero;
            print("destroyed");
            if( targetInstance ){
                Destroy(targetInstance);
            }
        }
        else
        {
            Vector3 directionalVector = targetVector.normalized * inertia.magnitude;
            rb.velocity = new Vector3(directionalVector.x, 0, directionalVector.y);

            print( rb.velocity + " | " + inertia );
        }
        
        Rotate();
        updateCooldown();

    }

    public float getFitness()
    {
        float distance = Vector2.Distance(player.position, currentPosition);
        return Mathf.Max(player.soundLevel - distance, 0);
    }

    void Rotate(){
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
            Destroy(collider.gameObject);
            health -= 10;
            if (health <= 0)
            {
                Destroy(gameObject);
            }

        }
    }

    void OnTriggerStay(Collider collider)
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
