using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class UIButtonAnimator : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    public enum ButtonType { First, Second, Third, Fourth, None }
    
    [Header("Button Role")]
    public ButtonType role = ButtonType.None;
    public bool isSpecialButton;
    

    [Header("Sprites")]
    public bool changeSprite;
    public Sprite normalSprite;
    public Sprite hoverSprite;
    public Sprite clickSprite;

    private Image buttonImage;
    private TextMeshProUGUI buttonText;
    private Vector3 originalScale;
    private Color normalTextColor;

    void Start()
    {
        buttonImage = GetComponent<Image>();
        buttonText = GetComponentInChildren<TextMeshProUGUI>();
        originalScale = transform.localScale;

        SetInitialColor();
        if (buttonImage != null && normalSprite != null) buttonImage.sprite = normalSprite;
    }

    void SetInitialColor()
    {
        if (buttonText == null) return;

        if (role == ButtonType.None || role == ButtonType.Fourth) 
        { 
            normalTextColor = buttonText.color; 
            return; 
        }

        string hexColor = "";
        switch (role)
        {
            case ButtonType.First: hexColor = "#C5BC00"; break;  
            case ButtonType.Second: hexColor = "#A9A100"; break; 
            case ButtonType.Third: hexColor = "#97910B"; break; 
            case ButtonType.Fourth : hexColor = default; break;
        }

        if (ColorUtility.TryParseHtmlString(hexColor, out Color newCol))
        {
            normalTextColor = newCol;
            buttonText.color = normalTextColor;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isSpecialButton)
        {
            transform.localScale = originalScale * 1.1f;
        }
        else
        {
            if (changeSprite && hoverSprite != null) buttonImage.sprite = hoverSprite;
            
            if (role != ButtonType.Fourth && buttonText != null) {
            ColorUtility.TryParseHtmlString("#FFF300", out Color hoverColor);
            buttonText.color = hoverColor;
        }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (isSpecialButton)
        {
            transform.localScale = originalScale;
        }
        else
        {
            if (changeSprite && clickSprite != null) buttonImage.sprite = clickSprite;

            Color clickColor;
            ColorUtility.TryParseHtmlString("#B656B7", out clickColor);
            if (buttonText != null) buttonText.color = clickColor;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = originalScale;
        if (changeSprite) buttonImage.sprite = normalSprite;
        if (buttonText != null) buttonText.color = normalTextColor;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!isSpecialButton && changeSprite) buttonImage.sprite = normalSprite;
    }
}