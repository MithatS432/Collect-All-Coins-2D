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
    public int startCoins = 4;
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
    private int attackIndex = 0;

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

        if (rb == null) Debug.LogError("Soldier: Rigidbody2D missing.");
        if (anim == null) Debug.LogWarning("Soldier: Animator missing.");
        if (sprite == null) Debug.LogError("Soldier: SpriteRenderer missing.");
        if (swordAttack == null) Debug.LogWarning("Soldier: SwordAttack missing.");
        if (arrowPrefab == null) Debug.LogWarning("Soldier: arrowPrefab missing.");
        if (firePoint == null) Debug.LogWarning("Soldier: firePoint missing.");

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

        if (x != 0)
            sprite.flipX = x < 0;

        if (transform.position.x < xRange)
            transform.position = new Vector3(xRange, transform.position.y, transform.position.z);
    }

    private void Jump()
    {
        if (!isGrounded) return;
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        isGrounded = false;
        if (jumpSound != null)
            AudioSource.PlayClipAtPoint(jumpSound, transform.position);
    }

    private void Attack()
    {
        attackIndex++;
        if (anim != null)
        {
            switch (attackIndex)
            {
                case 1: anim.SetTrigger("AttackNormal1"); break;
                case 2: anim.SetTrigger("AttackNormal2"); break;
            }
        }

        if (swordAttack != null)
            swordAttack.StartAttack();

        if (attackIndex > 2) attackIndex = 0;

        if (attackSound != null)
            AudioSource.PlayClipAtPoint(attackSound, transform.position);
    }

    private void BowAttack()
    {
        if (startArrows <= 0) return;

        startArrows--;
        if (UIManager.Instance != null)
            UIManager.Instance.UpdateArrows(startArrows);

        if (startArrows <= 0) isArrowLeft = false;

        if (anim != null) anim.SetTrigger("BowAttack");
        if (attackSound != null)
            AudioSource.PlayClipAtPoint(attackSound, transform.position);

        if (arrowPrefab == null || firePoint == null) return;

        GameObject arrowObj = GetArrowFromPool();
        if (arrowObj != null)
        {
            arrowObj.transform.position = firePoint.position;
            arrowObj.SetActive(true);

            Arrow arrow = arrowObj.GetComponent<Arrow>();
            if (arrow != null)
            {
                Vector2 shootDir = sprite.flipX ? Vector2.left : Vector2.right;
                arrow.Initialize(shootDir);
            }
        }

        // Ensure the arrow doesn't inherit vertical velocity from the player/jump
        if (arrowObj != null)
        {
            // reset rotation so arrow faces horizontally
            arrowObj.transform.rotation = Quaternion.identity;

            Rigidbody2D aRb = arrowObj.GetComponent<Rigidbody2D>();
            if (aRb != null)
            {
                // zero vertical velocity and angular velocity
                aRb.linearVelocity = new Vector2(aRb.linearVelocity.x, 0f);
                aRb.angularVelocity = 0f;
            }
        }
    }

    private GameObject GetArrowFromPool()
    {
        for (int i = arrowPool.Count - 1; i >= 0; i--)
            if (arrowPool[i] == null) arrowPool.RemoveAt(i);

        foreach (GameObject arrow in arrowPool)
            if (arrow != null && !arrow.activeInHierarchy)
                return arrow;

        return null;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Coin"))
        {
            if (coinSound != null)
                AudioSource.PlayClipAtPoint(coinSound, transform.position);
            Destroy(other.gameObject);
            startCoins--;
            if (UIManager.Instance != null)
                UIManager.Instance.UpdateCoins(startCoins);
            if (startCoins <= 0) isFinished = true;
        }

        if (other.CompareTag("Ground")) isGrounded = true;

        if (other.CompareTag("Flag") && isFinished)
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

        if (other.CompareTag("ArrowItem"))
        {
            Destroy(other.gameObject);
            startArrows += 5;
            if (UIManager.Instance != null)
                UIManager.Instance.UpdateArrows(startArrows);
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
            startCoins = 4;
            ForceUIUpdate();
            transform.position = Vector3.zero;
        }

        CameraFollow cam = Camera.main?.GetComponent<CameraFollow>();
        if (cam != null) cam.target = transform;
    }

    public void GetDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;
        if (UIManager.Instance != null)
            UIManager.Instance.UpdateHealth((float)currentHealth / maxHealth);

        if (hurtSound != null)
            AudioSource.PlayClipAtPoint(hurtSound, transform.position);

        if (currentHealth <= 0) Die();
        else if (anim != null) anim.SetTrigger("Hit");
    }

    private void Die()
    {
        if (deathSound != null)
            AudioSource.PlayClipAtPoint(deathSound, transform.position);
        if (anim != null) anim.SetTrigger("Die");

        rb.simulated = false;
        this.enabled = false;
        Invoke(nameof(ReturnToMainMenu), 2f);
    }

    private void ReturnToMainMenu() => SceneManager.LoadScene("MainMenu");

    private void LoadPlayerData()
    {
        currentHealth = PlayerPrefs.GetInt("Health", maxHealth);
        startArrows = PlayerPrefs.GetInt("Arrows", startArrows);
    }

    private void CreateArrowPool()
    {
        if (arrowPrefab == null) return;

        GameObject poolParent = new GameObject("ArrowPool");
        poolParent.transform.SetParent(transform.root, true);

        for (int i = 0; i < poolSize; i++)
        {
            GameObject arrow = Instantiate(arrowPrefab, poolParent.transform);
            arrow.SetActive(false);
            arrowPool.Add(arrow);
        }
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

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
