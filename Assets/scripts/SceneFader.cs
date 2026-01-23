using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SceneFader : MonoBehaviour
{
    public Image fadeOverlay; 
    public float fadeDuration = 0.8f;
    public Color fadeColor = Color.black; 

    void Start()
    {
        if (fadeOverlay != null)
        {
            fadeOverlay.gameObject.SetActive(true);
            StartCoroutine(Fade(1, 0));
        }
    }

    public IEnumerator Fade(float startAlpha, float endAlpha)
    {
        float timer = 0;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, timer / fadeDuration);
            
            fadeOverlay.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, alpha);
            yield return null;
        }

        if (endAlpha <= 0) fadeOverlay.gameObject.SetActive(false);
    }
}