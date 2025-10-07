using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class Soldier : MonoBehaviour
{
    [Header("Components")]
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sprite;
    public AudioClip attackSound, hurtSound, deathSound, coinSound, jumpSound;

    [Header("Game Stats")]
    public int currentHealth = 200;
    public int startCoins = 10;
    public int startArrows = 10;
    private int maxHealth = 200;
    private bool isArrowLeft = true;
    public bool isFinished;
    public bool isGrounded;

    [Header("Settings")]
    private float speed = 5f;
    private float jumpForce = 7f;
    private float xRange = -10f;

    [Header("Arrow System")]
    public GameObject arrowPrefab;
    public Transform firePoint;
    private List<GameObject> arrowPool = new List<GameObject>();
    private int poolSize = 10;

    private SwordAttack swordAttack;
    private int attackIndex = 0;

    private static Soldier instance;

    void Awake()
    {
        PlayerPrefs.DeleteAll(); //test amaçlı
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        swordAttack = GetComponentInChildren<SwordAttack>();

        LoadPlayerData();
        CreateArrowPool();
        ForceUIUpdate();
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

        sprite.flipX = x < 0;

        if (transform.position.x < xRange)
            transform.position = new Vector3(xRange, transform.position.y, transform.position.z);
    }

    // 🎯 BU METODU EKLE - UI GÜNCELLEME
    public void ForceUIUpdate()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateCoins(startCoins);
            UIManager.Instance.UpdateArrows(startArrows);
            UIManager.Instance.UpdateHealth((float)currentHealth / maxHealth);
            Debug.Log("ForceUIUpdate called!");
        }
        else
        {
            Debug.LogError("UIManager.Instance is null!");
        }
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
        attackIndex = (attackIndex % 2) + 1;
        anim.SetInteger("AttackIndex", attackIndex);
        anim.SetTrigger("Attack");

        if (swordAttack != null) swordAttack.DoSwordAttack();
    }

    void BowAttack()
    {
        startArrows--;
        PlayerPrefs.SetInt("Arrows", startArrows);
        PlayerPrefs.Save();

        // UI'YI HEMEN GÜNCELLE
        if (UIManager.Instance != null)
            UIManager.Instance.UpdateArrows(startArrows);

        if (startArrows <= 0) isArrowLeft = false;

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

        // UI'YI HEMEN GÜNCELLE
        if (UIManager.Instance != null)
            UIManager.Instance.UpdateArrows(startArrows);
    }

    public void GetDamage(int damage)
    {
        currentHealth -= damage;
        PlayerPrefs.SetInt("Health", currentHealth);
        PlayerPrefs.Save();

        // UI'YI HEMEN GÜNCELLE
        if (UIManager.Instance != null)
            UIManager.Instance.UpdateHealth((float)currentHealth / maxHealth);

        AudioSource.PlayClipAtPoint(hurtSound, transform.position);
        anim.SetTrigger("Hurt");

        if (currentHealth <= 0)
        {
            AudioSource.PlayClipAtPoint(deathSound, transform.position);
            anim.SetTrigger("Die");
            Invoke(nameof(Die), 1.5f);
        }
    }

    private void Die()
    {
        PlayerPrefs.DeleteKey("Health");
        PlayerPrefs.DeleteKey("Arrows");
        PlayerPrefs.DeleteKey("Coins");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Coin"))
        {
            AudioSource.PlayClipAtPoint(coinSound, transform.position);
            Destroy(other.gameObject);
            startCoins--;
            PlayerPrefs.SetInt("Coins", startCoins);
            PlayerPrefs.Save();

            // UI'YI HEMEN GÜNCELLE
            if (UIManager.Instance != null)
                UIManager.Instance.UpdateCoins(startCoins);

            if (startCoins <= 0)
            {
                isFinished = true;
                ShowCoinsCollectedText();
            }
        }

        if (other.gameObject.CompareTag("ArrowItem"))
        {
            Destroy(other.gameObject);
            AddArrowsToPool(5);
        }
    }
    private void ShowCoinsCollectedText()
    {
        if (UIManager.Instance != null && UIManager.Instance.coinsDoneText != null)
        {
            UIManager.Instance.coinsDoneText.text = "COINS COLLECTED!";
            UIManager.Instance.coinsDoneText.gameObject.SetActive(true);
            Invoke(nameof(HideCoinsCollectedText), 1f); // 1 saniye sonra gizle
        }
    }

    private void HideCoinsCollectedText()
    {
        if (UIManager.Instance != null && UIManager.Instance.coinsDoneText != null)
        {
            UIManager.Instance.coinsDoneText.gameObject.SetActive(false);
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Flag") && isFinished)
        {
            SavePlayerData();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }

        if (other.gameObject.CompareTag("Ground"))
            isGrounded = true;

        if (other.gameObject.CompareTag("Space"))
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenu")
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
            LoadPlayerData();
            ForceUIUpdate();

            transform.position = Vector3.zero;

            isFinished = false;
            isGrounded = false;
        }
        CameraFollow cam = Camera.main.GetComponent<CameraFollow>();
        if (cam != null)
            cam.target = this.transform;
    }

    private void LoadPlayerData()
    {
        currentHealth = PlayerPrefs.GetInt("Health", maxHealth);
        startArrows = PlayerPrefs.GetInt("Arrows", startArrows);
        startCoins = PlayerPrefs.GetInt("Coins", startCoins);
    }

    private void SavePlayerData()
    {
        PlayerPrefs.SetInt("Health", currentHealth);
        PlayerPrefs.SetInt("Arrows", startArrows);
        PlayerPrefs.SetInt("Coins", startCoins);
        PlayerPrefs.Save();
    }

    private void CreateArrowPool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject arrow = Instantiate(arrowPrefab);
            arrow.SetActive(false);
            arrowPool.Add(arrow);
        }
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}