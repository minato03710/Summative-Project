using UnityEngine;
using System.Collections;

public class PlayerAnimationManager : MonoBehaviour
{

    public Animator playerAnimator;
    private bool playerWalking;
    private CharacterController characterController;
    public PlayerMovement playerMovement;
    public PlayerDash playerDash;
    public PlayerAttack playerAttack;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        playerAnimator = GetComponentInChildren<Animator>(); // References animator from player visuals
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            playerAnimator.SetBool("meleeIsMoving", true);
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            playerAnimator.SetBool("meleeIsMoving", true);
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            playerAnimator.SetBool("meleeIsMoving", true);
        }

        if(Input.GetKeyDown(KeyCode.D))
        {
            playerAnimator.SetBool("meleeIsMoving", true);
        }

        if (Input.GetKeyUp(KeyCode.W))
        {
            playerAnimator.SetBool("meleeIsMoving", false);
        }

        if (Input.GetKeyUp(KeyCode.A))
        {
            playerAnimator.SetBool("meleeIsMoving", false);
        }

        if (Input.GetKeyUp(KeyCode.S))
        {
            playerAnimator.SetBool("meleeIsMoving", false);
        }

        if (Input.GetKeyUp(KeyCode.D))
        {
            playerAnimator.SetBool("meleeIsMoving", false);
        }

        PlayerJump();

        PlayerDashCheck();

        PlayerAttackCheck();
    }

    void PlayerJump()
    {
        if (characterController.isGrounded)
        {
            playerAnimator.SetBool("meleeOnGround", true);
        }

        else
        {
            playerAnimator.SetBool("meleeOnGround", false);
        }
    }

    void PlayerDashCheck()
    {
        if (playerDash.playPlayerDash == true)
        {
            playerAnimator.SetBool("meleeIsDashing", true);
        }

        if (playerDash.playPlayerDash == false)
        {
            playerAnimator.SetBool("meleeIsDashing", false);
        }
    }

    void PlayerAttackCheck()
    {
        if (playerAttack.playerPlayAttack == true)
        {
            playerAnimator.SetBool("meleeIsAttacking", true);
        }

        if (playerAttack.playerPlayAttack == false)
        {
            playerAnimator.SetBool("meleeIsAttacking", false);
        }
    }

}