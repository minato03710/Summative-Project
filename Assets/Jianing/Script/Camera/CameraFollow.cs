using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Camera Settings")]
    public float distance = 8f;
    public float angle = 50f;
    public float smoothSpeed = 10f;

    [Header("Camera Boundary")]
    public float minX = -20f;
    public float maxX = 20f;
    public float minZ = -20f;
    public float maxZ = 20f;

    void LateUpdate()
    {
        if (target == null)
            return;

        // Camera offset
        Vector3 offset = Quaternion.Euler(angle, 0, 0)
                         * new Vector3(0, 0, -distance);

        Vector3 targetPosition = target.position + offset;

        // Limit camera range
        targetPosition.x = Mathf.Clamp(
            targetPosition.x,
            minX,
            maxX
        );

        targetPosition.z = Mathf.Clamp(
            targetPosition.z,
            minZ,
            maxZ
        );

        // Smooth follow
        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            smoothSpeed * Time.deltaTime
        );

        // Look at the player
        transform.LookAt(target);
    }
}




