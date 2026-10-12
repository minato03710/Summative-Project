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

    public bool playPlayerDash;

    public void DashPressed(InputAction.CallbackContext context)
    {
        GetComponent<PlayerInputReader>().ReadDash(context);
    }

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        PlayerInputReader input = PlayerInputReader.Instance;

        if (input == null ||
            !input.GameplayAllowed ||
            !controller.enabled)
        {
            StopDash();
            return;
        }

        if (cooldownTimer > 0)
            cooldownTimer -= Time.deltaTime;

        if (!isDashing)
        {
            playPlayerDash = false;

            if (input.DashPressed)
                TryDash();
        }

        if (isDashing)
            Dash();
    }

    void TryDash()
    {
        if (cooldownTimer > 0)
            return;

        Vector2 input = PlayerInputReader.Instance.Move;

        if (input.sqrMagnitude < 0.01f)
            return;

        input = Vector2.ClampMagnitude(input, 1f);

        dashDirection =
            new Vector3(input.x, 0, input.y).normalized;

        isDashing = true;
        playPlayerDash = true;

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
            StopDash();
    }

    public bool IsDashing()
    {
        return isDashing;
    }

    private void StopDash()
    {
        isDashing = false;
        playPlayerDash = false;
        dashTimer = 0f;
    }

    private void OnDisable()
    {
        StopDash();
    }
}