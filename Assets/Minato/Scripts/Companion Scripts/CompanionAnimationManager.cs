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
        JumpCheck();
    }

    void JumpCheck()
    {
        if (companionMovement.companionPlayJump == false)
        {
            companionAnimator.SetBool("companionIsJumping", true);
        }

        if (companionMovement.companionPlayJump == true)
        {
            companionAnimator.SetBool("companionIsJumping", false);
        }
    }

}