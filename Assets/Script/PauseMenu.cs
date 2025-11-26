using UnityEngine;
using UnityEngine.SceneManagement; 
using UnityEngine.UI;
using System.Collections;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu instance;

    [Header("Referensi UI")]
    [SerializeField] private GameObject pauseCanvas;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button restartButton; // Urutan 2
    [SerializeField] private Button hintButton;    // Urutan 3
    [SerializeField] private Button quitButton;    // Urutan 4

    [Header("Pengaturan")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    [SerializeField] private string prologSceneName = "FirstRoom"; 

    [Header("Audio")]
    [SerializeField] private AudioSource sfxSourceUI;
    [SerializeField] private AudioClip pauseSfx;
    [SerializeField] private AudioClip navSfx;
    [SerializeField] private AudioClip submitSfx;

    private Button[] buttons;
    private int selectedIndex = 0;
    private bool isPaused = false;
    private Coroutine resumeCoroutine;
    private Coroutine quitCoroutine;
    private PlayerMovement playerMovement; 

    void Awake()
    {
        if (instance == null) { instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); return; }
        
        if (sfxSourceUI == null) sfxSourceUI = GetComponent<AudioSource>();

        // --- URUTAN TOMBOL DIPERBARUI ---
        buttons = new Button[] { resumeButton, restartButton, hintButton, quitButton };
        // --------------------------------
        
        if (pauseCanvas != null) pauseCanvas.SetActive(false);
    }

    void Update()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        if (currentSceneName == mainMenuSceneName) return;
        if (resumeCoroutine != null || quitCoroutine != null) return;
        if (TutorialManager.instance != null && TutorialManager.instance.IsInTutorialMode()) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) ResumeGame();
            else
            {
                if (InspectManager.instance != null && InspectManager.instance.IsInspecting()) return;
                PauseGame();
            }
        }

        if (!isPaused) return;

        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            selectedIndex--;
            if (selectedIndex < 0) selectedIndex = buttons.Length - 1;
            UpdateSelection();
            if (sfxSourceUI != null && navSfx != null) sfxSourceUI.PlayOneShot(navSfx);
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            selectedIndex++;
            if (selectedIndex >= buttons.Length) selectedIndex = 0;
            UpdateSelection();
            if (sfxSourceUI != null && navSfx != null) sfxSourceUI.PlayOneShot(navSfx);
        }

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.E))
        {
            PressSelectedButton();
        }
    }

    public bool IsGamePaused() { return isPaused || resumeCoroutine != null || quitCoroutine != null; }

    // --- DIBUAT PUBLIC ---
    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        if (pauseCanvas != null) pauseCanvas.SetActive(true);
        if (sfxSourceUI != null && pauseSfx != null) sfxSourceUI.PlayOneShot(pauseSfx);
        
        if (playerMovement == null) playerMovement = FindObjectOfType<PlayerMovement>();
        if (playerMovement != null) playerMovement.SetLock(true, playerMovement.IsFacingRight());
        
        selectedIndex = 0;
        UpdateSelection();
    }

    public void ResumeGame()
    {
        if (resumeCoroutine == null) resumeCoroutine = StartCoroutine(DelayedResume());
    }

    private IEnumerator DelayedResume()
    {
        if (sfxSourceUI != null && submitSfx != null) sfxSourceUI.PlayOneShot(submitSfx);
        yield return null; 
        isPaused = false;
        Time.timeScale = 1f; 
        if (pauseCanvas != null) pauseCanvas.SetActive(false);
        if (playerMovement != null) playerMovement.SetLock(false, playerMovement.IsFacingRight());
        resumeCoroutine = null;
    }

    public void ShowHint()
    {
        if (TutorialManager.instance != null)
        {
            if (pauseCanvas != null) pauseCanvas.SetActive(false);
            isPaused = false; 
            Time.timeScale = 1f; 
            TutorialManager.instance.OpenHint(true); 
        }
    }

    public void RestartGame()
    {
        StartCoroutine(DelayedRestart());
    }

    private IEnumerator DelayedRestart()
    {
        if (sfxSourceUI != null && submitSfx != null) sfxSourceUI.PlayOneShot(submitSfx);
        yield return null;
        
        GameData.ResetAllData();
        
        isPaused = false;
        Time.timeScale = 1f;
        if (pauseCanvas != null) pauseCanvas.SetActive(false);
        
        if (InspectManager.instance != null) InspectManager.instance.CancelInspect();
        if (TutorialManager.instance != null) TutorialManager.instance.ResetTutorialState();
        // if (ObjectiveManager.instance != null) ObjectiveManager.instance.HideObjective(); // Dihapus sesuai request

        if (FadeManager.instance != null) yield return StartCoroutine(FadeManager.instance.FadeOut());
        
        SceneManager.LoadScene(prologSceneName);
    }

    public void QuitToMenu()
    {
        if (quitCoroutine == null) quitCoroutine = StartCoroutine(DelayedQuitToMenu());
    }

    private IEnumerator DelayedQuitToMenu()
    {
        if (sfxSourceUI != null && submitSfx != null) sfxSourceUI.PlayOneShot(submitSfx);
        yield return null; 
        isPaused = false;
        Time.timeScale = 1f; 
        if (pauseCanvas != null) pauseCanvas.SetActive(false);
        
        if (InspectManager.instance != null) InspectManager.instance.CancelInspect();
        if (TutorialManager.instance != null) TutorialManager.instance.ResetTutorialState();
        // if (ObjectiveManager.instance != null) ObjectiveManager.instance.HideObjective(); // Dihapus sesuai request
        if (MusicManager.instance != null) MusicManager.instance.PlayMenuMusic();
        
        playerMovement = null;
        
        if (FadeManager.instance != null) yield return StartCoroutine(FadeManager.instance.FadeOut());
        
        SceneManager.LoadScene(mainMenuSceneName);
        quitCoroutine = null;
    }

    void UpdateSelection()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i] != null && i == selectedIndex) buttons[i].Select();
        }
    }

    void PressSelectedButton()
    {
        if (buttons[selectedIndex] == resumeButton) ResumeGame();
        else if (buttons[selectedIndex] == restartButton) RestartGame();
        else if (buttons[selectedIndex] == hintButton) ShowHint();
        else if (buttons[selectedIndex] == quitButton) QuitToMenu();
    }
}