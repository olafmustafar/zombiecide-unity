using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    public Transform firePoint;
    public GameObject bulletPrefab;

    public float shotCooldown = 0.20f;
    public float reducedShotCooldown = 0.10f;
    public float bulletForce = 20f;

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
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position + (firePoint.right * 0.2f), firePoint.rotation * Quaternion.Euler(0, 90, 0) );
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            rb.AddForce(firePoint.forward * bulletForce, ForceMode.Impulse);

            GameObject bullet2 = Instantiate(bulletPrefab, firePoint.position + (firePoint.right * -0.2f), firePoint.rotation * Quaternion.Euler(0, 90, 0));
            Rigidbody rb2 = bullet2.GetComponent<Rigidbody>();
            rb2.AddForce(firePoint.forward * bulletForce, ForceMode.Impulse);
        }
        else
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation * Quaternion.Euler(0, 90, 0));
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            rb.AddForce(firePoint.forward * bulletForce, ForceMode.Impulse);
        }


        if (powerUp == PowerUpType.SPREAD_BULLETS)
        {
            GameObject bullet2 = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation * Quaternion.Euler(0, 90 - 40, 0));
            Rigidbody rb2 = bullet2.GetComponent<Rigidbody>();
            rb2.AddForce(Quaternion.Euler(0, -40, 0) * firePoint.forward * bulletForce, ForceMode.Impulse);

            GameObject bullet3 = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation * Quaternion.Euler(0, 90 + 40, 0));
            Rigidbody rb3 = bullet3.GetComponent<Rigidbody>();
            rb3.AddForce(Quaternion.Euler(0, 40, 0) * firePoint.forward * bulletForce, ForceMode.Impulse);
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "PowerUp")
        {
            powerUp = (PowerUpType)Random.Range(1, (int)PowerUpType.SIZE);
        }
    }
}
