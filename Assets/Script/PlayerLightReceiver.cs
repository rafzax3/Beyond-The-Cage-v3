using UnityEngine;
using System.Collections;

// DITEMPEL DI PREFAB PLAYER
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerLightReceiver : MonoBehaviour
{
    [Tooltip("Warna 'ambient' atau default player di ruangan gelap")]
    [SerializeField] private Color ambientColor = new Color(0.5f, 0.5f, 0.5f, 1f); // Abu-abu gelap

    private SpriteRenderer myRenderer;
    private Coroutine colorFadeCoroutine;

    void Awake()
    {
        myRenderer = GetComponent<SpriteRenderer>();
        // Set warna awal ke warna ambient
        myRenderer.color = ambientColor;
    }

    // Dipanggil oleh LightingZone saat player KELUAR dari zona terang
    public void ReturnToAmbient(float duration)
    {
        SetTargetColor(ambientColor, duration, false);
    }

    // Dipanggil oleh LightingZone saat player MASUK ke zona
    public void SetTargetColor(Color newColor, float duration, bool isNewAmbient)
    {
        // Jika ini adalah zona ambient baru (misal: masuk ruangan baru),
        // simpan sebagai warna default baru.
        if (isNewAmbient)
        {
            ambientColor = newColor;
        }

        // Hentikan coroutine yang sedang berjalan agar tidak "berkelahi"
        if (colorFadeCoroutine != null)
        {
            StopCoroutine(colorFadeCoroutine);
        }

        // Mulai coroutine baru untuk mengubah warna
        colorFadeCoroutine = StartCoroutine(FadeTo(newColor, duration));
    }

    private IEnumerator FadeTo(Color targetColor, float duration)
    {
        Color startColor = myRenderer.color;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            // Ubah warna secara bertahap
            myRenderer.color = Color.Lerp(startColor, targetColor, timer / duration);
            yield return null;
        }

        // Pastikan warna akhir akurat
        myRenderer.color = targetColor;
        colorFadeCoroutine = null;
    }
}