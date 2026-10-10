using UnityEngine;
using System.Collections;

public class BossAnimationManager : MonoBehaviour
{
    
    // References

    private CharacterController bossCharacterController;
    public Animator bossAnimator;
    public BossBruteAI bossBruteAI;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        bossCharacterController = GetComponent<CharacterController>();
        bossAnimator = GetComponentInChildren<Animator>();

        // Boss begins in idle state until player noticed

        bossAnimator.SetBool("bossIsMoving", false);
    }

    // Update is called once per frame
    void Update()
    {

        MoveCheck(); // Checks if walking or idle
        GroundCheck(); // Checks if grounded or flying
        DashCheck(); // Checks if dashing

        // if () Attacking
        {
            //bossAnimator.SetBool("bossIsAttacking", true);
        }

        // if () Summoning
        {
            //bossAnimator.SetBool("bossIsSummoning", true);
        }

        void MoveCheck()
        {
            if (bossBruteAI.bossPlayMoving == false)
            {
                bossAnimator.SetBool("bossIsMoving", false);
            }

            if (bossBruteAI.bossPlayMoving == true)
            {
                bossAnimator.SetBool("bossIsMoving", true);
            }
        }

        void GroundCheck()
        {
            if (bossCharacterController.isGrounded)
            {
            bossAnimator.SetBool("bossInAir", false);
            }

            else
            {
                bossAnimator.SetBool("bossInAir", true);
            }
        }

        void DashCheck()
        {
            if (bossBruteAI.bossPlayDash == true)
            {
                Debug.Log("Dash coroutine started");
                StartCoroutine(DashAnimationTime());
            }
        }

        IEnumerator DashAnimationTime()
        {
            bossAnimator.SetBool("bossIsDashing", true);
            yield return new WaitForSeconds(3f);
            bossAnimator.SetBool("bossIsDashing", false);
            yield return null;
            Debug.Log("Dash coroutine ended");
        }

    }
}