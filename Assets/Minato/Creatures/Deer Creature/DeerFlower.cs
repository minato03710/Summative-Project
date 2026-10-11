using UnityEngine;

public class DeerFlower : MonoBehaviour
{

    public bool newFlower;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        newFlower = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider collision)
    {
        newFlower = true;
        Destroy(gameObject);
    }

}