using UnityEngine;

public class PlayerSpriteBillboard : MonoBehaviour
{
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void LateUpdate()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
            

        transform.forward =
            mainCamera.transform.forward;
    }


}

