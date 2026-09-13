using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [Header("Bullet")]
    public float speed = 10f;
    public float damage = 10f;
    public float lifetime = 5f;

    private Vector3 direction;
    private bool hasHit = false;

    public void SetDirection(Vector3 newDirection)
    {
        direction = newDirection.normalized;
    }

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.position +=
            direction * speed * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        if (hasHit)
            return;

        Debug.Log(
            "Bullet collided with: " + other.gameObject.name
        );

        PlayerHealth playerHealth =
            other.GetComponentInParent<PlayerHealth>();

        if (playerHealth != null)
        {
            hasHit = true;

            Debug.Log(
                "Bullet hit Player! Damage = " + damage
            );

            playerHealth.TakeDamage(damage);

            Destroy(gameObject);
        }
    }
}






