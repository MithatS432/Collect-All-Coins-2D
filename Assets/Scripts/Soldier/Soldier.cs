using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class Soldier : MonoBehaviour
{
    [Header("Components")]
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sprite;
    private SwordAttack swordAttack;

    [Header("Audio Clips")]
    public AudioClip attackSound, hurtSound, deathSound, coinSound, jumpSound;

    [Header("Game Stats")]
    public int currentHealth = 200;
    private int maxHealth = 200;
    public int startCoins = 4;   // Her bölümde sabit 4 coin
    public int startArrows = 10;
    public bool isFinished;
    public bool isGrounded;

    [Header("Movement Settings")]
    private float speed = 5f;
    private float jumpForce = 7f;
    private float xRange = -10f;

    [Header("Arrow System")]
    public GameObject arrowPrefab;
    public Transform firePoint;
    private List<GameObject> arrowPool = new List<GameObject>();
    private int poolSize = 10;
    private bool isArrowLeft = true;

    private static Soldier instance;

    private void Awake()
    {
        PlayerPrefs.DeleteAll();
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

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        swordAttack = GetComponentInChildren<SwordAttack>();

        // Basic runtime checks to help diagnose missing Inspector assignments
        if (rb == null) Debug.LogError("Soldier: Rigidbody2D component missing on " + gameObject.name);
        if (anim == null) Debug.LogWarning("Soldier: Animator component missing on " + gameObject.name + ". Arrow/Hit/Die animations won't play.");
        if (sprite == null) Debug.LogError("Soldier: SpriteRenderer component missing on " + gameObject.name);
        if (swordAttack == null) Debug.LogWarning("Soldier: SwordAttack child component not found. Melee attacks won't work.");
        if (arrowPrefab == null) Debug.LogWarning("Soldier: arrowPrefab not assigned in Inspector. BowAttack will not spawn arrows.");
        if (firePoint == null) Debug.LogWarning("Soldier: firePoint not assigned in Inspector. Arrows won't have a spawn position.");

        LoadPlayerData();
        CreateArrowPool();
        ForceUIUpdate();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Jump") && isGrounded) Jump();
        if (Input.GetMouseButtonDown(0)) Attack();
        if (Input.GetMouseButtonDown(1) && isArrowLeft) BowAttack();
    }

    private void FixedUpdate()
    {
        float x = Input.GetAxis("Horizontal");
        transform.position += new Vector3(x, 0, 0) * Time.fixedDeltaTime * speed;
        anim.SetFloat("Speed", Mathf.Abs(x));
        sprite.flipX = x < 0;

        if (transform.position.x < xRange)
            transform.position = new Vector3(xRange, transform.position.y, transform.position.z);
    }

    public void ForceUIUpdate()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateCoins(startCoins);
            UIManager.Instance.UpdateArrows(startArrows);
            UIManager.Instance.UpdateHealth((float)currentHealth / maxHealth);
        }
    }

    private void Jump()
    {
        AudioSource.PlayClipAtPoint(jumpSound, transform.position);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        isGrounded = false;
    }

    private void Attack()
    {
        AudioSource.PlayClipAtPoint(attackSound, transform.position);
        if (anim != null)
            anim.SetTrigger("Attack");
        else
            Debug.Log("Soldier: Attack trigger skipped because Animator is null");
        if (swordAttack != null) swordAttack.DoSwordAttack();
    }

    private void BowAttack()
    {
        startArrows--;
        if (UIManager.Instance != null)
            UIManager.Instance.UpdateArrows(startArrows);

        if (startArrows <= 0) isArrowLeft = false;

        AudioSource.PlayClipAtPoint(attackSound, transform.position);
        if (anim != null)
            anim.SetTrigger("BowAttack");
        else
            Debug.Log("Soldier: BowAttack trigger skipped because Animator is null");

        if (arrowPrefab == null || firePoint == null)
        {
            Debug.LogWarning("Soldier: Cannot spawn arrow because arrowPrefab or firePoint is not assigned.");
            return;
        }

        GameObject arrowObj = GetArrowFromPool();
        if (arrowObj != null)
        {
            arrowObj.transform.position = firePoint.position;
            arrowObj.SetActive(true);

            Arrow arrow = arrowObj.GetComponent<Arrow>();
            if (arrow != null)
            {
                Vector2 shootDir = sprite != null && sprite.flipX ? Vector2.left : Vector2.right;
                arrow.Initialize(shootDir);
            }
            else
            {
                Debug.LogWarning("Soldier: Pooled arrow doesn't have an Arrow component.");
            }
        }
        else
        {
            Debug.Log("Soldier: No available arrows in pool to fire.");
        }
    }

    private GameObject GetArrowFromPool()
    {
        foreach (GameObject arrow in arrowPool)
            if (!arrow.activeInHierarchy) return arrow;
        return null;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Coin"))
        {
            AudioSource.PlayClipAtPoint(coinSound, transform.position);
            Destroy(other.gameObject);
            startCoins--;
            if (UIManager.Instance != null)
                UIManager.Instance.UpdateCoins(startCoins);

            if (startCoins <= 0)
            {
                isFinished = true;
            }
        }

        if (other.CompareTag("Ground")) isGrounded = true;

        if (other.CompareTag("Flag") && isFinished)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
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
            startCoins = 4; // her sahnede sıfırdan 4 coin
            ForceUIUpdate();
            transform.position = Vector3.zero;
        }

        CameraFollow cam = Camera.main?.GetComponent<CameraFollow>();
        if (cam != null)
            cam.target = transform;
    }
    public void GetDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;

        // UI güncelle
        if (UIManager.Instance != null)
            UIManager.Instance.UpdateHealth((float)currentHealth / maxHealth);

        // Ses efekti çal
        if (hurtSound != null)
            AudioSource.PlayClipAtPoint(hurtSound, transform.position);
        else
            Debug.Log("Soldier: hurtSound not set (optional)");

        // Eğer can sıfırsa öl
        if (currentHealth <= 0)
            Die();
        else
        {
            if (anim != null)
                anim.SetTrigger("Hit");
            else
                Debug.Log("Soldier: Hit trigger skipped because Animator is null");
        }
    }

    private void Die()
    {
        // Ölüm sesi
        if (deathSound != null)
            AudioSource.PlayClipAtPoint(deathSound, transform.position);

        // Ölüm animasyonu
        if (anim != null)
            anim.SetTrigger("Die");

        // Oyuncuyu devre dışı bırak
        rb.simulated = false;
        this.enabled = false;

        // 2 saniye sonra ana menüye dön
        Invoke(nameof(ReturnToMainMenu), 2f);
    }

    private void ReturnToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }


    private void LoadPlayerData()
    {
        currentHealth = PlayerPrefs.GetInt("Health", maxHealth);
        startArrows = PlayerPrefs.GetInt("Arrows", startArrows);
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

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
