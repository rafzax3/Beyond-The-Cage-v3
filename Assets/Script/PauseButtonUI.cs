using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseButtonUI : MonoBehaviour
{
    [Header("Referensi")]
    [SerializeField] private Button pauseButton;
    [SerializeField] private Image buttonImage;

    [Header("Sprite Ikon")]
    [SerializeField] private Sprite pauseIcon;
    [SerializeField] private Sprite resumeIcon;
    
    [Header("Pengaturan Scene")]
    [Tooltip("Nama scene Main Menu")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    [Tooltip("Nama scene Intro Cutscene")]
    [SerializeField] private string introSceneName = "Intro_Cutscene"; // <-- TAMBAHAN BARU

    void Start()
    {
        if (pauseButton != null)
        {
            pauseButton.onClick.RemoveAllListeners();
            pauseButton.onClick.AddListener(TogglePause);
        }
        UpdateVisibility();
    }

    void Update()
    {
        UpdateVisibility();

        if (PauseMenu.instance != null)
        {
            UpdateIcon(PauseMenu.instance.IsGamePaused());
        }
    }
    
    void UpdateVisibility()
    {
        if (pauseButton == null) return;

        string currentScene = SceneManager.GetActiveScene().name;
        
        // --- LOGIKA BARU ---
        // Tombol mati jika di Main Menu ATAU di Intro
        bool isMenu = (currentScene == mainMenuSceneName);
        bool isIntro = (currentScene == introSceneName);

        bool shouldBeActive = !isMenu && !isIntro;
        // -------------------

        if (pauseButton.gameObject.activeSelf != shouldBeActive)
        {
            pauseButton.gameObject.SetActive(shouldBeActive);
        }
    }

    void TogglePause()
    {
        if (PauseMenu.instance != null)
        {
            if (PauseMenu.instance.IsGamePaused())
            {
                PauseMenu.instance.ResumeGame();
            }
            else
            {
                if (InspectManager.instance == null || !InspectManager.instance.IsInspecting())
                {
                    PauseMenu.instance.PauseGame();
                }
            }
        }
    }

    void UpdateIcon(bool isPaused)
    {
        if (buttonImage != null)
        {
            buttonImage.sprite = isPaused ? resumeIcon : pauseIcon;
        }
    }
}