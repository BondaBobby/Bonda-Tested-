using UnityEngine;

/// <summary>
/// Simple smooth 2-D / 3-D camera follow.
/// Attach to the camera (or camera pivot) object.
/// </summary>
public class CameraFollowIso : MonoBehaviour
{
    [Header("Target")]
    [Tooltip("Object the camera should follow.")]
    public Transform target;

    [Header("Offset")]
    [Tooltip("Fixed offset from the target (in world space).")]
    public Vector3 offset = new Vector3(0f, 10f, -10f);

    [Header("Smoothing")]
    [Tooltip("Higher = snappier, lower = smoother.")]
    [Range(0.1f, 20f)]
    public float smoothSpeed = 5f;

    [Header("Look At")]
    [Tooltip("Should the camera keep looking at the target?")]
    public bool lookAtTarget = false;

    private void LateUpdate()
    {
        if (target == null) return;

        // Desired position
        Vector3 desiredPosition = target.position + offset;

        // Smooth interpolation
        Vector3 smoothedPosition = Vector3.Lerp(
            transform.position,
            desiredPosition,
            smoothSpeed * Time.deltaTime
        );

        transform.position = smoothedPosition;

        // Optional look-at
        if (lookAtTarget)
            transform.LookAt(target);
    }
}