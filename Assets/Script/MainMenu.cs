using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    // --- TAMBAHAN BARU: SINGLETON ---
    public static MainMenu instance;
    // -------------------------------

    [Header("Pengaturan Scene")]
    [SerializeField] private string firstSceneName;

    [Header("Referensi Tombol")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button creditsButton;
    [SerializeField] private Button quitButton;

    // --- TAMBAHAN BARU: Tombol Achievements ---
    [Tooltip("Tombol untuk membuka panel achievement")]
    [SerializeField] private Button achievementsButton;
    // ------------------------------------------

    [Header("Referensi Panel")]
    [SerializeField] private GameObject creditsPanel;

    [Header("Audio")]
    [SerializeField] private AudioSource sfxSourceUI;
    [SerializeField] private AudioClip navSfx;
    [SerializeField] private AudioClip submitSfx;

    private Button[] buttons;
    private int selectedIndex = 0;
    private bool isTransitioning = false;
    private bool isViewingCredits = false;
    private bool isViewingAchievements = false; // Status baru

    void Awake()
    {
        // Setup Singleton
        if (instance == null)
        {
            instance = this;
            // Kita TIDAK pakai DontDestroyOnLoad untuk MainMenu 
            // karena biasanya MainMenu hancur saat masuk game.
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    IEnumerator Start()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Update daftar tombol (tambahkan achievementButton jika ada)
        if (achievementsButton != null)
        {
            buttons = new Button[] { playButton, achievementsButton, creditsButton, quitButton };
        }
        else
        {
            buttons = new Button[] { playButton, creditsButton, quitButton };
        }

        if (creditsPanel != null) creditsPanel.SetActive(false);
        isViewingCredits = false;
        isViewingAchievements = false;

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

        // Jika sedang lihat Credits
        if (isViewingCredits)
        {
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.E))
            {
                HideCredits();
            }
            return;
        }

        // Jika sedang lihat Achievements (dikontrol oleh AchievementManager)
        if (isViewingAchievements)
        {
            // MainMenu diam saja, biarkan AchievementManager yang handle input
            return;
        }

        // Navigasi Menu Utama
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            selectedIndex--;
            if (selectedIndex < 0) selectedIndex = buttons.Length - 1;
            UpdateSelection();
            if (sfxSourceUI != null && navSfx != null) sfxSourceUI.PlayOneShot(navSfx);
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
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

    void UpdateSelection()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i] == null) continue;
            if (i == selectedIndex) buttons[i].Select();
        }
    }

    void PressSelectedButton()
    {
        if (sfxSourceUI != null && submitSfx != null) sfxSourceUI.PlayOneShot(submitSfx);

        if (buttons[selectedIndex] == playButton)
        {
            StartGame();
        }
        else if (achievementsButton != null && buttons[selectedIndex] == achievementsButton)
        {
            OpenAchievements();
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

    // --- FUNGSI BARU UNTUK ACHIEVEMENT ---
    void OpenAchievements()
    {
        // Cari manager di scene (karena dia mungkin DontDestroyOnLoad)
        AchievementManager am = FindObjectOfType<AchievementManager>();
        if (am != null)
        {
            isViewingAchievements = true;
            am.OpenAchievementMenu();
        }
        else
        {
            Debug.LogError("AchievementManager tidak ditemukan di scene!");
        }
    }

    // Fungsi ini dipanggil OLEH AchievementManager saat ditutup (Tekan F)
    public void ReturnToMenuFromAchievements()
    {
        isViewingAchievements = false;

        // Putar suara kembali jika perlu
        if (sfxSourceUI != null && navSfx != null) sfxSourceUI.PlayOneShot(navSfx);

        // Kembalikan fokus
        UpdateSelection();
    }
    // -------------------------------------

    void ShowCredits()
    {
        if (creditsPanel == null) return;
        isViewingCredits = true;
        creditsPanel.SetActive(true);
    }

    void HideCredits()
    {
        if (creditsPanel == null) return;

        if (sfxSourceUI != null && navSfx != null) sfxSourceUI.PlayOneShot(navSfx);

        isViewingCredits = false;
        creditsPanel.SetActive(false);

        // Kembalikan fokus (Cari index credits button)
        // Cara gampangnya: Biarkan saja selectedIndex terakhir
        UpdateSelection();
    }

    public void StartGame()
    {
        if (isTransitioning) return;
        isTransitioning = true;

        if (MusicManager.instance != null)
        {
            MusicManager.instance.StopAllMusic();
        }

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