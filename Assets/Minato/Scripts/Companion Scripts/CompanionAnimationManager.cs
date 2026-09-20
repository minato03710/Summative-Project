using UnityEngine;

public class CompanionAnimationManager : MonoBehaviour
{

    // This script manages the companion character's animations

    // Variables

    public Animator companionAnimator;

    // Movement

    private bool onGround; // Checks if companion is on the ground

    // Interactions

    private bool canExamine; // Checks if companion has found an object they can examine

    // Cutscenes & Other

    // Cutscene 1 - Player Meets Companion

    private bool companionWary;
    private bool companionScared;
    private bool companionIntimidate;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        companionAnimator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        CompanionMovementAnimations();
    }

    private void CompanionMovementAnimations() // Manages movement animations
    {
        if (onGround)
        {

        }
        else
        {
            // companionAnimator.SetBool()
        }
    }
}
