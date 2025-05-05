using UnityEngine;

public class ShootingController : MonoBehaviour
{
    [Header("Projectile Settings")]
    [Tooltip("Prefab of the projectile to spawn")]
    public GameObject projectilePrefab;
    [Tooltip("Spawn point (e.g. tip of your staff)")]
    public Transform firePoint;
    [Tooltip("Speed at which the projectile is launched")]
    public float projectileSpeed = 20f;
    [Tooltip("How many shots per second")]
    public float fireRate = 5f;
    [Tooltip("Time in seconds before a spawned projectile is destroyed")]
    public float projectileLifeTime = 5f;

    public bool canShoot;
    public GameObject staffObj;
    
    private bool isShooting;
    private float shootTimer;

    /// <summary>
    /// Call this when entering shooting mode.
    /// </summary>
    public void StartShooting()
    {
        isShooting = true;
        shootTimer = 0f;
    }

    /// <summary>
    /// Call this when exiting shooting mode.
    /// </summary>
    public void StopShooting()
    {
        isShooting = false;
    }

    void Update()
    {
        if (canShoot && !staffObj.activeSelf)
        {
            staffObj.SetActive(true);
            FindAnyObjectByType<OrthographicCharacterController>().ghostItems.staff.SetActive(true);
        }

        if (!isShooting || projectilePrefab == null || firePoint == null || canShoot == false)
            return;

        shootTimer += Time.deltaTime;
        float interval = 1f / fireRate;
        if (shootTimer >= interval)
        {
            shootTimer -= interval;
            FireProjectile();
        }
    }

    private void FireProjectile()
    {
        // Instantiate and launch
        GameObject proj = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = proj.GetComponent<Rigidbody>();
        if (rb != null)
            rb.linearVelocity = firePoint.forward * projectileSpeed;

        // Auto-destroy after lifetime
        Destroy(proj, projectileLifeTime);
    }
}
