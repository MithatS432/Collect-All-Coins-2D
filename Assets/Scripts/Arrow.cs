using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float speed = 50f;
    public float damage = 10f;

    private Vector2 direction;

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
        transform.Translate(direction * speed * Time.deltaTime);
        if (Mathf.Abs(transform.position.x) > 50 || Mathf.Abs(transform.position.y) > 50)
        {
            Destroy(gameObject);
        }
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Enemy enemy = other.gameObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.GetDamage(damage);
                Destroy(gameObject);

            }
        }
    }
}
