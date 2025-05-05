using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(ConfigurableJoint), typeof(Rigidbody))]
public class PhysicsButton3D : MonoBehaviour
{
    [Header("Button Settings")]
    [Tooltip("Local Y offset threshold for physics-based press detection")]
    public float pressThreshold = 0.05f;

    [Tooltip("Event to fire when button is pressed")]
    public UnityEvent onPressed;

    [Header("Physical Press Force")]
    [Tooltip("Impulse force applied downward when pressed via CharacterController collision")]
    public float pressForce = 5f;

    private ConfigurableJoint joint;
    private Rigidbody rb;
    private bool wasPressed = false;

    void Start()
    {
        joint = GetComponent<ConfigurableJoint>();
        rb = GetComponent<Rigidbody>();
        var col = GetComponent<Collider>();
        col.isTrigger = false;
    }

    void Update()
    {
        CheckPhysicsPress();
    }

    private void CheckPhysicsPress()
    {
        // Determine how far the button has moved from its anchor
        float displacement = transform.localPosition.y - joint.anchor.y;
        bool isPressed = displacement < -pressThreshold;

        if (isPressed && !wasPressed)
        {
            wasPressed = true;
            onPressed.Invoke();
        }
        else if (!isPressed && wasPressed)
        {
            wasPressed = false;
        }
    }

    /// <summary>
    /// Call from CharacterController collision to both fire the event and apply a downward force.
    /// </summary>
    public void ForcePress()
    {
        if (!wasPressed)
        {
            wasPressed = true;
            //onPressed.Invoke();
            // apply physical force to push the button down
            rb.AddForce(Vector3.down * pressForce, ForceMode.Impulse);
        }
    }
}