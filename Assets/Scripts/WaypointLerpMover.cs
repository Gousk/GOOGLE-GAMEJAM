using UnityEngine;

[AddComponentMenu("Movement/Waypoint Lerp Mover")]
public class WaypointLerpMover : MonoBehaviour
{
    [Header("Waypoints (in world space)")]
    [Tooltip("At least two points required.")]
    public Vector3[] points;

    [Header("Timing Settings")]
    [Tooltip("Time (in seconds) to move between each pair of points.")]
    public float duration = 1f;

    [Header("Movement Mode")]
    [Tooltip("If true, the object will ping-pong back and forth. Otherwise it will loop.")]
    public bool pingPong = true;

    int currentIndex = 0;
    int direction = 1;  // +1 = forward through array, -1 = backward
    float elapsed = 0f;

    void Start()
    {
        if (points == null || points.Length < 2)
            Debug.LogWarning("WaypointLerpMover needs at least 2 points to move between.", this);

        // Initialize object at the first point
        if (points.Length > 0)
            transform.position = points[0];
    }

    void Update()
    {
        if (points == null || points.Length < 2) return;

        // Advance time
        elapsed += Time.deltaTime;
        float t = elapsed / duration;

        // Lerp between the current point and the next
        Vector3 a = points[currentIndex];
        Vector3 b = points[currentIndex + direction];
        transform.position = Vector3.Lerp(a, b, t);

        // When we've reached (or passed) the end of this segment...
        if (elapsed >= duration)
        {
            elapsed -= duration;              // carry over extra time
            currentIndex += direction;        // step to next index

            if (pingPong)
            {
                // Reverse direction at the ends
                if (currentIndex <= 0 || currentIndex >= points.Length - 1)
                    direction = -direction;
            }
            else
            {
                // Loop back to start when reaching the last segment
                if (currentIndex >= points.Length - 1)
                    currentIndex = 0;
            }
        }
    }
}
