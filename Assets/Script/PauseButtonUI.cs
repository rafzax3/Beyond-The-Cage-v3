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
    
    [Header("Pengaturan")]
    [Tooltip("Daftar nama scene di mana tombol HARUS SEMBUNYI")]
    // Tambahkan scene ending Anda di sini
    [SerializeField] private string[] hiddenScenes = new string[] { 
        "MainMenu", 
        "Intro_Cutscene", 
        "EndingTextScene", 
        "EndingCutscene" 
    };

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
        
        bool shouldHide = false;
        foreach (string sceneName in hiddenScenes)
        {
            if (currentScene == sceneName)
            {
                shouldHide = true;
                break;
            }
        }

        bool shouldBeActive = !shouldHide;

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