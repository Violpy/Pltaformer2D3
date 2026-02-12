using UnityEngine;
using System.Collections;

public class DissaperingPlatform : MonoBehaviour
{
    [Header("Timings")]
    public float delayBeforeDisappearing = 0.5f; // Задержка перед началом исчезновения
    public float fadeDuration = 1.0f;           // Как долго длится "затухание"
    public float respawnDelay = 3.0f;           // Через сколько секунд платформа вернется

    private SpriteRenderer spriteRenderer;
    private Collider2D platformCollider;
    private Color originalColor;
    private bool isWorking = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        platformCollider = GetComponent<Collider2D>();
        originalColor = spriteRenderer.color;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Проверяем игрока и чтобы платформа уже не была в процессе исчезновения
        if (collision.gameObject.CompareTag("player") && !isWorking)
        {
            StartCoroutine(FadeRoutine());
        }
    }

    private IEnumerator FadeRoutine()
    {
        isWorking = true;

        // 1. Ждем перед исчезновением
        yield return new WaitForSeconds(delayBeforeDisappearing);

        // 2. Плавно делаем прозрачной
        float elapsedTime = 0;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        // 3. Выключаем физику и видимость
        spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0);
        platformCollider.enabled = false;

        // 4. Ждем время до восстановления
        yield return new WaitForSeconds(respawnDelay);

        // 5. Возвращаем всё назад
        spriteRenderer.color = originalColor;
        platformCollider.enabled = true;
        isWorking = false;
    }
}