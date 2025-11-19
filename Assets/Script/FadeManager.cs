using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// Versi Diperbarui: Sekarang FadeIn() sama pintarnya dengan FadeOut()
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

    // --- FADE OUT (Tidak Berubah) ---

    // Versi 1: Panggil ini jika Anda tidak peduli (default ke HITAM)
    public IEnumerator FadeOut()
    {
        yield return StartCoroutine(FadeOut(Color.black, defaultFadeTime));
    }

    // Versi 2: Panggil ini jika Anda ingin fade ke warna TERTENTU (default time)
    public IEnumerator FadeOut(Color fadeToColor)
    {
        yield return StartCoroutine(FadeOut(fadeToColor, defaultFadeTime));
    }

    // Versi 3: Fungsi utama (ke WARNA, durasi KUSTOM)
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

    // --- FADE IN (Baru & Diperbarui) ---

    // Versi 1: Panggil ini jika Anda tidak peduli (default dari HITAM)
    public IEnumerator FadeIn()
    {
        yield return StartCoroutine(FadeIn(Color.black, defaultFadeTime));
    }

    // Versi 2: Panggil ini jika Anda ingin fade dari warna TERTENTU (default time)
    public IEnumerator FadeIn(Color fadeFromColor)
    {
        yield return StartCoroutine(FadeIn(fadeFromColor, defaultFadeTime));
    }

    // Versi 3: Fungsi utama (dari WARNA, durasi KUSTOM)
    public IEnumerator FadeIn(Color fadeFromColor, float duration)
    {
        // Pastikan warna awal adalah opaque (penuh)
        fadeFromColor.a = 1f;

        float timer = duration;
        while (timer > 0f)
        {
            timer -= Time.deltaTime;

            // Hitung alpha
            float alpha = timer / duration;

            // Atur warna dengan alpha baru
            fadeImage.color = new Color(fadeFromColor.r, fadeFromColor.g, fadeFromColor.b, alpha);
            yield return null;
        }

        // Selesai: transparan penuh
        fadeImage.color = Color.clear;
    }
}