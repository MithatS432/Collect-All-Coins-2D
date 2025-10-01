using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float speed = 50f;
    public float damage = 10f;
    public LayerMask collisionLayers;

    private Vector2 direction;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }
    }

    public void Initialize(Vector2 shootDirection)
    {
        direction = shootDirection.normalized;

        if (direction.x < 0)
            transform.localScale = new Vector3(-1, 1, 1);
        else
            transform.localScale = new Vector3(1, 1, 1);
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime, Space.World);

        if (Mathf.Abs(transform.position.x) > 125 || Mathf.Abs(transform.position.y) > 50)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) return;

        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.GetDamage(damage);
            }
            Destroy(gameObject);
        }
    }
}