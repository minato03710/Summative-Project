using UnityEngine;
using System.Collections;

public class RangedEnemyAnimationManager : MonoBehaviour
{

    // References

    public Animator rangedAnimator;
    private CharacterController rangedCharacterController;
    public RangedEnemyAI rangedScript;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rangedCharacterController = GetComponent<CharacterController>();
        rangedAnimator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (rangedScript.rangedPlayAttack == true)
        {
            Debug.Log("Ranged attacking");
            rangedAnimator.SetBool("rangedIsAttacking", true);
        }

        if (rangedScript.rangedPlayAttack == false)
        {
            Debug.Log("Ranged not attacking");
            rangedAnimator.SetBool("rangedIsAttacking", false);
        }
    }

}