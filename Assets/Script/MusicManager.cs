using UnityEngine;
using System.Collections;

// Ditempel di 'MusicManager' (GameObject kosong) di scene MainMenu
public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;

    [Header("Audio Sources")]
    [Tooltip("AudioSource untuk BGM Menu")]
    [SerializeField] private AudioSource menuMusicSource;
    [Tooltip("AudioSource untuk BGM Gameplay")]
    [SerializeField] private AudioSource gameplayMusicSource;
    [Tooltip("AudioSource untuk BGM Ending")] // <-- TAMBAHAN BARU
    [SerializeField] private AudioSource endingMusicSource;

    [Header("Audio Clips")]
    [Tooltip("Seret (drag) file musik menu ke sini")]
    [SerializeField] private AudioClip menuMusicClip;
    [Tooltip("Seret (drag) file musik gameplay ke sini")]
    [SerializeField] private AudioClip gameplayMusicClip;
    [Tooltip("Seret (drag) file musik ending ke sini")] // <-- TAMBAHAN BARU
    [SerializeField] private AudioClip endingMusicClip;

    [Header("Pengaturan Fade")]
    [Tooltip("Durasi cross-fade antar musik (detik)")]
    [SerializeField] private float fadeDuration = 1.0f;

    [Header("Pengaturan Volume")]
    [Range(0f, 1f)]
    [SerializeField] private float maxVolume = 0.7f; 

    void Awake()
    {
        // Pola Singleton
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Setup AudioSources
        if (menuMusicSource != null && menuMusicClip != null)
        {
            menuMusicSource.clip = menuMusicClip;
            menuMusicSource.loop = true;
            menuMusicSource.playOnAwake = false;
            menuMusicSource.volume = 0f; 
        }
        if (gameplayMusicSource != null && gameplayMusicClip != null)
        {
            gameplayMusicSource.clip = gameplayMusicClip;
            gameplayMusicSource.loop = true;
            gameplayMusicSource.playOnAwake = false;
            gameplayMusicSource.volume = 0f;
        }
        
        // --- TAMBAHAN BARU ---
        if (endingMusicSource != null && endingMusicClip != null)
        {
            endingMusicSource.clip = endingMusicClip;
            endingMusicSource.loop = true;
            endingMusicSource.playOnAwake = false;
            endingMusicSource.volume = 0f;
        }
        // ---------------------
    }

    // Dipanggil oleh MainMenu
    public void PlayMenuMusic()
    {
        if (menuMusicSource.isPlaying && menuMusicSource.volume > 0.01f) return;
        
        // Matikan dua lagu lainnya
        StartCoroutine(FadeTrack(menuMusicSource, gameplayMusicSource));
        StartCoroutine(FadeTrack(null, endingMusicSource)); // <-- DIPERBARUI
    }

    // Dipanggil oleh GameManager
    public void PlayGameplayMusic()
    {
        if (gameplayMusicSource.isPlaying && gameplayMusicSource.volume > 0.01f) return;
        
        // Matikan dua lagu lainnya
        StartCoroutine(FadeTrack(gameplayMusicSource, menuMusicSource));
        StartCoroutine(FadeTrack(null, endingMusicSource)); // <-- DIPERBARUI
    }

    // --- FUNGSI BARU ---
    // Dipanggil oleh FinalTrigger
    public void PlayEndingMusic()
    {
        if (endingMusicSource.isPlaying && endingMusicSource.volume > 0.01f) return;
        
        // Matikan dua lagu lainnya
        StartCoroutine(FadeTrack(endingMusicSource, menuMusicSource));
        StartCoroutine(FadeTrack(null, gameplayMusicSource));
    }
    // -------------------

    // (FadeTrack tidak berubah, ini sudah generik dan bagus)
    private IEnumerator FadeTrack(AudioSource sourceToFadeIn, AudioSource sourceToFadeOut)
    {
        float timer = 0f;
        
        float startVolumeOut = (sourceToFadeOut != null) ? sourceToFadeOut.volume : 0f;
        
        float startVolumeIn = 0f;
        float targetVolumeIn = maxVolume; 
        
        if (sourceToFadeIn != null)
        {
            if (!sourceToFadeIn.isPlaying)
            {
                sourceToFadeIn.Play();
            }
            startVolumeIn = sourceToFadeIn.volume; 
        }
        else
        {
            targetVolumeIn = 0f; 
        }

        while (timer < fadeDuration)
        {
            timer += Time.unscaledDeltaTime; 
            float progress = timer / fadeDuration;

            if (sourceToFadeOut != null)
            {
                sourceToFadeOut.volume = Mathf.Lerp(startVolumeOut, 0f, progress);
            }
            if (sourceToFadeIn != null)
            {
                sourceToFadeIn.volume = Mathf.Lerp(startVolumeIn, targetVolumeIn, progress);
            }

            yield return null;
        }

        if (sourceToFadeOut != null)
        {
            sourceToFadeOut.Stop();
            sourceToFadeOut.volume = 0f; 
        }
        if (sourceToFadeIn != null)
        {
            sourceToFadeIn.volume = targetVolumeIn;
        }
    }
}