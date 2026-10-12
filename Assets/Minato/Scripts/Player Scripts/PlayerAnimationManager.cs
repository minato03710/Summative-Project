using UnityEngine;

public class PlayerAnimationManager : MonoBehaviour
{
    public Animator playerAnimator;
    public PlayerMovement playerMovement;
    public PlayerDash playerDash;
    public PlayerAttack playerAttack;

    private CharacterController characterController;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();

        if (playerAnimator == null)
            playerAnimator = GetComponentInChildren<Animator>();

        if (playerMovement == null)
            playerMovement = GetComponent<PlayerMovement>();

        if (playerDash == null)
            playerDash = GetComponent<PlayerDash>();

        if (playerAttack == null)
            playerAttack = GetComponent<PlayerAttack>();
    }

    private void LateUpdate()
    {
        if (playerAnimator == null)
            return;

        playerAnimator.SetBool(
            "meleeIsMoving",
            playerMovement != null &&
            playerMovement.enabled &&
            playerMovement.IsMoving
        );

        playerAnimator.SetBool(
            "meleeOnGround",
            characterController != null &&
            characterController.isGrounded
        );

        playerAnimator.SetBool(
            "meleeIsDashing",
            playerDash != null &&
            playerDash.IsDashing()
        );

        playerAnimator.SetBool(
            "meleeIsAttacking",
            playerAttack != null &&
            playerAttack.playerPlayAttack
        );
    }
}