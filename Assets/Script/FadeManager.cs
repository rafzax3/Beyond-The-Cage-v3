using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeManager : MonoBehaviour
{
    public static FadeManager instance;
    public Image fadeImage;
    private float defaultFadeTime = 0.5f;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public IEnumerator FadeOut()
    {
        yield return StartCoroutine(FadeOut(Color.black, defaultFadeTime));
    }

    public IEnumerator FadeOut(Color fadeToColor)
    {
        yield return StartCoroutine(FadeOut(fadeToColor, defaultFadeTime));
    }

    public IEnumerator FadeOut(Color fadeToColor, float duration)
    {
        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float alpha = timer / duration;
            fadeImage.color = new Color(fadeToColor.r, fadeToColor.g, fadeToColor.b, alpha);
            yield return null;
        }
        fadeImage.color = fadeToColor;
    }

    public IEnumerator FadeIn()
    {
        yield return StartCoroutine(FadeIn(Color.black, defaultFadeTime));
    }

    public IEnumerator FadeIn(Color fadeFromColor)
    {
        yield return StartCoroutine(FadeIn(fadeFromColor, defaultFadeTime));
    }

    public IEnumerator FadeIn(Color fadeFromColor, float duration)
    {
        fadeFromColor.a = 1f;

        float timer = duration;
        while (timer > 0f)
        {
            timer -= Time.deltaTime;

            float alpha = timer / duration;

            fadeImage.color = new Color(fadeFromColor.r, fadeFromColor.g, fadeFromColor.b, alpha);
            yield return null;
        }

        fadeImage.color = Color.clear;
    }
}