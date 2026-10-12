using UnityEngine;

public class BossSummonerAnimations : MonoBehaviour
{

    private float bossDirection;
    private CharacterController bossCharacterController;
    public Animator bossAnimator;
    public BossSummonerAI bossSummonerAI;
    public Transform bossVisuals;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        bossCharacterController = GetComponent<CharacterController>();
        bossAnimator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        bossDirection = Input.GetAxisRaw("Horizontal");

        if (bossDirection != 0)
        {
            Vector3 scale = bossVisuals.localScale;
            scale.x = (bossDirection < 0) ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.y);
            bossVisuals.localScale = scale;
        }

        MoveCheck();
    }

    void MoveCheck()
    {
        if (bossSummonerAI.bossPlayMoving == false)
        {
            bossAnimator.SetBool("bossIsMoving", false);
        }

        if (bossSummonerAI.bossPlayMoving == true)
        {
            bossAnimator.SetBool("bossIsMoving", true);
        }
    }
}
