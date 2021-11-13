using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Rigidbody player;
    public float health;
    public float damage;
    public float attackCooldown;
    public float movementSpeed;
    public Transform position;

    private float cooldown = 0;

    void Start()
    {
        GameObject playerObj = GameObject.Find("Player");
        player = playerObj.GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (!player)
        {
            return;
        }

        Rigidbody enemyRb = GetComponent<Rigidbody>();

        Vector3 lookDir = player.position - enemyRb.position;
        float angle = -Mathf.Atan2(lookDir.z, lookDir.x) * Mathf.Rad2Deg;
        enemyRb.MoveRotation(Quaternion.Euler(new Vector3(0, angle, 0)));
        enemyRb.MovePosition(enemyRb.position + (lookDir.normalized * movementSpeed * Time.fixedDeltaTime));

        if (cooldown > 0)
        {
            cooldown -= Time.fixedDeltaTime;
        }

        print( seesPlayer() );

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
                PlayerMovement pm = collider.gameObject.GetComponent<PlayerMovement>();
                pm.TakeDamage(damage);
                cooldown += attackCooldown;
            }
        }
    }

    bool seesPlayer()
    {
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        // Ray ray = new Ray(rigidbody.position, player.position);
        float distance = Vector3.Distance(rigidbody.position, player.position);

        RaycastHit hit;
        if (Physics.Raycast(rigidbody.position, player.position, out hit, distance, 0))
        {
            return hit.collider.CompareTag("Player");
        }

        return false;
    }

    void getSoundLevel(){
        //distancia minima até o player 
        // max( volume do jogador - distancia ,0 );
    }

}
