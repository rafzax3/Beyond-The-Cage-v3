using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource menuMusicSource;
    [SerializeField] private AudioSource gameplayMusicSource;
    [SerializeField] private AudioSource endingMusicSource; 

    [Header("Pengaturan Volume")]
    [Range(0f, 1f)] [SerializeField] private float maxVolume = 0.7f; 
    [SerializeField] private float fadeDuration = 1.0f;

    void Awake()
    {
        if (instance == null) { instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); return; }

        if (menuMusicSource) menuMusicSource.volume = 0;
        if (gameplayMusicSource) gameplayMusicSource.volume = 0;
        if (endingMusicSource) endingMusicSource.volume = 0;
    }

    public void PlayMenuMusic()
    {
        StartCoroutine(FadeTrack(menuMusicSource, gameplayMusicSource));
        StartCoroutine(FadeTrack(null, endingMusicSource));
    }

    public void PlayGameplayMusic()
    {
        StartCoroutine(FadeTrack(gameplayMusicSource, menuMusicSource));
        StartCoroutine(FadeTrack(null, endingMusicSource));
    }

    public void PlayEndingMusic()
    {
        StartCoroutine(FadeTrack(endingMusicSource, menuMusicSource));
        StartCoroutine(FadeTrack(null, gameplayMusicSource));
    }

    public void StopAllMusic()
    {
        StartCoroutine(FadeTrack(null, menuMusicSource));
        StartCoroutine(FadeTrack(null, gameplayMusicSource));
        StartCoroutine(FadeTrack(null, endingMusicSource));
    }

    private IEnumerator FadeTrack(AudioSource sourceToFadeIn, AudioSource sourceToFadeOut)
    {
        float timer = 0f;
        float startVolumeOut = (sourceToFadeOut != null) ? sourceToFadeOut.volume : 0f;
        float startVolumeIn = (sourceToFadeIn != null) ? sourceToFadeIn.volume : 0f;
        float targetVolumeIn = maxVolume;

        if (sourceToFadeIn != null)
        {
            if (!sourceToFadeIn.isPlaying) sourceToFadeIn.Play();
            startVolumeIn = sourceToFadeIn.volume;
        }

        while (timer < fadeDuration)
        {
            timer += Time.unscaledDeltaTime;
            float progress = timer / fadeDuration;

            if (sourceToFadeOut != null)
                sourceToFadeOut.volume = Mathf.Lerp(startVolumeOut, 0f, progress);
            
            if (sourceToFadeIn != null)
                sourceToFadeIn.volume = Mathf.Lerp(startVolumeIn, targetVolumeIn, progress);

            yield return null;
        }

        if (sourceToFadeOut != null)
        {
            sourceToFadeOut.Stop();
            sourceToFadeOut.volume = 0f;
        }
        if (sourceToFadeIn != null) sourceToFadeIn.volume = targetVolumeIn;
    }
}