using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Ditempel di 'MenuManager' di scene MainMenu
public class MainMenu : MonoBehaviour
{
    [Header("Pengaturan Scene")]
    [Tooltip("Nama scene pertama yang akan dimuat (misal: Cutscene)")]
    [SerializeField] private string firstSceneName;

    [Header("Referensi Tombol")]
    [SerializeField] private Button playButton;
    [Tooltip("Tombol untuk membuka panel credits")]
    [SerializeField] private Button creditsButton; 
    [SerializeField] private Button quitButton;

    [Header("Referensi Panel")]
    [Tooltip("Panel UI yang berisi info credits")]
    [SerializeField] private GameObject creditsPanel;

    [Header("Audio")]
    [Tooltip("Sumber SFX untuk UI (Seret dari PauseManager)")]
    [SerializeField] private AudioSource sfxSourceUI;
    [Tooltip("SFX saat pindah tombol (navigasi)")]
    [SerializeField] private AudioClip navSfx;
    [Tooltip("SFX saat memilih tombol (submit)")]
    [SerializeField] private AudioClip submitSfx;

    private Button[] buttons;
    private int selectedIndex = 0;
    private bool isTransitioning = false;
    private bool isViewingCredits = false;

    IEnumerator Start()
    {
        Time.timeScale = 1f;
        buttons = new Button[] { playButton, creditsButton, quitButton };
        
        if (creditsPanel != null) creditsPanel.SetActive(false);
        isViewingCredits = false;

        UpdateSelection();
        
        if (FadeManager.instance != null)
        {
            yield return StartCoroutine(FadeManager.instance.FadeIn());
        }
        if (MusicManager.instance != null)
        {
            MusicManager.instance.PlayMenuMusic();
        }
    }

    void Update()
    {
        if (isTransitioning) return;

        // --- LOGIKA STATE DIPERBARUI ---
        if (isViewingCredits)
        {
            // Jika kita sedang melihat credits,
            // dengarkan 'Escape' ATAU 'E' (Submit) untuk kembali.
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.E))
            {
                HideCredits();
            }
            return; // Hentikan di sini.
        }
        // -------------------------

        // --- Navigasi Menu Utama (W/S) ---
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            selectedIndex--;
            if (selectedIndex < 0) selectedIndex = buttons.Length - 1; // Wrap ke bawah
            UpdateSelection();
            if (sfxSourceUI != null && navSfx != null) sfxSourceUI.PlayOneShot(navSfx);
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            selectedIndex++;
            if (selectedIndex >= buttons.Length) selectedIndex = 0; // Wrap ke atas
            UpdateSelection();
            if (sfxSourceUI != null && navSfx != null) sfxSourceUI.PlayOneShot(navSfx);
        }

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.E))
        {
            PressSelectedButton();
        }
    }

    void UpdateSelection()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i] == null) continue; 
            
            if (i == selectedIndex)
            {
                buttons[i].Select();
            }
        }
    }

    void PressSelectedButton()
    {
        if (sfxSourceUI != null && submitSfx != null) sfxSourceUI.PlayOneShot(submitSfx);

        if (buttons[selectedIndex] == playButton)
        {
            StartGame();
        }
        else if (buttons[selectedIndex] == creditsButton)
        {
            ShowCredits();
        }
        else if (buttons[selectedIndex] == quitButton)
        {
            QuitGame();
        }
    }

    // --- FUNGSI BARU UNTUK CREDITS ---
    void ShowCredits()
    {
        if (creditsPanel == null) return;
        isViewingCredits = true;
        creditsPanel.SetActive(true);
    }

    void HideCredits()
    {
        if (creditsPanel == null) return;
        
        // Putar suara "kembali" (kita pakai navSfx)
        if (sfxSourceUI != null && navSfx != null) sfxSourceUI.PlayOneShot(navSfx); 
        
        isViewingCredits = false;
        creditsPanel.SetActive(false);
        
        // Fokuskan kembali ke tombol "Credits" (yang index-nya 1)
        selectedIndex = 1; 
        UpdateSelection();
    }
    // ---------------------------------

    public void StartGame()
    {
        if (isTransitioning) return;
        isTransitioning = true;
        
        SceneManager.LoadScene(firstSceneName);
    }

    public void QuitGame()
    {
        if (isTransitioning) return;
        isTransitioning = true;
        
        Debug.Log("Keluar dari Game...");
        Application.Quit();
    }
}