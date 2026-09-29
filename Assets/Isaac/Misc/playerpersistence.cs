using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerPersistence : MonoBehaviour
{
    public static PlayerPersistence Instance { get; private set; }

    private void Awake()
    {
        transform.SetParent(null);

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 1. Move player to the new scene's Spawn Point
        GameObject spawnPoint = GameObject.FindWithTag("SpawnPoint");
        if (spawnPoint != null)
        {
            CharacterController cc = GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;

            transform.position = spawnPoint.transform.position;
            transform.rotation = spawnPoint.transform.rotation;

            if (cc != null) cc.enabled = true;
            Debug.Log($"[PlayerPersistence] Teleported to SpawnPoint in {scene.name}");
        }
        else
        {
            Debug.LogWarning($"[PlayerPersistence] No object tagged 'SpawnPoint' found in {scene.name}!");
        }

        // 2. Automatically link camera to the persistent player
        BindCameraToPlayer();
    }

    private void BindCameraToPlayer()
    {
        // A. Check for a custom follow script on Main Camera (e.g. CameraFollow)
        if (Camera.main != null)
        {
            // IMPORTANT: If your teammate's script name is not "CameraFollow", 
            // replace "CameraFollow" below with their exact script name!
            var customCam = Camera.main.GetComponent<CameraFollow>();
            if (customCam != null)
            {
                customCam.target = transform;
                Debug.Log("[PlayerPersistence] Linked custom camera script to persistent player.");
                return;
            }
        }

        // B. Fallback check for Cinemachine 3.x (Unity 6)
        var cinemachineCam = Object.FindFirstObjectByType<Unity.Cinemachine.CinemachineCamera>();
        if (cinemachineCam != null)
        {
            cinemachineCam.Target.TrackingTarget = transform;
            cinemachineCam.Target.LookAtTarget = transform;
            Debug.Log("[PlayerPersistence] Linked Cinemachine 3.x camera to persistent player.");
            return;
        }

        // C. Fallback check for Cinemachine 2.x
        #pragma warning disable CS0618
        var vcam = Object.FindFirstObjectByType<Unity.Cinemachine.CinemachineVirtualCamera>();
        if (vcam != null)
        {
            vcam.Follow = transform;
            vcam.LookAt = transform;
            Debug.Log("[PlayerPersistence] Linked Cinemachine 2.x camera to persistent player.");
        }
        #pragma warning restore CS0618
    }
}