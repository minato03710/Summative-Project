using UnityEngine;
using System.Collections;

public class BossAnimationManager : MonoBehaviour
{
    
    // References

    [SerializeField] public CharacterController bossCharacterController;
    public Animator bossAnimator;
    public BossBruteAI bossBruteAI;

    private float smallBossDirection;
    public Transform smallBossVisuals;

    public bool canDash;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        bossAnimator = GetComponentInChildren<Animator>();

        // Boss begins in idle state until player noticed

        bossAnimator.SetBool("beetleIsGrounded", true);
        bossAnimator.SetBool("beetleIsMoving", false);
    }

    // Update is called once per frame
    void Update()
    {

        smallBossDirection = Input.GetAxisRaw("Horizontal");

        if (smallBossDirection != 0)
        {
            Vector3 scale = smallBossVisuals.localScale;
            scale.x = (smallBossDirection < 0) ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.y);
            smallBossVisuals.localScale = scale;
            bossAnimator.SetBool("beetleIsMoving", true);
        }

        else
        {
            bossAnimator.SetBool("beetleIsMoving", false);
            bossAnimator.SetBool("beetleIsDashing", false);
        }

        GroundCheck();
        DashCheck();

        // if () Summoning
        {
            //bossAnimator.SetBool("bossIsSummoning", true);
        }

    }

    void GroundCheck()
        {
            if (bossCharacterController.isGrounded)
            {
            Debug.Log("Boss is grounded");
            bossAnimator.SetBool("beetleIsGrounded", true);
            DashCheck();
            }

            else
            {
                bossAnimator.SetBool("beetleIsGrounded", false);
            }
        }

    void DashCheck()
        {

            if (bossBruteAI.bossPlayDash)
                {
                    canDash = true;
                    bossAnimator.SetBool("beetleIsDashing", true);
                }

            if (canDash)
            {
                if (bossAnimator.GetBool("beetleIsDashing"))
                {
                    StartCoroutine("DashStop");
                }
            }
        }

    IEnumerator DashStop()
    {
        yield return new WaitForSeconds(5f);
        bossAnimator.SetBool("beetleIsDashing", false);
        yield return null;
    }

}