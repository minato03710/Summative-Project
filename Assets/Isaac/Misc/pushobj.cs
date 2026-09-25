using UnityEngine;

public class PushObjects : MonoBehaviour
{
    [SerializeField] private float pushPower = 3.0f;

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody body = hit.collider.attachedRigidbody;

        // Ignore objects without a Rigidbody or objects set to Kinematic
        if (body == null || body.isKinematic)
            return;

        // Do not push objects when standing on top of them
        if (hit.moveDirection.y < -0.3f)
            return;

        // Calculate horizontal push direction
        Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);

        // Updated: Uses linearVelocity instead of velocity
        body.linearVelocity = pushDir * pushPower;
    }
}