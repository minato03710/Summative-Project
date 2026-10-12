using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDash : MonoBehaviour
{
    [Header("Dash Settings")]
    public float dashSpeed = 15f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;

    private CharacterController controller;
    PlayerMovement playerMovement;

    private Vector3 dashDirection;
    private float dashTimer;
    private float cooldownTimer;

    private bool isDashing;

    // Animation Bool

    public bool playPlayerDash;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        // CD
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }

        // Start Dash
        //if (!isDashing)
        //{
        //    playPlayerDash = false;
        //    if (Keyboard.current != null &&
        //        Keyboard.current.leftShiftKey.wasPressedThisFrame)
        //    {
        //        TryDash();
        //        playPlayerDash = true;
        //    }
        //}

        // Dash move
        if (isDashing)
        {
            Dash();
        }
        else
        {
            playPlayerDash = false;
        }
    }

    public void DashPressed(InputAction.CallbackContext context)
    {
        if(isDashing == true)
        {
            return;
        }

        if(context.performed == true)
        {
            TryDash();
            playPlayerDash = true;
        }
    }

    void TryDash()
    {
        //  Cooldown Dash
        if (cooldownTimer > 0)
            return;

        Vector2 input = playerMovement.GetMovementInput;

        // No movement direction, cannot dash
        if (input.sqrMagnitude < 0.01f)
            return;

        input = Vector2.ClampMagnitude(input, 1f);

        dashDirection = new Vector3(
            input.x,
            0,
            input.y
        ).normalized;

        isDashing = true;
        dashTimer = dashDuration;
        cooldownTimer = dashCooldown;
    }

    void Dash()
    {
        controller.Move(
            dashDirection * dashSpeed * Time.deltaTime
        );

        dashTimer -= Time.deltaTime;

        if (dashTimer <= 0)
        {
            isDashing = false;
        }
    }

    public bool IsDashing()
    {
        return isDashing;
    }
}


