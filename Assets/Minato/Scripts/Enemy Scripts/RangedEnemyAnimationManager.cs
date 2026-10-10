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
        // if (rangedEnemyScript)
        {
            RangedAttack();
        }
    }

    void RangedAttack()
    {
        StartCoroutine(RangedAnimationTime());
    }

    IEnumerator RangedAnimationTime()
        {
            // rangedAnimator.SetBool
            yield return new WaitForSeconds(3f);
            // rangedAnimator.SetBool
            yield return null;
        }

}