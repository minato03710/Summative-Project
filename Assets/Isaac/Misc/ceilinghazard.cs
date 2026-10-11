using System.Collections;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class CeilingHazard : MonoBehaviour
{
    [Header("Timing Settings")]
    [SerializeField] private float minWaitTime = 4f;
    [SerializeField] private float maxWaitTime = 9f;
    [SerializeField] private float warningDuration = 0.8f; 
    [SerializeField] private float stayOnGroundDelay = 2.5f;

    [Header("Drop Settings")]
    [SerializeField] private float dropSlamSpeed = 35f; 

    [Header("Effects")]
    [SerializeField] private GameObject warningIndicatorPrefab; 
    [SerializeField] private GameObject impactParticlePrefab; 
    [SerializeField] private float fadeDuration = 0.8f; 
    
    [Header("Camera Shake Settings")]
    [SerializeField] private bool enableCameraShake = true; 
    [SerializeField] private float camShakeDuration = 0.2f;
    [SerializeField] private float camShakeIntensity = 0.15f;

    [Header("Combat")]
    [SerializeField] private int damageAmount = 25;

    private Vector3 ceilingPosition;
    private Rigidbody rb;
    private MeshRenderer meshRenderer;
    private bool isFalling = false;
    private GameObject activeWarning;
    private Material hazardMaterial;
    private Color originalMaterialColor;

    private void Awake()
    {
        ceilingPosition = transform.position;
        rb = GetComponent<Rigidbody>();
        meshRenderer = GetComponent<MeshRenderer>();

        hazardMaterial = meshRenderer.material; 
        originalMaterialColor = hazardMaterial.color;

        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }
    }

    private void Start()
    {
        StartCoroutine(HazardRoutine());
    }

    private IEnumerator HazardRoutine()
    {
        while (true)
        {
            ResetMaterialAlpha();

            // 1. Wait for random interval
            float randomWait = Random.Range(minWaitTime, maxWaitTime);
            yield return new WaitForSeconds(randomWait);

            // 2. Pick a random target spot across the ceiling
            float randomXOffset = Random.Range(-4f, 4f);
            float randomZOffset = Random.Range(-4f, 4f);
            Vector3 targetCeilingPos = ceilingPosition + new Vector3(randomXOffset, 0f, randomZOffset);

            // 3. Raycast down BEFORE moving the spike there, so it never hits itself!
            Vector3 groundHitPoint = new Vector3(targetCeilingPos.x, 0f, targetCeilingPos.z);
            if (Physics.Raycast(targetCeilingPos, Vector3.down, out RaycastHit hit, 100f))
            {
                groundHitPoint = hit.point + new Vector3(0, 0.02f, 0); 
            }
            else
            {
                Debug.LogWarning("[Hazard] Raycast missed the floor! Using fallback floor height.");
            }

            // Now move the spike to its drop position
            transform.position = targetCeilingPos;

            // 4. Spawn Warning Indicator
            if (warningIndicatorPrefab != null)
            {
                activeWarning = Instantiate(warningIndicatorPrefab, groundHitPoint, Quaternion.Euler(90f, 0f, 0f));
            }

            // 5. Warning Phase
            float warningElapsed = 0f;
            while (warningElapsed < warningDuration)
            {
                warningElapsed += Time.deltaTime;
                float progress = warningElapsed / warningDuration;

                if (activeWarning != null)
                {
                    activeWarning.transform.localScale = Vector3.Lerp(Vector3.zero, new Vector3(3f, 3f, 1f), progress);
                }
                yield return null;
            }

            if (activeWarning != null) { Destroy(activeWarning); }

            // 6. SNAP DROP SLAM
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.useGravity = true;
                rb.linearVelocity = Vector3.down * dropSlamSpeed; 
            }
            isFalling = true;

            float fallTimer = 0f;
            while (isFalling && fallTimer < 3f)
            {
                fallTimer += Time.deltaTime;
                yield return null;
            }

            // 7. Ground Time -> Fadeout
            yield return new WaitForSeconds(stayOnGroundDelay);

            float fadeElapsed = 0f;
            while (fadeElapsed < fadeDuration)
            {
                fadeElapsed += Time.deltaTime;
                float alphaProgress = 1.0f - Mathf.Clamp01(fadeElapsed / fadeDuration);
                
                if (hazardMaterial != null)
                {
                    Color lerpedColor = originalMaterialColor;
                    lerpedColor.a = alphaProgress;
                    hazardMaterial.color = lerpedColor;
                }
                yield return null;
            }

            ResetToCeiling();
        }
    }

    private void ResetMaterialAlpha()
    {
        if (hazardMaterial != null)
        {
            hazardMaterial.color = originalMaterialColor;
        }
    }

    private void ResetToCeiling()
    {
        isFalling = false;
        ResetMaterialAlpha();

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = false;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        transform.position = ceilingPosition;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isFalling) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            var playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageAmount);
            }
            Debug.Log($"[Hazard] Slam hit Player for {damageAmount} damage!");
        }

        isFalling = false;
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = false;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        // Find the exact contact point on the floor for the dust
        Vector3 contactPoint = transform.position;
        if (collision.contacts.Length > 0)
        {
            contactPoint = collision.contacts[0].point;
        }

        // Spawn Dust Particles precisely on the floor surface
        if (impactParticlePrefab != null)
        {
            Instantiate(impactParticlePrefab, contactPoint + new Vector3(0, 0.05f, 0), Quaternion.identity);
        }

        // Optional Camera Shake based on Inspector toggle
        if (enableCameraShake)
        {
            StartCoroutine(ShakeCameraRoutine());
        }
    }

    private IEnumerator ShakeCameraRoutine()
    {
        if (Camera.main == null) yield break;

        Transform camTransform = Camera.main.transform;
        Vector3 originalCamPos = camTransform.localPosition;
        float elapsed = 0f;

        while (elapsed < camShakeDuration)
        {
            Vector3 randomOffset = Random.insideUnitSphere * camShakeIntensity;
            camTransform.localPosition = originalCamPos + new Vector3(randomOffset.x, randomOffset.y, 0f);
            
            elapsed += Time.deltaTime;
            yield return null;
        }

        camTransform.localPosition = originalCamPos;
    }
}