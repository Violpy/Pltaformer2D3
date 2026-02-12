using UnityEngine;

public class SpriteAnimator : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    
    [Header("Animation Frames")]
    public Sprite idleSprite;
    public Sprite[] walkFrames;
    public Sprite jumpSprite;

    public float frameRate = 0.1f; // Скорость смены кадров
    private float timer;
    private int currentFrame;

    public void PlayIdle()
    {
        spriteRenderer.sprite = idleSprite;
    }

    public void PlayJump()
    {
        spriteRenderer.sprite = jumpSprite;
    }

    public void PlayWalk()
    {
        timer += Time.deltaTime;
        if (timer >= frameRate)
        {
            timer = 0;
            currentFrame = (currentFrame + 1) % walkFrames.Length;
            spriteRenderer.sprite = walkFrames[currentFrame];
        }
    }
}