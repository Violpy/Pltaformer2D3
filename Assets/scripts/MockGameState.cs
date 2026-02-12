using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MockGameState : MonoBehaviour
{
    [Header("HUD Elements")]
    public TextMeshProUGUI moonCounterText; 
    public Image[] heartImages;
    
    [Header("Heart Sprites")]
    public Sprite redHeartSprite;  
    public Sprite blackHeartSprite;
    
    private int moons = 0;
    private int hp;

    void Start()
    {
        hp = heartImages.Length;
        UpdateHUD();
    }

    public void AddMoon()
    {
        if(moons < 3)
        {
        moons++;
        UpdateHUD();
        SceneController.instance.PlaySFX(SceneController.instance.collectSound);
        if(moons >= 3)
        {
            Invoke("TriggerWIN", 0.8f); 
        }
        if (moons >= 3)
        {
            SceneController.instance.PlaySFX(SceneController.instance.winSound);        }
        }
    }
    public void ResetMoons()
{
    moons = 0;
    UpdateHUD();
}
    public void DecreaseHP()
    {
        if (hp > 0)
        {
            hp--;
            heartImages[hp].sprite = blackHeartSprite; 
            
            if (hp <= 0)
            {
                Invoke("TriggerGameOver", 0.5f);
            }
        }
    }

    
    public void TriggerGameOver()
    {
        SceneController.instance.LoadSpecificScene("GameOver");
    }
    public void IncreaseHP()
    {
        if (hp < heartImages.Length)
        {
            heartImages[hp].sprite = redHeartSprite; 
            hp++;
        }
    }
    public void TriggerWIN()
    {
        SceneController.instance.LoadSpecificScene("WIN");  
    }
    public void UpdateHUD()
    {
        moonCounterText.text = $"{moons}/3"; 
    }
}