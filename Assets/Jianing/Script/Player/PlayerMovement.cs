using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;

    [Header("Jump")]
    public float jumpHeight = 2f;

    [Header("Gravity")]
    public float gravity = -20f;

    [Header("VFX")]
    public VisualEffect FootstepsVFX;

    private CharacterController controller;
    private Vector3 velocity;
    private PlayerDash playerDash;

    public bool IsMoving { get; private set; }

    public Vector2 GetMovementInput =>
        PlayerInputReader.Instance != null
            ? PlayerInputReader.Instance.Move
            : Vector2.zero;

    public void GetInput(InputAction.CallbackContext context)
    {
        GetComponent<PlayerInputReader>().ReadMove(context);
    }

    public void JumpPressed(InputAction.CallbackContext context)
    {
        GetComponent<PlayerInputReader>().ReadJump(context);
    }

    void Start()
    {
        controller = GetComponent<CharacterController>();
        playerDash = GetComponent<PlayerDash>();

        SimpleVFX.Prepare(FootstepsVFX);
    }

    void Update()
    {
        PlayerInputReader input = PlayerInputReader.Instance;

        if (input == null ||
            !input.GameplayAllowed ||
            !controller.enabled)
        {
            IsMoving = false;
            SimpleVFX.Loop(FootstepsVFX, false);
            return;
        }

        Move();
        ApplyGravity();
        Jump();
    }

    void Move()
    {
        if (playerDash != null && playerDash.IsDashing())
        {
            IsMoving = false;
            SimpleVFX.Loop(FootstepsVFX, false);
            return;
        }

        Vector2 input = PlayerInputReader.Instance.Move;

        Vector3 moveDirection =
            new Vector3(input.x, 0f, input.y);

        Vector3 beforeMove = transform.position;

        controller.Move(
            moveDirection * moveSpeed * Time.deltaTime
        );

        Vector3 displacement = transform.position - beforeMove;
        displacement.y = 0f;

        IsMoving = displacement.sqrMagnitude > 0.000001f;

        SimpleVFX.Loop(
            FootstepsVFX,
            controller.isGrounded &&
            displacement.sqrMagnitude > 0.000001f
        );

        if (moveDirection.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation =
                Quaternion.LookRotation(moveDirection);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }

    void ApplyGravity()
    {
        if (controller.isGrounded && velocity.y < 0f)
            velocity.y = -2f;

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }

    void Jump()
    {
        if (PlayerInputReader.Instance.JumpPressed)
        {
            Debug.Log(
                "Jump Pressed | Grounded = " +
                controller.isGrounded
            );

            if (controller.isGrounded)
            {
                SimpleVFX.Loop(FootstepsVFX, false);

                velocity.y = Mathf.Sqrt(
                    jumpHeight * -2f * gravity
                );

                Debug.Log("JUMP!");
            }
        }
    }

    private void OnDisable()
    {
        IsMoving = false;
        SimpleVFX.Prepare(FootstepsVFX);

        if (PlayerInputReader.Instance != null)
            PlayerInputReader.Instance.SuppressGameplay();
    }
}