using UnityEngine;
using System.Collections;

public class BossAnimationManager : MonoBehaviour
{
    
    // References

    private CharacterController bossCharacterController;
    public Animator bossAnimator;
    public BossBruteAI bossBruteAI;

    private float smallBossDirection;
    public Transform smallBossVisuals;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        bossCharacterController = GetComponent<CharacterController>();
        bossAnimator = GetComponentInChildren<Animator>();

        // Boss begins in idle state until player noticed

        bossAnimator.SetBool("beetleIsGrounded", true);
    }

    // Update is called once per frame
    void Update()
    {

        // smallBossDirection = Input.GetAxisRaw("Horizontal");

        // if (smallBossDirection != 0)
        {
            // Vector3 scale = smallBossVisuals.localScale;
            // scale.x = (smallBossDirection < 0) ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.y);
            // smallBossVisuals.localScale = scale;
        }

        GroundCheck();

        // if () Attacking
        {
            //bossAnimator.SetBool("bossIsAttacking", true);
        }

        // if () Summoning
        {
            //bossAnimator.SetBool("bossIsSummoning", true);
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
            bossAnimator.SetBool("beetleIsGrounded", true);
            }

            else
            {
                bossAnimator.SetBool("beetleIsGrounded", false);
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