using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class PlayerLightReceiver : MonoBehaviour
{
    [SerializeField] private Color ambientColor = new Color(0.5f, 0.5f, 0.5f, 1f);

    private SpriteRenderer myRenderer;
    private Coroutine colorFadeCoroutine;
    
    private bool isDarknessOverride = false;
    private Color lastTargetColor; 

    void Awake()
    {
        myRenderer = GetComponent<SpriteRenderer>();
        myRenderer.color = ambientColor;
        lastTargetColor = ambientColor;
    }

    public Color GetCurrentAmbientColor()
    {
        return ambientColor;
    }

    public void SetDarknessOverride(bool isOverride, Color darkColor)
    {
        isDarknessOverride = isOverride;

        if (isOverride)
        {
            if (colorFadeCoroutine != null) StopCoroutine(colorFadeCoroutine);
            
            myRenderer.color = darkColor;
        }
        else
        {
            if (colorFadeCoroutine != null) StopCoroutine(colorFadeCoroutine);
            colorFadeCoroutine = StartCoroutine(FadeTo(lastTargetColor, 0.1f));
        }
    }

    public void ReturnToAmbient(float duration)
    {
        SetTargetColor(ambientColor, duration, false);
    }

    public void SetTargetColor(Color newColor, float duration, bool isNewAmbient)
    {
        if (isNewAmbient)
        {
            ambientColor = newColor;
        }

        lastTargetColor = newColor;

        if (isDarknessOverride) return;

        if (colorFadeCoroutine != null)
        {
            StopCoroutine(colorFadeCoroutine);
        }

        colorFadeCoroutine = StartCoroutine(FadeTo(newColor, duration));
    }

    private IEnumerator FadeTo(Color targetColor, float duration)
    {
        if (duration <= 0f)
        {
            myRenderer.color = targetColor;
            yield break;
        }

        Color startColor = myRenderer.color;
        float timer = 0f;

        while (timer < duration)
        {
            if (isDarknessOverride) yield break;

            timer += Time.deltaTime;
            myRenderer.color = Color.Lerp(startColor, targetColor, timer / duration);
            yield return null;
        }

        if (!isDarknessOverride)
        {
            myRenderer.color = targetColor;
        }
        colorFadeCoroutine = null;
    }
}