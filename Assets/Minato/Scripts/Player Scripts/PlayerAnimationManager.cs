using UnityEngine;

public class PlayerAnimationManager : MonoBehaviour
{

    public Animator playerAnimator;
    private bool playerWalking;
    private CharacterController characterController;
    public PlayerMovement playerMovement;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        playerAnimator = GetComponentInChildren<Animator>(); // References animator from player visuals
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            playerAnimator.SetBool("isWalking", true);
        }

        if(Input.GetKeyDown(KeyCode.D))
        {
            playerAnimator.SetBool("isWalking", true);
        }

        if (Input.GetKeyUp(KeyCode.A))
        {
            playerAnimator.SetBool("isWalking", false);
        }

        if (Input.GetKeyUp(KeyCode.D))
        {
            playerAnimator.SetBool("isWalking", false);
        }

        PlayerJump();
    }

    void PlayerJump()
    {
        if(characterController.isGrounded)
        {
            playerAnimator.SetBool("onGround", true);
        }

        else
        {
            playerAnimator.SetBool("onGround", false);
        }
    }

}