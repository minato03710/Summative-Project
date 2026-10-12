using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;
    Vector2 input;
    public Vector2 GetMovementInput => input;

    [Header("Jump")]
    public float jumpHeight = 2f;

    [Header("Gravity")]
    public float gravity = -20f;

    [Header("VFX")]
    public VisualEffect FootstepsVFX;

    private CharacterController controller;
    private Vector3 velocity;
    private PlayerDash playerDash;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        playerDash = GetComponent<PlayerDash>();

        SimpleVFX.Prepare(FootstepsVFX);
    }

    void Update()
    {
        if (Time.timeScale <= 0f)
            return;

        //KeyboardInputs();
        Move();
        ApplyGravity();
        Jump();
    }

    public void GetInput(InputAction.CallbackContext ctx)
    {
        input = ctx.ReadValue<Vector2>();
    }

    void KeyboardInputs()
    {
        Vector2 keyBoardinput = Vector2.zero;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed)
                keyBoardinput.y += 1;

            if (Keyboard.current.sKey.isPressed)
                keyBoardinput.y -= 1;

            if (Keyboard.current.aKey.isPressed)
                keyBoardinput.x -= 1;

            if (Keyboard.current.dKey.isPressed)
                keyBoardinput.x += 1;
        }

        //GetInput(keyBoardinput);
    }

    void Move()
    {
        if (playerDash != null && playerDash.IsDashing())
        {
            SimpleVFX.Loop(FootstepsVFX, false);
            return;
        }

        input = Vector2.ClampMagnitude(input, 1f);

        Vector3 moveDirection = new Vector3(
            input.x,
            0f,
            input.y
        );

        Vector3 beforeMove = transform.position;

        controller.Move(
            moveDirection * moveSpeed * Time.deltaTime
        );

        Vector3 displacement = transform.position - beforeMove;
        displacement.y = 0f;

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

        controller.Move(
            velocity * Time.deltaTime
        );
    }

    void Jump()
    {
        if (Keyboard.current == null)
            return;

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            Debug.Log(
                "Space Pressed | Grounded = " +
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
        SimpleVFX.Prepare(FootstepsVFX);
    }
}