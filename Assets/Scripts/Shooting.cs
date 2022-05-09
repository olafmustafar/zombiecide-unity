using UnityEngine;

public class Shooting : MonoBehaviour
{
    public Transform firePoint;
    public GameObject bulletPrefab;

    public float shotCooldown = 0.5f;
    public float bulletForce = 20f;

    private float cooldown = 0.0f;

    void Update()
    {
        if (cooldown > 0)
        {
            cooldown -= Time.fixedDeltaTime;
        }
 
        if (Input.GetButtonDown("Fire1") && cooldown <= 0)
        {
            Shoot();
            cooldown += shotCooldown;
        }
    }

    void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation * Quaternion.Euler(0, 90, 0));
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.AddForce(firePoint.forward * bulletForce, ForceMode.Impulse);
    }
}
