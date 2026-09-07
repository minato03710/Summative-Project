using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDash : MonoBehaviour
{
    [Header("Dash Settings")]
    public float dashSpeed = 15f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;

    private CharacterController controller;

    private Vector3 dashDirection;
    private float dashTimer;
    private float cooldownTimer;

    private bool isDashing;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // CD
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }

        // Start Dash
        if (!isDashing)
        {
            if (Keyboard.current != null &&
                Keyboard.current.leftShiftKey.wasPressedThisFrame)
            {
                TryDash();
            }
        }

        // Dash move
        if (isDashing)
        {
            Dash();
        }
    }

    void TryDash()
    {
        //  Cooldown Dash
        if (cooldownTimer > 0)
            return;

        Vector2 input = Vector2.zero;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed)
                input.y += 1;

            if (Keyboard.current.sKey.isPressed)
                input.y -= 1;

            if (Keyboard.current.aKey.isPressed)
                input.x -= 1;

            if (Keyboard.current.dKey.isPressed)
                input.x += 1;
        }

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


