using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, Agent
{
    public float health;
    public float damage;
    public float attackCooldown;
    public float velocity;

    public GameObject target;

    //Agent interface
    [HideInInspector] public Vector2 targetPosition { get; set; }
    [HideInInspector] public Vector2 currentPosition { get; set; }
    [HideInInspector] public float pBest { get; set; }
    [HideInInspector] public Vector2 pBestPosition { get; set; }
    [HideInInspector] public Vector2 inertia { get; set; }

    Player player;
    Rigidbody rb;
    GameObject targetInstance;

    private float cooldown = 0;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        currentPosition = new Vector2(rb.position.x, rb.position.z);
        targetPosition = new Vector2(rb.position.x, rb.position.z);
        pBest = -1;
    }

    void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
    }

    void FixedUpdate()
    {
        currentPosition = VectorConverter.Convert(rb.position);
        MarkTargePosition();
        Move();
        Rotate();
        updateCooldown();
    }

    public float getFitness()
    {
        float distance = Vector2.Distance(player.position, currentPosition);
        return Mathf.Max(player.soundLevel - distance, 0);
    }

    void MarkTargePosition()
    {
        if (!targetInstance || !targetInstance.activeInHierarchy)
        {
            targetInstance = Instantiate(target, VectorConverter.Convert(targetPosition, rb.position.y), Quaternion.identity);
        }
        else if (currentPosition == targetPosition)
        {
            Destroy(targetInstance);
        }
    }

    void Move()
    {
        Vector2 targetVector = (targetPosition - currentPosition);
        Vector2 newPos = (targetVector.normalized * Mathf.Min(inertia.magnitude, velocity) * Time.fixedDeltaTime);


        if (Mathf.Approximately(newPos.sqrMagnitude, targetVector.sqrMagnitude) || newPos.sqrMagnitude > targetVector.sqrMagnitude)
        {
            print("2");
            rb.MovePosition(VectorConverter.Convert(currentPosition + targetVector, rb.position.y));
        }
        else
        {
            rb.MovePosition(VectorConverter.Convert(currentPosition + newPos, rb.position.y));
            Vector2 nnPos = currentPosition + newPos;
            newPos = currentPosition - targetPosition;
            print($"1 {newPos.sqrMagnitude} < {targetVector.sqrMagnitude}\n np:{newPos.x} - {newPos.y} \n tp:{targetPosition.x} - {targetPosition.y}\n cp:{currentPosition.x} - {currentPosition.y} \nnnps:{nnPos.x} - {nnPos.y}");
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
