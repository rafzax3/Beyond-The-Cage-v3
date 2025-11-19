// File 5: EndScreen.cs
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndScreen : MonoBehaviour
{
    [Header("Pengaturan")]
    [Tooltip("Berapa lama layar 'The End' ditampilkan (detik)")]
    [SerializeField] private float waitDuration = 5f;
    [Tooltip("Nama scene Video Credit")]
    [SerializeField] private string creditsVideoSceneName = "Cutscene_Credits";
    [Tooltip("Durasi fade putih dari EndingRoom")]
    [SerializeField] private float whiteFadeDuration = 2f;

    IEnumerator Start()
    {
        // 1. Fade IN dari PUTIH (karena kita keluar dari FinalTrigger)
        if (FadeManager.instance != null)
        {
            // --- INI ADALAH BARIS YANG SUDAH DIPERBAIKI ---
            // Kita berikan WARNA (Color.white) dan DURASI (whiteFadeDuration)
            yield return StartCoroutine(FadeManager.instance.FadeIn(Color.white, whiteFadeDuration));
        }

        // 2. Tunggu
        yield return new WaitForSeconds(waitDuration);

        // 3. Fade OUT ke HITAM (untuk pindah ke video credit)
        if (FadeManager.instance != null)
        {
            yield return StartCoroutine(FadeManager.instance.FadeOut());
        }

        // 4. Pindah scene
        SceneManager.LoadScene(creditsVideoSceneName);
    }
}