using UnityEngine;

public class MeleeAnimationManager : MonoBehaviour
{

    // References

    public Animator meleeAnimator;
    private CharacterController meleeCharacterController;
    public MeleeEnemyAI meleeEnemyAI;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        meleeCharacterController = GetComponent<CharacterController>();
        meleeAnimator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        AttackCheck();
        meleeAnimator.SetBool("meleeOnGround", true);
        meleeAnimator.SetBool("meleeIsDashing", false);
        meleeAnimator.SetBool("meleeIsMoving", true);
    }

    private void AttackCheck()
    {
        if (meleeEnemyAI.meleePlayAttack == true)
        {
            meleeAnimator.SetBool("meleeIsAttacking", true);
            Debug.Log("Melee enemy attacking");
        }

        if (meleeEnemyAI.meleePlayAttack == false)
        {
            meleeAnimator.SetBool("meleeIsAttacking", false);
            meleeAnimator.SetBool("meleeIsMoving", true);
            Debug.Log("Melee enemy stopped attacking");
        }
    }
}
