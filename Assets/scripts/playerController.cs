using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public int maxJumps = 2; // Максимальное количество прыжков (2 для двойного)
    private int jumpCount;   // Текущее количество прыжков
    
    [Header("Combat & Health")]
    public float knockbackForce = 7f;
    public float invulnerabilityDuration = 1f;
    
    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isInvulnerable = false;
    private Vector2 lastCheckpointPosition;
    private List<GameObject> collectedMoons = new List<GameObject>(); // Список для возврата лун
    
    public MockGameState gameState; // Сделай PUBLIC и перетащи Canvas сюда в инспекторе!
    private SpriteAnimator spriteAnim;

    [Header("Effects")]
    public ParticleSystem jumpParticles;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteAnim = GetComponent<SpriteAnimator>();
        
        // Автопоиск, если забыли перетащить вручную
        if (gameState == null) gameState = Object.FindFirstObjectByType<MockGameState>();
        
        lastCheckpointPosition = transform.position;
    }

    void Update()
    {
        float moveInput = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        // Логика анимации
        if (!isGrounded) spriteAnim.PlayJump();
        else if (Mathf.Abs(moveInput) > 0.1f) spriteAnim.PlayWalk();
        else spriteAnim.PlayIdle();

        // Поворот
        if (moveInput > 0) transform.localScale = new Vector3(1, 1, 1);
        else if (moveInput < 0) transform.localScale = new Vector3(-1, 1, 1);
        
        // --- ЛОГИКА ДВОЙНОГО ПРЫЖКА ---
        if (isGrounded)
        {
            jumpCount = 0; // Сбрасываем счетчик, когда мы на земле
        }

        if (Input.GetKeyDown(KeyCode.Space) && (isGrounded || jumpCount < maxJumps - 1))
        {
            Jump();
        }
    }

    void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0); // Обнуляем Y-скорость для четкого второго прыжка
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        jumpCount++;
        // Запуск частиц
        if (jumpParticles != null)
        {
            jumpParticles.Play();
        }
        SceneController.instance.PlaySFX(SceneController.instance.jumpSound);
    }

    public void UpdateCheckpoint(Vector2 newPos) => lastCheckpointPosition = newPos;

    public void Respawn()
    {
        transform.position = lastCheckpointPosition;
        rb.linearVelocity = Vector2.zero;

        // Возвращаем луны на места
        foreach (GameObject moon in collectedMoons)
        {
            if (moon != null) moon.SetActive(true);
        }
        collectedMoons.Clear();

        if (gameState != null) gameState.ResetMoons();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground")) isGrounded = true;
        
        if ((collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Trap")) && !isInvulnerable)
        {
            TakeDamage(collision.transform.position);
        }
    }

    private void TakeDamage(Vector2 hazardPosition)
    {
        if (gameState != null) gameState.DecreaseHP(); // Связь с Canvas
        SceneController.instance.PlaySFX(SceneController.instance.deathSound);

        Vector2 knockbackDir = (transform.position - (Vector3)hazardPosition).normalized;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(new Vector2(knockbackDir.x, 0.5f) * knockbackForce, ForceMode2D.Impulse);

        StartCoroutine(BecomeInvulnerable());
    }

    private IEnumerator BecomeInvulnerable()
    {
        isInvulnerable = true;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        for (float i = 0; i < invulnerabilityDuration; i += 0.1f)
        {
            sr.enabled = !sr.enabled;
            yield return new WaitForSeconds(0.1f);
        }
        sr.enabled = true;
        isInvulnerable = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Moon"))
        {
            if (gameState != null)
            {
                gameState.AddMoon();
                collectedMoons.Add(other.gameObject); // Запоминаем для возврата
                other.gameObject.SetActive(false);
            }
        }
        if (other.CompareTag("KillZone"))
        {
            SceneController.instance.PlaySFX(SceneController.instance.deathSound);
            gameState.DecreaseHP();
            Respawn();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground")) isGrounded = false;
    }
}