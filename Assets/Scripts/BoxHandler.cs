using UnityEngine;

public class BoxHandler : MonoBehaviour
{
    [Header("Respawn Settings")]
    [Tooltip("Transform to respawn this object to.")]
    public Transform respawnPoint;

    [Header("Line Settings")]
    [Tooltip("Transform the line will connect to.")]
    public Transform lineStart;
    public Transform lineTarget;

    private Rigidbody rb;
    private LineRenderer lineRenderer;

    public string colliderTag;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        lineRenderer = GetComponent<LineRenderer>();

        // Ensure the line has two points
        lineRenderer.positionCount = 2;
    }

    void Update()
    {
        // Update the line each frame so it always connects
        if (lineTarget != null)
        {
            lineRenderer.SetPosition(0, lineStart.position);
            lineRenderer.SetPosition(1, lineTarget.position);
        }
        else
        {
            lineRenderer.enabled = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("OutOfBounds"))
        {
            // Teleport back to respawnPoint (position + rotation)
            if (respawnPoint != null)
            {
                transform.position = respawnPoint.position;
                transform.rotation = respawnPoint.rotation;
            }

            // Zero out physics so it stays put
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(colliderTag))
        {
            // Teleport back to respawnPoint (position + rotation)
            if (respawnPoint != null)
            {
                transform.position = respawnPoint.position;
                transform.rotation = respawnPoint.rotation;
            }

            // Zero out physics so it stays put
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}
