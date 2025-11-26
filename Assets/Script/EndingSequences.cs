using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

// DITEMPEL DI SCENE 'EndingTextScene'
public class EndingSequence : MonoBehaviour
{
    [System.Serializable]
    public struct EndingLine
    {
        [TextArea(3, 5)] public string textContent;
        [Tooltip("Berapa lama teks ini muncul di layar (detik)")]
        public float displayDuration;
    }

    [Header("Referensi UI")]
    [SerializeField] private TextMeshProUGUI endingText;
    
    [Header("Audio")]
    [Tooltip("Voice Line Narasi")]
    [SerializeField] private AudioClip voiceLine;
    [SerializeField] private AudioSource audioSource;

    [Header("Pengaturan Waktu")]
    [SerializeField] private float delayBeforeStart = 1f;
    [Tooltip("Jeda waktu setelah suara mulai sebelum teks pertama muncul")]
    [SerializeField] private float delayBetweenVoiceAndText = 2f; // <-- FITUR BARU
    [SerializeField] private float fadeDuration = 1.5f; 
    [SerializeField] private float delayBetweenLines = 0.5f;

    [Header("Daftar Teks")]
    [SerializeField] private List<EndingLine> endingLines; 
    
    [Header("Scene Selanjutnya")]
    [SerializeField] private string nextSceneName = "EndingCutscene";

    IEnumerator Start()
    {
        // 1. Setup Awal (Layar Putih)
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

        // 2. Fade In Layar (Putih -> Bening)
        if (FadeManager.instance != null)
        {
            yield return StartCoroutine(FadeManager.instance.FadeIn(Color.white, 2f));
        }

        yield return new WaitForSeconds(delayBeforeStart);

        // 3. Mulai Voice Line
        if (audioSource != null && voiceLine != null)
        {
            // Kita assign clip agar bisa cek .isPlaying nanti
            audioSource.clip = voiceLine;
            audioSource.Play();
        }

        // --- JEDA ANTARA SUARA DAN TEKS ---
        yield return new WaitForSeconds(delayBetweenVoiceAndText);
        // ----------------------------------

        // 4. Loop Teks Bergantian
        foreach (EndingLine line in endingLines)
        {
            if (endingText != null) endingText.text = line.textContent;

            // Fade In Teks
            yield return StartCoroutine(FadeText(0, 1, fadeDuration));

            // Tunggu baca
            yield return new WaitForSeconds(line.displayDuration);

            // Fade Out Teks
            yield return StartCoroutine(FadeText(1, 0, fadeDuration));

            // Jeda antar baris
            yield return new WaitForSeconds(delayBetweenLines);
        }

        // --- TUNGGU SUARA SELESAI ---
        // Jika teks sudah habis tapi suara masih ada, tunggu sampai suara selesai
        if (audioSource != null && audioSource.isPlaying)
        {
            while (audioSource.isPlaying)
            {
                yield return null;
            }
        }
        // ---------------------------

        // 5. Selesai - Fade Out Layar & Pindah
        if (FadeManager.instance != null)
        {
            yield return StartCoroutine(FadeManager.instance.FadeOut(Color.white, 1f));
        }

        SceneManager.LoadScene(nextSceneName);
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