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
    private float moveSpeed = 2f;
    private bool isAlive = true;
    private bool isDead = false;
    public GameObject player;
    private Vector3 originalScale;
    private float attackRange = 1.5f;
    public EnemyAxeAttack enemyaxe;
    private float attackCooldown = 1f;
    private float lastAttackTime;
    private float viewDistance = 7f;



    [Header("UI")]
    public Image healthBarImage;
    public RectTransform healthBarRect;
    public Vector3 offset = new Vector3(0, 2f, 0);


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        currentHealth = maxHealth;
        originalScale = transform.localScale;
        if (anim == null)
            anim = GetComponent<Animator>() ?? GetComponentInParent<Animator>();
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
    private void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        Vector3 direction = (player.transform.position - transform.position).normalized;

        if (distanceToPlayer < attackRange && Time.time - lastAttackTime > attackCooldown)
        {
            anim.SetTrigger("AttackE");
            if (enemyaxe != null)
            {
                enemyaxe.AxeAnimation();
            }
            lastAttackTime = Time.time;
        }

        if (isAlive && distanceToPlayer > 1f && distanceToPlayer < viewDistance)
        {

            rb.linearVelocity = new Vector2(direction.x * moveSpeed, rb.linearVelocity.y);
            anim.SetFloat("SpeedE", Mathf.Abs(rb.linearVelocity.x));

            if (direction.x > 0)
                transform.localScale = new Vector3(Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
            else
                transform.localScale = new Vector3(-Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);

        }
        else
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            anim.SetFloat("SpeedE", 0f);
        }
    }


    public void GetDamage(float damageAmount)
    {
        if (!isAlive) return;

        currentHealth -= damageAmount;
        anim.SetTrigger("Hurt");
        UpdateHealthBar();

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

        Destroy(gameObject, 1f);
    }
}