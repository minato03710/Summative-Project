using System.Collections;
using UnityEngine;

public class RedLightGreenLightManager : MonoBehaviour
{
    public static RedLightGreenLightManager Instance { get; private set; }

    public enum LightState { Green, Warning, Red }
    public LightState CurrentState { get; private set; } = LightState.Green;

    [Header("Timer Settings (Randomized)")]
    [SerializeField] private float minGreenDuration = 2.5f;
    [SerializeField] private float maxGreenDuration = 5.0f;
    [SerializeField] private float warningDuration = 0.75f;
    [SerializeField] private float minRedDuration = 2.0f;
    [SerializeField] private float maxRedDuration = 4.0f;

    [Header("Visual Indicators")]
    [SerializeField] private Renderer lightRenderer;
    [SerializeField] private Light targetPointLight;
    [ColorUsage(true, true)] [SerializeField] private Color greenColor = Color.green;
    [ColorUsage(true, true)] [SerializeField] private Color warningColor = Color.yellow;
    [ColorUsage(true, true)] [SerializeField] private Color redColor = Color.red;

    [Header("Pulse Effects")]
    [SerializeField] private bool enablePulse = true;
    [SerializeField] private float pulseSpeed = 5.0f;
    [SerializeField] private float minIntensity = 0.5f;
    [SerializeField] private float maxIntensity = 3.5f;

    private Material lightMaterial;
    private Color activeBaseColor;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        if (lightRenderer != null)
        {
            lightMaterial = lightRenderer.material;
            lightMaterial.EnableKeyword("_EMISSION");
        }

        activeBaseColor = greenColor;
        StartCoroutine(LightCycleRoutine());
    }

    private void Update()
    {
        UpdatePulse();
    }

    private IEnumerator LightCycleRoutine()
    {
        while (true)
        {
            SetState(LightState.Green);
            yield return new WaitForSeconds(Random.Range(minGreenDuration, maxGreenDuration));

            if (warningDuration > 0)
            {
                SetState(LightState.Warning);
                yield return new WaitForSeconds(warningDuration);
            }

            SetState(LightState.Red);
            yield return new WaitForSeconds(Random.Range(minRedDuration, maxRedDuration));
        }
    }

    private void SetState(LightState newState)
    {
        CurrentState = newState;

        if (newState == LightState.Green) activeBaseColor = greenColor;
        else if (newState == LightState.Warning) activeBaseColor = warningColor;
        else if (newState == LightState.Red) activeBaseColor = redColor;

        if (lightMaterial != null) lightMaterial.color = activeBaseColor;
        if (targetPointLight != null) targetPointLight.color = activeBaseColor;

        Debug.Log($"[TrafficBot] Light changed to: {CurrentState}");
    }

    private void UpdatePulse()
    {
        float intensityFactor = 1.0f;

        if (enablePulse)
        {
            float sineWave = (Mathf.Sin(Time.time * pulseSpeed) + 1.0f) / 2.0f; 
            intensityFactor = Mathf.Lerp(minIntensity, maxIntensity, sineWave);
        }

        if (lightMaterial != null)
        {
            Color finalEmissionColor = activeBaseColor * intensityFactor;
            lightMaterial.SetColor("_EmissionColor", finalEmissionColor);
        }

        if (targetPointLight != null)
        {
            targetPointLight.intensity = intensityFactor;
        }
    }
}