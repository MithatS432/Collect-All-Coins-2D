using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [Header("Components")]
    private Rigidbody2D rb;
    private Animator anim;

    [Header("Stats")]
    private float maxHealth = 50f;
    private float currentHealth;
    private bool isAlive = true;

    [Header("UI")]
    public Image healthBarImage;
    public RectTransform healthBarRect;
    public Vector3 offset = new Vector3(0, 2f, 0);

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        currentHealth = maxHealth;
    }

    void Update()
    {
        if (healthBarRect != null)
        {
            healthBarRect.position = Camera.main.WorldToScreenPoint(transform.position + offset);
        }
    }

    public void GetDamage(float damageAmount)
    {
        if (!isAlive) return;

        currentHealth -= damageAmount;
        UpdateHealthBar();
        anim.SetTrigger("Hurt");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void UpdateHealthBar()
    {
        if (healthBarImage != null)
        {
            float healthPercent = Mathf.Clamp01(currentHealth / maxHealth);
            healthBarImage.fillAmount = healthPercent;
        }
    }

    private void Die()
    {
        isAlive = false;
        anim.SetTrigger("Die");
        Destroy(gameObject, 2f);

        if (healthBarRect != null)
            Destroy(healthBarRect.gameObject);
    }
}
