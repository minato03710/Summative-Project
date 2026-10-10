using UnityEngine;
using System.Collections;

public class CompanionAnimationManager : MonoBehaviour
{

    // References

    public Animator companionAnimator;
    private CharacterController companionCharacterController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        companionCharacterController = GetComponent<CharacterController>();
        companionAnimator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        CompanionGroundCheck();
    }

    void CompanionMoveCheck()
    {

    }

    void CompanionGroundCheck()
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

    void CompanionActionCheck()
    {

    }

}