using UnityEngine;
using System.Collections;

public class CompanionAnimationManager : MonoBehaviour
{

    // References

    private float companionDirection;
    public Animator companionAnimator;
    private CharacterController companionCharacterController;
    public CompanionMovement companionMovement;
    public Transform companionVisuals;
    public CompanionCombat companionCombat;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        companionCharacterController = GetComponent<CharacterController>();
        companionAnimator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        companionDirection = Input.GetAxisRaw("Horizontal");

        if (companionDirection != 0)
        {
            Vector3 scale = companionVisuals.localScale;
            scale.x = (companionDirection < 0) ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.y);
            companionVisuals.localScale = scale;
        }

        MoveCheck();

        JumpCheck();

        AttackCheck();
    }

    void MoveCheck()
    {
        if (companionMovement.currentMoveDirection == Vector3.zero)
        {
            companionAnimator.SetBool("companionIsMoving", false);
        }

        else
        {
            companionAnimator.SetBool("companionIsMoving", true);
        }
    }

    void JumpCheck()
    {
        if (companionCharacterController.isGrounded == true)
        {
            companionAnimator.SetBool("companionIsJumping", false);
        }

        if (companionCharacterController.isGrounded == false)
        {
            companionAnimator.SetBool("companionIsJumping", true);
        }
    }

    void AttackCheck()
    {
        if (companionCombat.playCompanionAttack == true)
        {
            companionAnimator.SetBool("companionIsAttacking", true);
        }

        if (companionCombat.playCompanionAttack == false)
        {
            companionAnimator.SetBool("companionIsAttacking", false);
        }
    }

}