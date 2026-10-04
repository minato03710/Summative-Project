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
            var customCam = Camera.main.GetComponent<CameraFollow>();
            if (customCam != null)
            {
                customCam.target = transform;
                Debug.Log("[PlayerPersistence] Linked custom camera script to persistent player.");
                return;
            }
        }
    }
}