using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class Soldier : MonoBehaviour
{
    [Header("Components")]
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sprite;
    public AudioClip attackSound;
    public AudioClip hurtSound;
    public AudioClip deathSound;
    public AudioClip coinSound;
    public AudioClip jumpSound;

    [Header("State")]
    public bool isGrounded;
    private float speed = 5f;
    private float jumpForce = 7f;
    private float xRange = -10f;

    [Header("UI")]
    public TextMeshProUGUI leftCoinsText;
    public TextMeshProUGUI coinsDoneText;
    [SerializeField] private int startCoins = 10;

    int attackIndex = 0;

    public GameObject arrowPrefab;
    public Transform firePoint;
    public bool isFinished;

    [Header("Attack and Health")]
    public Image healthBar;
    private int maxHealth = 200;
    public int currentHealth;

    private SwordAttack swordAttack;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();

        swordAttack = GetComponentInChildren<SwordAttack>();
    }

    void Update()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump();
        }
        if (Input.GetMouseButtonDown(0))
        {
            Attack();
        }
        if (Input.GetMouseButtonDown(1))
        {
            BowAttack();
        }
    }

    void Jump()
    {
        AudioSource.PlayClipAtPoint(jumpSound, transform.position);
        rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
        isGrounded = false;
    }

    void Attack()
    {
        AudioSource.PlayClipAtPoint(attackSound, transform.position);
        attackIndex++;
        if (attackIndex > 2) attackIndex = 1;

        anim.SetInteger("AttackIndex", attackIndex);
        anim.SetTrigger("Attack");

        if (swordAttack != null)
        {
            swordAttack.DoSwordAttack();
        }
    }
    void BowAttack()
    {
        AudioSource.PlayClipAtPoint(attackSound, transform.position);
        anim.SetTrigger("BowAttack");

        GameObject arrowObj = Instantiate(arrowPrefab, firePoint.position, Quaternion.identity);
        Arrow arrow = arrowObj.GetComponent<Arrow>();

        Vector2 shootDir = sprite.flipX ? Vector2.left : Vector2.right;
        arrow.Initialize(shootDir);
    }




    public void GetDamage(int damage)
    {
        currentHealth -= damage;
        HealthBar();
        AudioSource.PlayClipAtPoint(hurtSound, transform.position);
        anim.SetTrigger("Hurt");
        if (currentHealth <= 0)
        {
            AudioSource.PlayClipAtPoint(deathSound, transform.position);
            anim.SetTrigger("Die");
            Invoke("Die", 1.5f);
        }
    }

    void HealthBar()
    {
        healthBar.fillAmount = (float)currentHealth / maxHealth;
    }

    void Die()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }



    private void FixedUpdate()
    {
        float x = Input.GetAxis("Horizontal");
        transform.position += new Vector3(x, 0, 0) * Time.deltaTime * speed;
        anim.SetFloat("Speed", Mathf.Abs(x));

        if (x < 0) sprite.flipX = true;
        else if (x > 0) sprite.flipX = false;

        if (transform.position.x < xRange)
        {
            transform.position = new Vector3(xRange, transform.position.y, transform.position.z);
        }
    }



    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Coin"))
        {
            AudioSource.PlayClipAtPoint(coinSound, transform.position);
            Destroy(other.gameObject);
            startCoins--;
            leftCoinsText.text = "Coins Left:" + startCoins.ToString();
            if (startCoins <= 0)
            {
                coinsDoneText.gameObject.SetActive(true);
                isFinished = true;
                leftCoinsText.gameObject.SetActive(false);
                Invoke("TextClean", 2f);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Flag") && isFinished)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        if (other.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    void TextClean()
    {
        coinsDoneText.gameObject.SetActive(false);
    }
}
