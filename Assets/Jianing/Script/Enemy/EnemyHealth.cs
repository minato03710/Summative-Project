using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.VFX;


[DisallowMultipleComponent]
public class EnemyHealth : MonoBehaviour
{
    [Header("Health")]
    [Min(1f)]
    public float maxHealth = 100f;

    [Header("VFX")]
    public VisualEffect DamageVFX;
    public VisualEffect DeathVFX;

    [Header("SFX")]
    public AudioSource DamageSFX;
    public AudioSource DestroySFX;

    [Header("Boss Second Life")]
    public bool useSecondLife = false;
    public bool isFinalBoss;

    [Min(0f)]
    public float reviveProtectionSeconds = 1f;

    public event Action SecondLifeStarted;
    public event Action Died;

    public bool IsSecondLife { get; private set; }

    public bool IsReviving =>
        !isDead && Time.timeAsDouble < protectedUntil;

    public float currentHealth;
    private bool initialized;
    private bool isDead;

    private double protectedUntil;

    private void Awake()
    {
        InitializeHealth();

        SimpleVFX.Prepare(DamageVFX);
        SimpleVFX.Prepare(DeathVFX);
        
    }

    private void InitializeHealth()
    {
        if (initialized)
            return;

        initialized = true;

        maxHealth = Mathf.Max(1f, maxHealth);
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        InitializeHealth();

        if (isDead || IsReviving || damage <= 0f)
            return;

        currentHealth = Mathf.Max(
            0f,
            currentHealth - damage
        );

        SimpleVFX.Spawn(DamageVFX, 0.15f);
        DamageSFX.Play();


        if (currentHealth > 0f)
            return;

        if (useSecondLife && !IsSecondLife)
            BeginSecondLife();
        else
            
            Die();
    }

    private void BeginSecondLife()
    {
        IsSecondLife = true;
        currentHealth = Mathf.Max(1f, maxHealth);

        protectedUntil =
            Time.timeAsDouble +
            Mathf.Max(0f, reviveProtectionSeconds);

        // 清除上一条命的中毒状态。
        EnemyPoison poison = GetComponent<EnemyPoison>();

        if (poison != null)
            poison.enabled = false;

        SecondLifeStarted?.Invoke();
    }

    private void Die()
    {
        
        if (isDead)
            return;

        isDead = true;
        currentHealth = 0f;

        SimpleVFX.Spawn(DeathVFX, 0.2f, 3f);
        

        Died?.Invoke();

        EnemyDropSystem dropSystem =
            GetComponent<EnemyDropSystem>();

        if (dropSystem != null)
            dropSystem.DropResources();


        if (DestroySFX != null) // Plays destroy SFX
        {
            AudioSource audio = DestroySFX;

            audio.transform.SetParent(null);
            audio.spatialBlend = 0f;
            //audio.volume = 1f;
            audio.Play();

            Destroy(audio.gameObject, audio.clip.length + 0.1f);
        }


        Destroy(gameObject);
    }

    public float GetCurrentHealth()
    {
        InitializeHealth();
        return currentHealth;
    }

    public float GetMaxHealth()
    {
        return Mathf.Max(1f, maxHealth);
    }

    public bool IsDead()
    {
        return isDead;
    }


    public void OnDestroy()
    {
        
        if (isFinalBoss == true)
        {

            SceneManager.LoadScene("GoodEnding");
        }
    }

}