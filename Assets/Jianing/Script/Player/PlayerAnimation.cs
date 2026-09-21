using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAnimation : MonoBehaviour
{
    [Header("Animator")]
    public Animator animator;


[Header("Movement")]
    public float movementThreshold = 0.01f;

    private CharacterController controller;


    void Start()
    {
        controller =
            GetComponent<CharacterController>();

        // 如果没有手动拖入 Animator
        // 自动寻找子物体中的 Animator
        if (animator == null)
        {
            animator =
                GetComponentInChildren<Animator>();
        }
    }


    void Update()
    {
        if (animator == null)
            return;

        UpdateMovementAnimation();
        UpdateJumpAnimation();
    }


    // =========================================================
    // Walking / Idle
    // =========================================================

    void UpdateMovementAnimation()
    {
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

        bool isMoving =
            input.sqrMagnitude >
            movementThreshold;

        animator.SetBool(
            "IsMoving",
            isMoving
        );
    }


    // =========================================================
    // Jump
    // =========================================================

    void UpdateJumpAnimation()
    {
        if (controller == null)
            return;

        bool isJumping =
            !controller.isGrounded;

        animator.SetBool(
            "IsJumping",
            isJumping
        );
    }


}
