using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float health;
    public float damage;
    public float attackCooldown;
    public float velocity;

    [HideInInspector] public Vector2 targetPosition;
    [HideInInspector] public Vector2 currentPosition;
    [HideInInspector] public float pBest = -1;
    [HideInInspector] public Vector2 pBestPosition;

    Rigidbody rb;
    Player player;

    private float cooldown = 0;

    public float getSoundLevel()
    {
        float distance = Vector2.Distance(player.position, currentPosition);
        return Mathf.Max(player.soundLevel - distance, 0);
    }

    void Awake(){
        Rigidbody rb = GetComponent<Rigidbody>();
        targetPosition.Set( rb.position.x, rb.position.z );
        currentPosition.Set( rb.position.x, rb.position.z );
    }
    
    void Start()
    {
        GameObject playerObj = GameObject.Find("Player");
        player = playerObj.GetComponent<Player>();
    }

    void FixedUpdate()
    {
        if (!player)
        {
            return;
        }

        Vector3 lookDir = new Vector3(targetPosition.x, 0, targetPosition.y) - rb.position;
        float angle = -Mathf.Atan2(lookDir.z, lookDir.x) * Mathf.Rad2Deg;
        rb.MoveRotation(Quaternion.Euler(new Vector3(0, angle, 0)));
        rb.MovePosition(rb.position + (lookDir.normalized * velocity * Time.fixedDeltaTime));

        if (cooldown > 0)
        {
            cooldown -= Time.fixedDeltaTime;
        }

        currentPosition.Set(-rb.position.x, -rb.position.z);
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
