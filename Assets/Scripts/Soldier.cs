using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class Soldier : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sprite;
    public AudioClip attackSound;
    public AudioClip hurtSound;
    public AudioClip deathSound;
    public AudioClip coinSound;
    public AudioClip jumpSound;

    public bool isGrounded;
    private float speed = 5f;
    private float jumpForce = 7f;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump();
        }
    }
    void Jump()
    {
        AudioSource.PlayClipAtPoint(jumpSound, transform.position);
        rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
        isGrounded = false;
    }
    private void FixedUpdate()
    {
        float x = Input.GetAxis("Horizontal");
        transform.position += new Vector3(x, 0, 0) * Time.deltaTime * speed;
        if (x < 0)
        {
            sprite.flipX = true;
        }
        else if (x > 0)
        {
            sprite.flipX = false;
        }
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Coin"))
        {
            AudioSource.PlayClipAtPoint(coinSound, transform.position);
            Destroy(other.gameObject);
        }
        if (other.gameObject.CompareTag("Flag"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        if (other.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
}
