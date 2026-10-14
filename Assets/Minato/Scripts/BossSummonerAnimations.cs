using UnityEngine;

public class BossSummonerAnimations : MonoBehaviour
{

    private float finalBossDirection;
    private CharacterController bossCharacterController;
    public Animator bossAnimator;
    public BossSummonerAI bossSummonerAI;
    public Transform finalBossVisuals;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        bossCharacterController = GetComponent<CharacterController>();
        bossAnimator = GetComponentInChildren<Animator>();
        bossAnimator.SetBool("bossIsMoving", true);
    }

    // Update is called once per frame
    void Update()
    {
        finalBossDirection = Input.GetAxisRaw("Horizontal");

        if (finalBossDirection != 0)
        {
            Vector3 scale = finalBossVisuals.localScale;
            scale.x = (finalBossDirection < 0) ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.y);
            finalBossVisuals.localScale = scale;
        }
    }
}
