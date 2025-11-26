using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class EndingCutscene : MonoBehaviour
{
    [Header("Referensi")]
    [SerializeField] private VideoPlayer videoPlayer;

    [Header("Pengaturan")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    [SerializeField] private float whiteFadeDuration = 2f;

    void Start()
    {
        if (FadeManager.instance != null)
        {
            FadeManager.instance.fadeImage.color = Color.white;
        }

        if (videoPlayer != null)
        {
            videoPlayer.playOnAwake = false;
            videoPlayer.loopPointReached += OnVideoFinished;
        }

        if (MusicManager.instance != null)
        {
            MusicManager.instance.PlayEndingMusic();
        }

        StartCoroutine(PlaySequence());
    }

    private IEnumerator PlaySequence()
    {
        if (videoPlayer != null)
        {
            videoPlayer.Prepare();
            while (!videoPlayer.isPrepared) yield return null;
            videoPlayer.Play();
        }

        if (FadeManager.instance != null)
        {
            yield return StartCoroutine(FadeManager.instance.FadeIn(Color.white, whiteFadeDuration));
        }
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        StartCoroutine(EndSequence());
    }

    private IEnumerator EndSequence()
    {
        if (FadeManager.instance != null)
        {
            yield return StartCoroutine(FadeManager.instance.FadeOut(Color.white, whiteFadeDuration));
        }

        SceneManager.LoadScene(mainMenuSceneName);
    }
}