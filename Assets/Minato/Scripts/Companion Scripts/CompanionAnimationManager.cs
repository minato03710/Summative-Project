using UnityEngine;
using System.Collections;

public class CompanionAnimationManager : MonoBehaviour
{

    // References

    private float companionDirection;
    private float companionVerticalDirection;
    public Animator companionAnimator;
    private CharacterController companionCharacterController;
    public CompanionMovement companionMovement;
    public Transform companionVisuals;
    public CompanionCombat companionCombat;
    public CompanionCommandController companionCommandController;

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

        companionVerticalDirection = Input.GetAxisRaw("Vertical");

        if (companionVerticalDirection != 0)
        {
            companionAnimator.SetBool("companionIsMoving", true);
        }

        else
        {
            companionAnimator.SetBool("companionIsMoving", false);
        }


        MoveCheck();

        AttackCheck();

        HealCheck();
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

        JumpCheck();
    }

    void JumpCheck()
    {
            companionAnimator.SetBool("companionIsGrounded",
            companionCharacterController != null &&
            companionCharacterController.isGrounded);
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

    void HealCheck()
    {
        if (companionCommandController.companionPlayHeal == true)
        {
            companionAnimator.SetBool("companionHealing", true);
        }

    }

}