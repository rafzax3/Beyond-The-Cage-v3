using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class EndingSequence : MonoBehaviour
{
    [Header("Referensi UI")]
    [SerializeField] private TextMeshProUGUI endingText;
    
    [Header("Audio")]
    [SerializeField] private AudioClip voiceLine;
    [SerializeField] private AudioSource audioSource;

    [Header("Pengaturan Waktu")]
    [SerializeField] private float delayBeforeStart = 1f;
    [SerializeField] private float textFadeInDuration = 2f;
    [SerializeField] private float textStayDuration = 4f;
    [SerializeField] private float textFadeOutDuration = 2f;
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    IEnumerator Start()
    {
        if (FadeManager.instance != null)
        {
            FadeManager.instance.fadeImage.color = Color.white; 
            FadeManager.instance.fadeImage.canvas.enabled = true; 
        }

        if (endingText != null) 
        {
            endingText.alpha = 0;
            endingText.gameObject.SetActive(true);
        }

        if (MusicManager.instance != null)
        {
            MusicManager.instance.PlayEndingMusic();
        }

        yield return new WaitForSeconds(0.5f);

        if (FadeManager.instance != null)
        {
            yield return StartCoroutine(FadeManager.instance.FadeIn(Color.white, 2f));
        }

        yield return new WaitForSeconds(delayBeforeStart);

        if (audioSource != null && voiceLine != null)
        {
            audioSource.PlayOneShot(voiceLine);
        }

        yield return StartCoroutine(FadeText(0, 1, textFadeInDuration));
        yield return new WaitForSeconds(textStayDuration);
        yield return StartCoroutine(FadeText(1, 0, textFadeOutDuration));

        if (FadeManager.instance != null)
        {
            yield return StartCoroutine(FadeManager.instance.FadeOut(Color.white, 2f));
        }

        SceneManager.LoadScene(mainMenuSceneName);
    }

    private IEnumerator FadeText(float startAlpha, float endAlpha, float duration)
    {
        if (endingText == null) yield break;

        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float currentAlpha = Mathf.Lerp(startAlpha, endAlpha, timer / duration);
            endingText.alpha = currentAlpha;
            yield return null;
        }
        endingText.alpha = endAlpha;
    }
}