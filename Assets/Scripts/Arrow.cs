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
    }
}
