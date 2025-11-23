using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

[RequireComponent(typeof(VideoPlayer))]
public class SkippableVideoPlayer : MonoBehaviour
{
    [Header("Referensi")]
    [SerializeField] private GameObject skipPromptUI;

    [Header("Pengaturan")]
    [SerializeField] private string nextSceneName;
    [SerializeField] private float skipPromptDelay = 3f;

    private VideoPlayer videoPlayer;
    private bool isSkipping = false;

    void Awake()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        videoPlayer.loopPointReached += OnVideoEnd; 
    }

    IEnumerator Start()
    {
        if (skipPromptUI != null) skipPromptUI.SetActive(false);
        
        if (FadeManager.instance != null)
        {
            yield return StartCoroutine(FadeManager.instance.FadeIn());
        }

        videoPlayer.Play();

        yield return new WaitForSeconds(skipPromptDelay);
        if (skipPromptUI != null) skipPromptUI.SetActive(true);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && !isSkipping)
        {
            LoadNextScene();
        }
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        LoadNextScene();
    }

    void LoadNextScene()
    {
        if (isSkipping) return;
        isSkipping = true;
        
        videoPlayer.Stop();

        StartCoroutine(Sequence_LoadNextScene());
    }

    private IEnumerator Sequence_LoadNextScene()
    {
        if (FadeManager.instance != null)
        {
            yield return StartCoroutine(FadeManager.instance.FadeOut());
        }
        SceneManager.LoadScene(nextSceneName);
    }
}