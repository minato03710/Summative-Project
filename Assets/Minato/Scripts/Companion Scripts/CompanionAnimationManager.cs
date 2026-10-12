using UnityEngine;
using System.Collections;

public class CompanionAnimationManager : MonoBehaviour
{

    // References

    public Animator companionAnimator;
    private CharacterController companionCharacterController;
    public CompanionMovement companionMovement;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        companionCharacterController = GetComponent<CharacterController>();
        companionAnimator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        MoveCheck();

        JumpCheck();
    }

    void MoveCheck()
    {
        if (companionMovement.currentMoveDirection == Vector3.zero)
        {
            companionAnimator.SetBool("companionIsMoving", false);
        }

        else
        {
            FlipVisuals();
            companionAnimator.SetBool("companionIsMoving", true);
        }
    }

    void JumpCheck()
    {
        if (companionCharacterController.isGrounded)
        {
            companionAnimator.SetBool("companionIsJumping", false);
        }

        else
        {
            companionAnimator.SetBool("companionIsJumping", true);
        }
    }

    void FlipVisuals()
    {
        if (companionMovement.currentMoveDirection.x > 0f)
        {

        }

        if (companionMovement.currentMoveDirection.x < 0f)
        {

        }
    }

}