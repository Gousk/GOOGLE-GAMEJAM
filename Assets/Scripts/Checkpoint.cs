using UnityEngine;

/// <summary>
/// Attach to checkpoint objects (with Collider set to Trigger).
/// When the player enters, it registers with the RespawnManager.
/// </summary>
[RequireComponent(typeof(Collider))]
public class Checkpoint : MonoBehaviour
{
    public Transform spawnPos;
    [Tooltip("Message displayed when this checkpoint is reached.")]
    public string message = "Checkpoint Reached";
    void Reset()
    {
        // Ensure collider is set as trigger
        var col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Register this checkpoint
            RespawnManager.Instance?.SetCheckpoint(spawnPos); 
            RespawnManager.Instance.ShowCheckpoint(message);
            GetComponent<BoxCollider>().enabled = false;
            this.enabled = false;
        }
    }
}