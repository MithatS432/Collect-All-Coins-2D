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

    private bool isDead = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        currentHealth = maxHealth;
    }

    void LateUpdate()
    {
        if (isDead || healthBarRect == null) return;

        try
        {
            Vector3 screenPoint = Camera.main.WorldToScreenPoint(transform.position + offset);
            healthBarRect.position = screenPoint;
        }
        catch
        {
            if (healthBarRect != null)
                Destroy(healthBarRect.gameObject);
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
            healthBarImage.fillAmount = currentHealth / maxHealth;
        }
    }

    private void Die()
    {
        isAlive = false;
        isDead = true;
        anim.SetTrigger("Die");

        if (healthBarRect != null)
        {
            Destroy(healthBarRect.gameObject);
            healthBarRect = null;
        }

        Destroy(gameObject, 2f);
    }
}