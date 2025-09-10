using UnityEngine;

public class PivotFollow : MonoBehaviour
{
    // Drag the Player cube here in the Inspector
    public Transform target;

    // Offset from the player's pivot point
    public Vector3 offset = new Vector3(0f, 0f, 0f);

    // How quickly the pivot catches up
    public float smoothSpeed = 5f;

    private void LateUpdate()
    {
        if (target == null) return;

        // Desired position = player + offset
        Vector3 desiredPos = target.position + offset;

        // Smooth move
        Vector3 smoothedPos = Vector3.Lerp(transform.position, desiredPos, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPos;
    }
}