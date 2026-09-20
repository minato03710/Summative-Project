using UnityEngine;

public class CompanionMovement : MonoBehaviour
{
    [Header("Movement")]
    public float rotationSpeed = 10f;


[Header("Gravity")]
    public float gravity = -20f;

    private CharacterController controller;

    private float verticalVelocity;

    private Vector3 currentMoveDirection;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        ApplyGravity();
    }

    public void Move(Vector3 direction, float speed)
    {
        if (controller == null)
            return;

        direction.y = 0f;

        if (direction.sqrMagnitude > 0.01f)
        {
            direction.Normalize();

            currentMoveDirection = direction;

            controller.Move(
                direction * speed * Time.deltaTime
            );

            LookAt(direction);
        }
        else
        {
            currentMoveDirection = Vector3.zero;
        }
    }

    public void Stop()
    {
        currentMoveDirection = Vector3.zero;

        if (controller == null)
            return;

        controller.Move(Vector3.zero);
    }

    void ApplyGravity()
    {
        if (controller == null)
            return;

        if (controller.isGrounded)
        {
            if (verticalVelocity < 0f)
            {
                verticalVelocity = -2f;
            }
        }
        else
        {
            verticalVelocity +=
                gravity * Time.deltaTime;
        }

        controller.Move(
            Vector3.up *
            verticalVelocity *
            Time.deltaTime
        );
    }

    void LookAt(Vector3 direction)
    {
        Quaternion targetRotation =
            Quaternion.LookRotation(direction);

        transform.rotation =
            Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
    }

    public void Jump(float jumpForce)
    {
        if (controller == null)
            return;

        if (!controller.isGrounded)
            return;

        verticalVelocity = jumpForce;
    }

    public bool IsGrounded()
    {
        if (controller == null)
            return false;

        return controller.isGrounded;
    }

    public Vector3 GetMoveDirection()
    {
        return currentMoveDirection;
    }


}
