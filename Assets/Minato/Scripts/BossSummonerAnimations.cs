using UnityEngine;

public class BossSummonerAnimations : MonoBehaviour
{

    private float finalBossDirection;
    private CharacterController bossCharacterController;
    public Animator bossAnimator;
    public BossSummonerAI bossSummonerAIScript;
    public BossBaseAI bossBaseAI;
    public Transform finalBossVisuals;
    public CutsceneStarter cutsceneStarter;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        bossCharacterController = GetComponent<CharacterController>();
        bossAnimator = GetComponentInChildren<Animator>();
        bossAnimator.SetBool("bossInAir", false);
        bossAnimator.SetBool("bossIsSummoning", false);
        bossAnimator.SetBool("bossCutscene", false);
    }

    // Update is called once per frame
    void Update()
    {

        CutsceneCheck();

        finalBossDirection = Input.GetAxisRaw("Horizontal");

        if (finalBossDirection != 0)
        {
            Vector3 scale = finalBossVisuals.localScale;
            scale.x = (finalBossDirection > 0) ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.y);
            finalBossVisuals.localScale = scale;
        }

        if (bossSummonerAIScript.bossPlaySummoning == true)
        {
            bossAnimator.SetBool("bossIsSummoning", true);
        }

        if (bossBaseAI.baseBossInAir == true)
        {
            bossAnimator.SetBool("bossInAir", true);
        }

        else
        {
            bossAnimator.SetBool("bossInAir", false);
            bossAnimator.SetBool("bossIsSummoning", false);
            bossAnimator.SetBool("bossIsAttacking", false);
        }
    }

    void CutsceneCheck()
    {
        if (cutsceneStarter.bossStart == true)
        {
            bossAnimator.SetBool("bossCutscene", true);
        }
    }

}
