using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BreakableObject : MonoBehaviour
{
    [Header("Break Settings")]
    [Tooltip("Tag of the projectile that triggers breaking")]
    public string projectileTag = "Projectile";

    [Tooltip("Child rigidbodies to unfreeze when broken. If empty, will auto-collect all child Rigidbodies.")]
    public Rigidbody[] pieces;

    [Tooltip("Explosion force applied to pieces when breaking")]
    public float breakForce = 500f;

    [Tooltip("Radius of the explosion force")]
    public float explosionRadius = 2f;

    [Tooltip("Upward modifier for explosion force")]
    public float upwardModifier = 0.5f;

    private bool hasBroken = false;

    void Awake()
    {
        Collider col = GetComponent<Collider>();
        col.isTrigger = false;

        if (pieces == null || pieces.Length == 0)
        {
            List<Rigidbody> list = new List<Rigidbody>();
            foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
            {
                if (rb.gameObject != this.gameObject)
                    list.Add(rb);
            }
            pieces = list.ToArray();
        }

        foreach (var rb in pieces)
        {
            rb.isKinematic = true;
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (hasBroken) return;
        if (!collision.collider.CompareTag(projectileTag)) return;
        Break();
    }

    private void Break()
    {
        GetComponent<Collider>().enabled = false;
        hasBroken = true;

        foreach (var rb in pieces)
        {
            rb.isKinematic = false;
            rb.constraints = RigidbodyConstraints.None;
            rb.transform.SetParent(null, true);
            // use explosion force for more natural scatter
            rb.AddExplosionForce(breakForce, transform.position, explosionRadius, upwardModifier, ForceMode.Impulse);
        }

        // Optionally disable visuals or self
        // GetComponent<Renderer>()?.enabled = false;
        // Destroy(gameObject);
    }
}
