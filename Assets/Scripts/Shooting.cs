using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    public Transform firePoint;
    public GameObject bulletPrefab;

    public float shotCooldown = 0.20f;
    public float reducedShotCooldown = 0.10f;
    public float bulletForce = 1000f;

    private float cooldown = 0.0f;

    enum PowerUpType
    {
        NONE,
        DOUBLE_BULLETS,
        SPREAD_BULLETS,
        FASTER_COOLDOWN,
        SIZE,
    }
    private PowerUpType powerUp = PowerUpType.NONE;

    void Update()
    {
        if (cooldown > 0)
        {
            cooldown -= Time.deltaTime;
        }

        if (Input.GetButton("Fire1") && cooldown <= 0)
        {
            Shoot();
            cooldown += powerUp == PowerUpType.FASTER_COOLDOWN ? reducedShotCooldown : shotCooldown;
        }
    }

    void Shoot()
    {
        if (powerUp == PowerUpType.DOUBLE_BULLETS)
        {
            InstantiateBullet(0, firePoint.right * 0.2f);
            InstantiateBullet(0, firePoint.right * -0.2f);
        }
        else
        {
            InstantiateBullet(0, Vector3.zero);
        }

        if (powerUp == PowerUpType.SPREAD_BULLETS)
        {
            InstantiateBullet(40, Vector3.zero);
            InstantiateBullet(-40, Vector3.zero);
        }
    }

    void InstantiateBullet(int angle, Vector3 positionShift)
    {

        GameObject bullet2 = Instantiate(bulletPrefab, firePoint.position + positionShift, firePoint.rotation * Quaternion.Euler(0, 90 + angle, 0));
        Rigidbody rb2 = bullet2.GetComponent<Rigidbody>();
        rb2.AddForce(Quaternion.Euler(0, angle, 0) * firePoint.forward * bulletForce, ForceMode.Impulse);
        Destroy(bullet2, 0.2f);
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "PowerUp")
        {
            powerUp = (PowerUpType)Random.Range(1, (int)PowerUpType.SIZE);
        }
    }
}
