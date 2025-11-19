using UnityEngine;

// DITEMPEL DI OBJEK YANG MENGELUARKAN SUARA (MISAL: KULKAS, JAM)
//
// Versi 3 (Versi "Benar")
// Ini akan OTOMATIS menambahkan AudioSource dan BoxCollider2D
// SAAT Anda menempelkan skrip ini di Inspector (Editor Mode).
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(BoxCollider2D))]
public class ProximitySound : MonoBehaviour
{
    private AudioSource myAudioSource;
    private Collider2D myCollider;

    // Fungsi 'Reset' dipanggil di Editor saat skrip ditambahkan
    void Reset()
    {
        // Panggil 'Awake' secara manual untuk setup di editor
        SetupComponents();
    }

    void Awake()
    {
        SetupComponents();
    }

    // Satu fungsi untuk setup, bisa dipanggil dari Awake() atau Reset()
    void SetupComponents()
    {
        // Kita BISA berasumsi ini ada, berkat [RequireComponent]
        myAudioSource = GetComponent<AudioSource>();
        myCollider = GetComponent<Collider2D>();

        // Pastikan collider adalah Trigger
        myCollider.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Saat player masuk zona, putar suara
        if (other.CompareTag("Player"))
        {
            if (myAudioSource != null && !myAudioSource.isPlaying)
            {
                myAudioSource.Play();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Saat player keluar zona, hentikan suara
        if (other.CompareTag("Player"))
        {
            if (myAudioSource != null && myAudioSource.isPlaying)
            {
                myAudioSource.Stop();
            }
        }
    }
}