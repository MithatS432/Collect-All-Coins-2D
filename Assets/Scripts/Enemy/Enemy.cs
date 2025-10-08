using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Components")]
    private Rigidbody2D rb;
    private Animator anim;

    [Header("Stats")]
    [SerializeField] private float maxHealth = 50f;
    private float currentHealth;
    [SerializeField] private float moveSpeed = 2f;
    private bool isAlive = true;
    public GameObject player;
    private Vector3 originalScale;
    [SerializeField] private float attackRange = 1.5f;
    public EnemyAxeAttack enemyaxe;
    [SerializeField] private float attackCooldown = 1f;
    private float lastAttackTime;
    [SerializeField] private float viewDistance = 7f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        currentHealth = maxHealth;
        originalScale = transform.localScale;

        // Rigidbody ayarlarını yap
        SetupRigidbody();
    }

    void Start()
    {
        if (player == null)
        {
            player = GameObject.FindWithTag("Player");
        }
    }

    // YENİ METOD: RIGIDBODY AYARLARI
    private void SetupRigidbody()
    {
        if (rb != null)
        {
            rb.gravityScale = 3f; // Yerçekimi - düşmemesi için normal değer
            rb.freezeRotation = true; // Rotasyonu kilitle
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous; // Daha iyi çarpışma
        }
    }

    void FixedUpdate()
    {
        if (!isAlive || player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        Vector3 direction = (player.transform.position - transform.position).normalized;

        if (distanceToPlayer > attackRange && distanceToPlayer < viewDistance)
        {
            // Yatay hareket - dikey hareketi etkilemesin
            rb.linearVelocity = new Vector2(direction.x * moveSpeed, rb.linearVelocity.y);
            anim.SetFloat("SpeedE", Mathf.Abs(rb.linearVelocity.x));

            // Yön değiştirme
            Vector3 scale = originalScale;
            scale.x = Mathf.Sign(direction.x) * Mathf.Abs(originalScale.x);
            transform.localScale = scale;
        }
        else
        {
            // Sadece yatay hızı sıfırla, dikey hızı koru (yerçekimi için)
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            anim.SetFloat("SpeedE", 0f);
        }

        // Saldırı
        if (distanceToPlayer <= attackRange && Time.time - lastAttackTime > attackCooldown)
        {
            anim.SetTrigger("AttackE");
            if (enemyaxe != null)
            {
                enemyaxe.AxeAnimation();
            }
            lastAttackTime = Time.time;
        }
    }

    public void GetDamage(float damageAmount)
    {
        if (!isAlive) return;

        currentHealth -= damageAmount;
        anim.SetTrigger("Hurt");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (!isAlive) return;

        isAlive = false;
        anim.SetTrigger("Die");

        // 🚨 ÖNEMLİ: Tüm fizik işlemlerini durdur
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.gravityScale = 0f; // Yerçekimini kapat
            rb.bodyType = RigidbodyType2D.Kinematic; // Fizik etkileşimini durdur (isKinematic yerine)
        }

        // Collider'ı devre dışı bırak
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        // Diğer collider'ları da devre dışı bırak
        Collider2D[] allColliders = GetComponentsInChildren<Collider2D>();
        foreach (Collider2D collider in allColliders)
        {
            collider.enabled = false;
        }

        Destroy(gameObject, 1f);
    }

    // Alternatif: Enemy ölünce sabit kalsın istiyorsan
    private void DieFixedPosition()
    {
        if (!isAlive) return;

        isAlive = false;
        anim.SetTrigger("Die");

        // Hareketi tamamen durdur
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.gravityScale = 0f;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        // Collider'ları kapat
        Collider2D[] colliders = GetComponentsInChildren<Collider2D>();
        foreach (Collider2D col in colliders)
        {
            col.enabled = false;
        }

        // Mevcut pozisyonda sabit kal
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);

        Destroy(gameObject, 1f);
    }
}