using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisappearingPlatform : MonoBehaviour
{
    [Header("Timers")]
    [SerializeField] private float timeBeforeDisappear = 2.0f;
    [SerializeField] private float timeBeforeRespawn = 3.0f;

    [Header("Visual Tuning")]
    [SerializeField] private float pulseSpeed = 3.0f;
    [SerializeField] private float flashSpeed = 15.0f;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color pulseColor = new Color(1.0f, 0.8f, 0.3f, 1.0f);
    [SerializeField] private Color warningColor = new Color(1.0f, 0.0f, 0.0f, 1.0f);

    private Renderer[] renderers;
    private Collider[] solidColliders;
    private Collider triggerCollider;
    private List<Material> materials = new List<Material>();

    private bool isTriggered = false;
    private bool isDisappeared = false;

    private void Awake()
    {
        Transform rootPlatform = transform.parent != null ? transform.parent : transform;

        renderers = rootPlatform.GetComponentsInChildren<Renderer>();
        
        // Separate solid physics colliders from the trigger detection zone
        Collider[] allCols = rootPlatform.GetComponentsInChildren<Collider>();
        List<Collider> solids = new List<Collider>();

        foreach (Collider c in allCols)
        {
            if (c.isTrigger)
            {
                triggerCollider = c;
            }
            else
            {
                solids.Add(c);
            }
        }
        solidColliders = solids.ToArray();

        materials.Clear();
        foreach (Renderer r in renderers)
        {
            foreach (Material mat in r.materials)
            {
                materials.Add(mat);
                SetMaterialColor(mat, normalColor);
            }
        }
    }

    private void Update()
    {
        if (!isTriggered && !isDisappeared)
        {
            PulsateLight();
        }
    }

    private void PulsateLight()
    {
        float lerpVal = (Mathf.Sin(Time.time * pulseSpeed) + 1.0f) / 2.0f;
        Color currentColor = Color.Lerp(normalColor, pulseColor, lerpVal);
        SetAllColors(currentColor);
    }

    private void SetAllColors(Color color)
    {
        foreach (Material mat in materials)
        {
            if (mat != null) SetMaterialColor(mat, color);
        }
    }

    private void SetMaterialColor(Material mat, Color color)
    {
        color.a = 1.0f;
        if (mat.HasProperty("_BaseColor"))
        {
            mat.SetColor("_BaseColor", color);
        }
        else
        {
            mat.color = color;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isTriggered || isDisappeared) return;

        if (other.CompareTag("Player"))
        {
            StartCoroutine(CollapseSequence());
        }
    }

    private IEnumerator CollapseSequence()
    {
        isTriggered = true;

        // 1. Flash Red for the set duration
        float elapsed = 0f;
        while (elapsed < timeBeforeDisappear)
        {
            elapsed += Time.deltaTime;
            float flashLerp = (Mathf.Sin(elapsed * flashSpeed) + 1.0f) / 2.0f;
            Color flashColor = Color.Lerp(normalColor, warningColor, flashLerp);
            SetAllColors(flashColor);
            yield return null;
        }

        // 2. Hide visuals, turn off solid collision, and disable trigger
        isDisappeared = true;
        TogglePlatformState(false);

        // 3. Wait for respawn timer
        yield return new WaitForSeconds(timeBeforeRespawn);

        // 4. Restore visuals, solid collision, and trigger
        TogglePlatformState(true);
        SetAllColors(normalColor);

        isTriggered = false;
        isDisappeared = false;
    }

    private void TogglePlatformState(bool visible)
    {
        foreach (Renderer r in renderers)
        {
            if (r != null) r.enabled = visible;
        }
        foreach (Collider c in solidColliders)
        {
            if (c != null) c.enabled = visible;
        }
        if (triggerCollider != null)
        {
            triggerCollider.enabled = visible;
        }
    }
}