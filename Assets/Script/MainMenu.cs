using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Pengaturan Scene")]
    [SerializeField] private string firstSceneName;

    [Header("Referensi Tombol")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button creditsButton; 
    [SerializeField] private Button quitButton;

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

    IEnumerator Start()
    {
        Time.timeScale = 1f;
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

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

        if (isViewingCredits)
        {
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.E))
            {
                HideCredits();
            }
            return; 
        }

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
        
        selectedIndex = 1; 
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