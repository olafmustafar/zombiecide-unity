using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    //Propriedades do inimigo

    public GameObject player;
    public float health;
    public float damage;
    public float attackCooldown;
    public float movementSpeed;

    private float cooldown = 0;

    void Start()
    {
        player = GameObject.Find("Player");
    }

    void FixedUpdate()
    {
        if (!player)
        {
            return;
        }

        Rigidbody playerRb = player.GetComponent<Rigidbody>();
        Rigidbody enemyRb = GetComponent<Rigidbody>();

        Vector3 lookDir = playerRb.position - enemyRb.position;
        float angle = -Mathf.Atan2(lookDir.z, lookDir.x) * Mathf.Rad2Deg;
        enemyRb.MoveRotation(Quaternion.Euler(new Vector3(0, angle, 0)));
        enemyRb.MovePosition(enemyRb.position + (lookDir.normalized * movementSpeed * Time.fixedDeltaTime));

        if( cooldown > 0 ){
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
            if( cooldown <= 0 ){
                PlayerMovement pm = collider.gameObject.GetComponent<PlayerMovement>();
                pm.TakeDamage(damage);
                cooldown += attackCooldown;
            }
        }
    }

}
