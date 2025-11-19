// File 4: SkippableVideoPlayer.cs (VERSI TANPA FADE-IN)
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

[RequireComponent(typeof(VideoPlayer))]
public class SkippableVideoPlayer : MonoBehaviour
{
    [Header("Referensi")]
    [Tooltip("Seret (drag) UI Text 'Tekan E untuk Skip' ke sini")]
    [SerializeField] private GameObject skipPromptUI;

    [Header("Pengaturan")]
    [Tooltip("Nama scene yang akan dimuat setelah video")]
    [SerializeField] private string nextSceneName;
    [Tooltip("Tunda skip prompt selama (detik)")]
    [SerializeField] private float skipPromptDelay = 3f;

    private VideoPlayer videoPlayer;
    private bool isSkipping = false;

    void Awake()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        videoPlayer.loopPointReached += OnVideoEnd; // Event saat video selesai
    }

    // --- FUNGSI START (DIUBAH) ---
    IEnumerator Start()
    {
        if (skipPromptUI != null) skipPromptUI.SetActive(false);
        
        // 1. HAPUS FADE IN
        // Kita tidak memanggil FadeIn() di sini lagi.
        
        // 2. Langsung Putar Video
        videoPlayer.Play();

        // 3. Tampilkan UI Skip setelah delay
        yield return new WaitForSeconds(skipPromptDelay);
        if (skipPromptUI != null) skipPromptUI.SetActive(true);
    }

    void Update()
    {
        // Cek input skip (Tidak Berubah)
        if (Input.GetKeyDown(KeyCode.E) && !isSkipping)
        {
            LoadNextScene();
        }
    }

    // Dipanggil otomatis saat video selesai (Tidak Berubah)
    void OnVideoEnd(VideoPlayer vp)
    {
        LoadNextScene();
    }

    // (Tidak Berubah)
    void LoadNextScene()
    {
        if (isSkipping) return;
        isSkipping = true;
        
        videoPlayer.Stop();
        StartCoroutine(Sequence_LoadNextScene());
    }

    // (Tidak Berubah) - Fungsi ini TETAP PAKAI FADE OUT
    private IEnumerator Sequence_LoadNextScene()
    {
        if (FadeManager.instance != null)
        {
            yield return StartCoroutine(FadeManager.instance.FadeOut());
        }
        SceneManager.LoadScene(nextSceneName);
    }
}