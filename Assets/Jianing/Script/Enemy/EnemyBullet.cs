using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [Header("Bullet")]
    public float speed = 10f;
    public float damage = 10f;
    public float lifetime = 5f;


[Header("Layers")]
    public LayerMask obstacleLayer;

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

        // =========================
        // 1. ºöÂÔµÐÈË
        // =========================

        if (other.CompareTag("Enemy"))
        {
            return;
        }

        // =========================
        // 2. Íæ¼Ò
        // =========================

        PlayerHealth playerHealth =
            other.GetComponentInParent<PlayerHealth>();

        if (playerHealth != null)
        {
            hasHit = true;

            Debug.Log(
                "Bullet hit Player! Damage = " +
                damage
            );

            playerHealth.TakeDamage(damage);

            Destroy(gameObject);

            return;
        }

        // =========================
        // 3. ³¡¾°ÕÏ°­Îï
        // =========================

        if (obstacleLayer.value != 0)
        {
            if (((1 << other.gameObject.layer) &
                 obstacleLayer.value) != 0)
            {
                hasHit = true;

                Debug.Log(
                    "Bullet hit obstacle: " +
                    other.gameObject.name
                );

                Destroy(gameObject);

                return;
            }
        }
    }


}








