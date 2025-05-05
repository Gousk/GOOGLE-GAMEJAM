using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Bomb : MonoBehaviour
{
    private MapSettings m_Settings;
    [Header("Explosion Settings")]
    public ParticleSystem explosionEffect;
    public float explosionRadius = 5f;
    public float explosionForce = 700f;

    [Header("References")]
    public string projectileTag = "Projectile";

    private bool hasExploded = false;

    void Start()
    {
        // ensure collider is a trigger
        var col = GetComponent<Collider>();
        col.isTrigger = true;
        m_Settings = FindAnyObjectByType<MapSettings>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasExploded) return;
        if (!other.CompareTag(projectileTag)) return;

        Explode();
    }

    private void Explode()
    {
        hasExploded = true;

        // spawn and play effect
        if (explosionEffect != null)
        {
            ParticleSystem effect = Instantiate(explosionEffect, transform.position, Quaternion.identity);
            effect.Play();
            //Destroy(effect.gameObject, effect.main.duration + effect.main.startLifetime.constantMax);
        }

        m_Settings.destroyedBombCount++;

        // optional: apply physics force to nearby Rigidbodies
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (var hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }
        }

        // destroy bomb object
        Destroy(gameObject);
    }
}
