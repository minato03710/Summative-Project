using UnityEngine;

public class CompanionMovement : MonoBehaviour
{
    [Header("Movement")]
    public float rotationSpeed = 10f;

    [Header("Gravity")]
    public float gravity = -20f;

    private CharacterController controller;
    private CompanionCommandController commands;

    private float verticalVelocity;
    public Vector3 currentMoveDirection;

    // Animation Manager Bools

    public bool companionPlayJump;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        commands = GetComponent<CompanionCommandController>();
    }

    private void Update()
    {
        if (Time.timeScale <= 0f)
            return;

        // ����ʱ�Ա������������������ָ������ա�
        ApplyGravity();
    }

    public void Move(Vector3 direction, float speed)
    {
        if (Time.timeScale <= 0f ||
            (commands != null && commands.IsSitting))
        {
            currentMoveDirection = Vector3.zero;
            return;
        }

        if (controller == null)
            return;

        speed *= commands != null
            ? commands.SpeedMultiplier
            : 1f;

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

    private void ApplyGravity()
    {
        if (controller == null)
            return;

        if (controller.isGrounded)
        {
            companionPlayJump = false;
            if (verticalVelocity < 0f)
            {
                verticalVelocity = -2f;
            }
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        controller.Move(
            Vector3.up * verticalVelocity * Time.deltaTime
        );
    }

    private void LookAt(Vector3 direction)
    {
        Quaternion targetRotation =
            Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }

    public void Jump(float jumpForce)
    {
        if (Time.timeScale <= 0f ||
            (commands != null && commands.IsSitting))
        {
            return;
        }

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