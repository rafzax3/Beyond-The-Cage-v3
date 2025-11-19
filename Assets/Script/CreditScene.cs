using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// Ditempel di obyek di dalam "CreditsImage_Scene" baru Anda
public class CreditsScreen : MonoBehaviour
{
    [Header("Pengaturan")]
    [Tooltip("Berapa lama gambar credit ditampilkan (detik)")]
    [SerializeField] private float waitDuration = 5f;
    
    [Tooltip("Nama scene Main Menu")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    IEnumerator Start()
    {
        // 1. Fade IN (dari TheEndScreen)
        if (FadeManager.instance != null)
        {
            yield return StartCoroutine(FadeManager.instance.FadeIn());
        }

        // 2. Tunggu untuk menampilkan gambar
        yield return new WaitForSeconds(waitDuration);

        // 3. Fade OUT (ke MainMenu)
        if (FadeManager.instance != null)
        {
            yield return StartCoroutine(FadeManager.instance.FadeOut());
        }

        // 4. Pindah scene
        SceneManager.LoadScene(mainMenuSceneName);
    }
}