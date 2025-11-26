using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager instance;

    [Header("Panel Tutorial Walk (Prolog)")]
    [SerializeField] private GameObject walkTutorialPanel;
    [SerializeField] private float walkDisplayDuration = 5f;

    [Header("Panel Tutorial Run (First Room)")]
    [SerializeField] private GameObject runTutorialPanel;
    [SerializeField] private GameObject pressFToSkipText;
    [Tooltip("Durasi maksimal tutorial lari muncul (jika tidak di-skip)")]
    [SerializeField] private float runDisplayDuration = 10f; // Default 10 detik

    [Header("Panel Tutorial Letter (Hint)")]
    [SerializeField] private GameObject letterPopupPanel; 
    [SerializeField] private TextMeshProUGUI letterContentText;
    [SerializeField] private GameObject hintReminderText; 
    [SerializeField] private float reminderDuration = 3f; 

    [Header("Isi Teks Hint")]
    [TextArea(3, 5)] [SerializeField] private string letterRoomText = "Ini adalah surat misterius...";
    [TextArea(3, 5)] [SerializeField] private string letterPauseText = "Petunjuk: Perhatikan jam...";

    [Header("Audio")]
    [SerializeField] private AudioSource tutorialAudioSource;
    [SerializeField] private AudioClip meowSfx;
    [SerializeField] private AudioClip popupSfx;

    [Header("Settings")]
    [SerializeField] private float fadeDuration = 0.5f;

    private bool isShowingHint = false;
    private bool currentHintIsFromPause = false;
    private PlayerMovement playerMovement;

    void Awake()
    {
        if (instance == null) { instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); }

        ResetTutorialState();
    }

    public void ResetTutorialState()
    {
        StopAllCoroutines();
        
        SetupPanel(walkTutorialPanel);
        SetupPanel(runTutorialPanel);
        SetupPanel(letterPopupPanel);
        SetupPanel(hintReminderText);
        
        if (pressFToSkipText != null) pressFToSkipText.SetActive(false);
        
        isShowingHint = false;
        currentHintIsFromPause = false;
    }

    void SetupPanel(GameObject panel)
    {
        if (panel != null)
        {
            CanvasGroup cg = panel.GetComponent<CanvasGroup>();
            if (cg == null) cg = panel.AddComponent<CanvasGroup>();
            cg.alpha = 0;
            panel.SetActive(false);
        }
    }

    void Update()
    {
        if (isShowingHint)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                CloseHintSequence();
            }
        }
    }

    public void PlayMeowSound()
    {
        if (tutorialAudioSource != null && meowSfx != null) tutorialAudioSource.PlayOneShot(meowSfx);
    }

    public void TriggerWalkTutorial()
    {
        if (GameData.tutorialWalkShown) return;
        StartCoroutine(SequenceWalkTutorial());
    }

    private IEnumerator SequenceWalkTutorial()
    {
        GameData.tutorialWalkShown = true;
        yield return new WaitForSeconds(0.5f);

        yield return StartCoroutine(FadeUI(walkTutorialPanel, true)); 
        if (popupSfx != null) tutorialAudioSource.PlayOneShot(popupSfx);

        yield return new WaitForSeconds(walkDisplayDuration);

        yield return StartCoroutine(FadeUI(walkTutorialPanel, false)); 
    }

    public void TriggerRunTutorial()
    {
        if (GameData.tutorialRunShown) return;
        StartCoroutine(SequenceRunTutorial());
    }

    private IEnumerator SequenceRunTutorial()
    {
        GameData.tutorialRunShown = true;
        yield return new WaitForSeconds(0.5f);

        StartCoroutine(FadeUI(runTutorialPanel, true)); 
        if (popupSfx != null) tutorialAudioSource.PlayOneShot(popupSfx);
        
        if (pressFToSkipText != null) pressFToSkipText.SetActive(true);

        // --- LOGIKA BARU: Tunggu Durasi ATAU Tombol F ---
        float timer = runDisplayDuration;
        while (timer > 0 && runTutorialPanel.activeSelf) 
        {
            if (Input.GetKeyDown(KeyCode.F)) break; // Skip jika tekan F
            timer -= Time.deltaTime;
            yield return null;
        }
        // ------------------------------------------------

        StartCoroutine(FadeUI(runTutorialPanel, false)); 
        if (pressFToSkipText != null) pressFToSkipText.SetActive(false);
    }

    public void TriggerLetterTutorial()
    {
        if (GameData.tutorialLetterShown) return;
        StartCoroutine(SequenceLetterTutorial());
    }

    private IEnumerator SequenceLetterTutorial()
    {
        GameData.tutorialLetterShown = true;
        if (popupSfx != null) tutorialAudioSource.PlayOneShot(popupSfx);
        
        OpenHint(false); 
        yield return null;
    }

    private void CloseHintSequence()
    {
        StartCoroutine(SequenceCloseHint());
    }

    private IEnumerator SequenceCloseHint()
    {
        // 1. Fade Out Panel Hint (Tunggu sampai selesai dulu!)
        yield return StartCoroutine(FadeUI(letterPopupPanel, false));
        
        isShowingHint = false; 

        // Buka Kunci Player
        if (playerMovement != null)
        {
            playerMovement.SetLock(false, playerMovement.IsFacingRight());
        }

        // 2. Cek apakah perlu menampilkan Reminder Text
        // (Hanya jika BUKAN dari Pause Menu)
        if (!currentHintIsFromPause && hintReminderText != null)
        {
            // --- PERBAIKAN: Jeda 1 detik SETELAH panel hilang ---
            yield return new WaitForSeconds(1f); 
            // ----------------------------------------------------
            
            // Fade In Teks
            yield return StartCoroutine(FadeUI(hintReminderText, true)); 
            
            // Tunggu durasi baca
            yield return new WaitForSeconds(reminderDuration);
            
            // Fade Out Teks
            yield return StartCoroutine(FadeUI(hintReminderText, false)); 
        }
        
        currentHintIsFromPause = false;
    }

    public void OpenHint(bool fromPauseMenu)
    {
        isShowingHint = true;
        currentHintIsFromPause = fromPauseMenu;

        if (letterContentText != null)
        {
            if (fromPauseMenu) letterContentText.text = letterPauseText;
            else letterContentText.text = letterRoomText;
        }

        StartCoroutine(FadeUI(letterPopupPanel, true));

        playerMovement = FindObjectOfType<PlayerMovement>();
        if (playerMovement != null)
        {
            playerMovement.SetLock(true, playerMovement.IsFacingRight());
        }
    }

    public void CloseHint()
    {
       CloseHintSequence();
    }

    public void HideAllTutorials()
    {
        StopAllCoroutines();
        if (walkTutorialPanel != null && walkTutorialPanel.activeSelf) StartCoroutine(FadeUI(walkTutorialPanel, false, 0.2f));
        if (runTutorialPanel != null && runTutorialPanel.activeSelf) StartCoroutine(FadeUI(runTutorialPanel, false, 0.2f));
        if (letterPopupPanel != null && letterPopupPanel.activeSelf) StartCoroutine(FadeUI(letterPopupPanel, false, 0.2f));
        if (hintReminderText != null && hintReminderText.activeSelf) StartCoroutine(FadeUI(hintReminderText, false, 0.2f));
        if (pressFToSkipText != null) pressFToSkipText.SetActive(false);
        
        if (isShowingHint)
        {
            isShowingHint = false;
            if (playerMovement != null) playerMovement.SetLock(false, playerMovement.IsFacingRight());
        }
    }

    private IEnumerator FadeUI(GameObject panel, bool fadeIn, float duration = -1f)
    {
        if (panel == null) yield break;

        CanvasGroup cg = panel.GetComponent<CanvasGroup>();
        if (cg == null) cg = panel.AddComponent<CanvasGroup>(); 

        if (duration < 0) duration = fadeDuration;

        float startAlpha = cg.alpha;
        float endAlpha = fadeIn ? 1f : 0f;

        if (fadeIn) panel.SetActive(true);

        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime; 
            cg.alpha = Mathf.Lerp(startAlpha, endAlpha, timer / duration);
            yield return null;
        }

        cg.alpha = endAlpha;
        if (!fadeIn) panel.SetActive(false);
    }

    public bool IsInTutorialMode()
    {
        return isShowingHint;
    }
}