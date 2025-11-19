using UnityEngine;
using UnityEngine.Video; // Penting untuk VideoPlayer
using UnityEngine.SceneManagement;

// Ini adalah script simpel PENGGANTI SkippableVideoPlayer
// Script ini TIDAK pakai FADE.
public class VideoPindahScene : MonoBehaviour
{
    [Header("Referensi")]
    [Tooltip("Seret (drag) komponen VideoPlayer dari scene ini")]
    [SerializeField] private VideoPlayer videoPlayer;

    [Header("Tujuan Pindah Scene")]
    [Tooltip("Nama scene yang akan dimuat setelah video selesai (WAJIB DIISI)")]
    [SerializeField] private string namaSceneBerikutnya;

    private bool sudahPindah = false;

    void Awake()
    {
        // Jika videoPlayer belum di-set, cari otomatis
        if (videoPlayer == null)
        {
            videoPlayer = GetComponent<VideoPlayer>();
        }

        // Ini adalah 'event listener'
        // Maksudnya: "Saat videoPlayer mencapai akhir (loopPointReached),
        // panggil fungsi kita yang bernama OnVideoSelesai"
        videoPlayer.loopPointReached += OnVideoSelesai;
    }

    void Update()
    {
        // Cek jika player menekan tombol 'E' untuk skip
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Video di-skip!");
            PindahScene();
        }
    }

    // Fungsi ini akan dipanggil OTOMATIS saat video selesai
    void OnVideoSelesai(VideoPlayer vp)
    {
        Debug.Log("Video selesai!");
        PindahScene();
    }

    // Satu fungsi untuk pindah scene (agar tidak duplikat kode)
    void PindahScene()
    {
        // Cek agar tidak pindah scene berkali-kali
        if (sudahPindah) return;
        sudahPindah = true;

        // Cek jika Anda lupa mengisi nama scene di Inspector
        if (string.IsNullOrEmpty(namaSceneBerikutnya))
        {
            Debug.LogError("Nama Scene Berikutnya di VideoPindahScene belum diisi!");
        }
        else
        {
            // Langsung pindah scene (sama seperti MENU.cs Anda)
            // Kita pakai LoadScene, bukan LoadSceneAsync, karena lebih simpel
            SceneManager.LoadScene(namaSceneBerikutnya);
        }
    }
}