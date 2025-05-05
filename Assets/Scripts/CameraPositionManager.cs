using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraPositionManager : MonoBehaviour
{
    [Header("Camera Root")]
    [Tooltip("Assign the camera (or its parent) whose rotation you want to manage.")]
    public Transform cameraRoot;

    [Header("Rotations")]
    [Tooltip("List of rotations (Euler angles) you can cycle through.")]
    public List<Vector3> rotationAngles = new List<Vector3>();

    [Header("Input")]
    [Tooltip("Key to press to switch to the next rotation.")]
    public KeyCode nextRotationKey = KeyCode.R;

    [Header("Transition")]
    [Tooltip("Duration in seconds for the smooth rotation transition.")]
    public float transitionDuration = 1f;

    [Header("Easing Curve")]
    [Tooltip("Animation curve to ease the rotation interpolation (t from 0 to 1).")]
    public AnimationCurve transitionCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

    private int currentIndex = 0;
    private bool isTransitioning = false;

    private void Start()
    {
        if (cameraRoot == null)
        {
            Debug.LogWarning("CameraPositionManager: Camera Root not assigned—defaulting to this GameObject.");
            cameraRoot = transform;
        }

        if (rotationAngles.Count > 0)
        {
            cameraRoot.rotation = Quaternion.Euler(rotationAngles[currentIndex]);
        }
        else
        {
            Debug.LogWarning("CameraPositionManager: No rotations configured in the list.");
        }
    }

    private void Update()
    {
        if (!isTransitioning && Input.GetKeyDown(nextRotationKey))
        {
            NextRotation();
        }
    }

    /// <summary>
    /// Move to the next rotation in the list (wraps around to 0 after the last).
    /// </summary>
    public void NextRotation()
    {
        if (rotationAngles.Count == 0)
            return;

        // Advance index and wrap to 0 when reaching the end
        currentIndex = (currentIndex + 1) % rotationAngles.Count;

        Quaternion from = cameraRoot.rotation;
        Quaternion to = Quaternion.Euler(rotationAngles[currentIndex]);

        StopAllCoroutines();
        StartCoroutine(SmoothRotate(from, to));
    }

    /// <summary>
    /// Smoothly interpolates the cameraRoot rotation over transitionDuration using the transitionCurve.
    /// </summary>
    private IEnumerator SmoothRotate(Quaternion from, Quaternion to)
    {
        isTransitioning = true;
        float elapsed = 0f;

        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / transitionDuration);
            // Evaluate curve for easing
            float curveT = transitionCurve.Evaluate(t);
            cameraRoot.rotation = Quaternion.Slerp(from, to, curveT);
            yield return null;
        }

        cameraRoot.rotation = to;
        isTransitioning = false;
    }
}
