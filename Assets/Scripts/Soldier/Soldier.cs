using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class Soldier : MonoBehaviour
{
    [Header("Components")]
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sprite;
    public AudioClip attackSound, hurtSound, deathSound, coinSound, jumpSound;

    [Header("State")]
    public bool isGrounded;
    private float speed = 5f;
    private float jumpForce = 7f;
    private float xRange = -10f;

    [Header("Gameplay")]
    [SerializeField] private int startCoins = 10;
    [SerializeField] private int startArrows = 10;
    public int currentHealth;
    private int maxHealth = 200;
    private bool isArrowLeft = true;
    public bool isFinished;

    [Header("Arrow")]
    public GameObject arrowPrefab;
    public Transform firePoint;
    private List<GameObject> arrowPool = new List<GameObject>();
    private int poolSize = 10;

    private SwordAttack swordAttack;
    private int attackIndex = 0;

    [Header("UI")]
    public GameObject coinsLeftUI;
    public GameObject coinsDoneUI;
    public GameObject arrowLeftUI;
    public GameObject healthUI;
    public GameObject backmenuUI;

    void Start()
    {
        //PlayerPrefs.DeleteAll();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        swordAttack = GetComponentInChildren<SwordAttack>();

        currentHealth = PlayerPrefs.HasKey("Health") ? PlayerPrefs.GetInt("Health") : maxHealth;
        startArrows = PlayerPrefs.HasKey("Arrows") ? PlayerPrefs.GetInt("Arrows") : startArrows;

        SetTextSafely(arrowLeftUI, "Arrows Left: " + startArrows.ToString());
        SetTextSafely(coinsLeftUI, "Coins Left: " + startCoins.ToString());
        UpdateHealthUI();

        for (int i = 0; i < poolSize; i++)
        {
            GameObject arrow = Instantiate(arrowPrefab);
            arrow.SetActive(false);
            arrowPool.Add(arrow);
        }
    }

    void Update()
    {
        if (Input.GetButtonDown("Jump") && isGrounded) Jump();
        if (Input.GetMouseButtonDown(0)) Attack();
        if (Input.GetMouseButtonDown(1) && isArrowLeft) BowAttack();
    }

    void FixedUpdate()
    {
        float x = Input.GetAxis("Horizontal");
        transform.position += new Vector3(x, 0, 0) * Time.fixedDeltaTime * speed;
        anim.SetFloat("Speed", Mathf.Abs(x));

        if (x < 0) sprite.flipX = true;
        else if (x > 0) sprite.flipX = false;

        if (transform.position.x < xRange)
            transform.position = new Vector3(xRange, transform.position.y, transform.position.z);
    }

    void Jump()
    {
        AudioSource.PlayClipAtPoint(jumpSound, transform.position);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        isGrounded = false;
    }

    void Attack()
    {
        AudioSource.PlayClipAtPoint(attackSound, transform.position);
        attackIndex++;
        if (attackIndex > 2) attackIndex = 1;

        anim.SetInteger("AttackIndex", attackIndex);
        anim.SetTrigger("Attack");

        if (swordAttack != null) swordAttack.DoSwordAttack();
    }

    void BowAttack()
    {
        startArrows--;
        PlayerPrefs.SetInt("Arrows", startArrows);
        PlayerPrefs.Save();

        if (startArrows <= 0)
        {
            isArrowLeft = false;
            SetTextSafely(arrowLeftUI, "NONE");
        }
        else
        {
            SetTextSafely(arrowLeftUI, "Arrows Left: " + startArrows.ToString());
        }

        AudioSource.PlayClipAtPoint(attackSound, transform.position);
        anim.SetTrigger("BowAttack");

        GameObject arrowObj = GetArrowFromPool();
        if (arrowObj != null)
        {
            arrowObj.transform.position = firePoint.position;
            arrowObj.SetActive(true);

            Arrow arrow = arrowObj.GetComponent<Arrow>();
            Vector2 shootDir = sprite.flipX ? Vector2.left : Vector2.right;
            arrow.Initialize(shootDir);
        }
    }

    GameObject GetArrowFromPool()
    {
        foreach (GameObject arrow in arrowPool)
        {
            if (arrow != null && !arrow.activeInHierarchy)
                return arrow;
        }
        return null;
    }

    public void AddArrowsToPool(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            GameObject arrow = Instantiate(arrowPrefab);
            arrow.SetActive(false);
            arrowPool.Add(arrow);
        }

        startArrows += amount;
        isArrowLeft = true;
        PlayerPrefs.SetInt("Arrows", startArrows);
        PlayerPrefs.Save();

        SetTextSafely(arrowLeftUI, "Arrows Left: " + startArrows.ToString());
    }

    public void GetDamage(int damage)
    {
        currentHealth -= damage;
        PlayerPrefs.SetInt("Health", currentHealth);
        PlayerPrefs.Save();

        UpdateHealthUI();
        AudioSource.PlayClipAtPoint(hurtSound, transform.position);
        anim.SetTrigger("Hurt");

        if (currentHealth <= 0)
        {
            AudioSource.PlayClipAtPoint(deathSound, transform.position);
            anim.SetTrigger("Die");
            Invoke(nameof(Die), 1.5f);
        }
    }

    public void UpdateHealthUI()
    {
        float healthPercent = (float)currentHealth / maxHealth;
        healthUI.GetComponent<UnityEngine.UI.Image>().fillAmount = healthPercent;
    }

    private void Die()
    {
        PlayerPrefs.DeleteKey("Health");
        PlayerPrefs.DeleteKey("Arrows");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Coin"))
        {
            AudioSource.PlayClipAtPoint(coinSound, transform.position);
            Destroy(other.gameObject);
            startCoins--;
            SetTextSafely(coinsLeftUI, "Coins Left: " + startCoins.ToString());

            if (startCoins <= 0)
            {
                isFinished = true;
                coinsDoneUI.SetActive(true);
                Invoke(nameof(CoinsDone), 1f);
            }
        }

        if (other.gameObject.CompareTag("ArrowItem"))
        {
            Destroy(other.gameObject);
            AddArrowsToPool(5);
        }
    }
    void CoinsDone()
    {
        coinsDoneUI.SetActive(false);
    }
    public void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    private void SetTextSafely(GameObject target, string text)
    {
        if (target == null) return;

        var tmp = target.GetComponent<TMP_Text>();
        if (tmp != null)
        {
            tmp.text = text;
            return;
        }

        var uiText = target.GetComponent<Text>();
        if (uiText != null)
        {
            uiText.text = text;
            return;
        }

        tmp = target.GetComponentInChildren<TMP_Text>();
        if (tmp != null)
        {
            tmp.text = text;
            return;
        }

        uiText = target.GetComponentInChildren<Text>();
        if (uiText != null)
        {
            uiText.text = text;
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Flag") && isFinished)
        {
            PlayerPrefs.SetInt("Health", currentHealth);
            PlayerPrefs.SetInt("Arrows", startArrows);
            PlayerPrefs.Save();

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }

        if (other.gameObject.CompareTag("Ground"))
            isGrounded = true;
    }
}
