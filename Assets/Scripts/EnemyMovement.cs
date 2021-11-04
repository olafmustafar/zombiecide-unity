using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    //Propriedades do inimigo

    public GameObject player;
    public float health;
    public float damage;
    public float attackSpeed;
    public float movementSpeed;
    // public float activeTime;
    // public float restTime;
    // public float projectileSpeed;
    // public int movementType;
    // public int weaponTipe;


    void Start()
    {
        player = GameObject.Find("Player");
    }

    void FixedUpdate()
    {
        Rigidbody playerRb = player.GetComponent<Rigidbody>();
        Rigidbody enemyRb = GetComponent<Rigidbody>();

        Vector3 lookDir = playerRb.position - enemyRb.position;
        float angle = -Mathf.Atan2(lookDir.z, lookDir.x) * Mathf.Rad2Deg;
        enemyRb.MoveRotation( Quaternion.Euler(new Vector3(0, angle, 0)));

        enemyRb.MovePosition( enemyRb.position + (lookDir.normalized * movementSpeed * Time.fixedDeltaTime) );


    }
}
