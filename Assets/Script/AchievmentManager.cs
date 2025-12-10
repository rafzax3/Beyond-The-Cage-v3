using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class AchievementManager : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private List<AnomalyData> allAnomalies; // Masukkan semua ScriptableObject di sini

    [Header("UI List")]
    [SerializeField] private GameObject achievementMenuPanel;
    [SerializeField] private Transform listContentContainer; // Tempat item-item akan dimunculkan (di dalam ScrollView)
    [SerializeField] private GameObject listItemPrefab; // Prefab untuk satu baris anomali
    [SerializeField] private ScrollRect scrollRect;

    [Header("UI Detail (Inspect Mode)")]
    [SerializeField] private GameObject detailPanel;
    [SerializeField] private Image detailImage;
    [SerializeField] private TextMeshProUGUI detailTitle;
    [SerializeField] private TextMeshProUGUI detailDescription;

    [Header("Navigasi")]
    [SerializeField] private Color selectedColor = Color.yellow;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private float scrollSpeed = 0.5f; // Kecepatan auto-scroll

    // State
    private bool isMenuOpen = false;
    private bool isDetailOpen = false;
    private int selectedIndex = 0;
    private List<GameObject> spawnedItems = new List<GameObject>();

    void Start()
    {
        // Load data permanen saat game mulai
        GameData.LoadAchievements();

        // Pastikan menu tertutup
        CloseAchievementMenu();
    }

    void Update()
    {
        if (!isMenuOpen) return;

        if (isDetailOpen)
        {
            // --- Mode Detail ---
            if (Input.GetKeyDown(KeyCode.F))
            {
                CloseDetailView();
            }
        }
        else
        {
            // --- Mode List ---
            if (Input.GetKeyDown(KeyCode.F))
            {
                CloseAchievementMenu();
                // Kembali ke Main Menu (misal: aktifkan kembali tombol Play/Credits)
                // Anda mungkin perlu event/callback ke MainMenu.cs
                MainMenu.instance.ReturnToMenuFromAchievements();
            }

            // Navigasi W/S
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                MoveSelection(-1);
            }
            else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                MoveSelection(1);
            }

            // Pilih (Enter/E)
            if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Return))
            {
                OpenDetailView();
            }
        }
    }

    public void OpenAchievementMenu()
    {
        isMenuOpen = true;
        achievementMenuPanel.SetActive(true);
        PopulateList();
        selectedIndex = 0;
        UpdateSelectionVisual();
    }

    public void CloseAchievementMenu()
    {
        isMenuOpen = false;
        achievementMenuPanel.SetActive(false);
        detailPanel.SetActive(false);
    }

    void PopulateList()
    {
        // Bersihkan list lama
        foreach (var item in spawnedItems) Destroy(item);
        spawnedItems.Clear();

        // Buat item baru
        foreach (var data in allAnomalies)
        {
            GameObject newItem = Instantiate(listItemPrefab, listContentContainer);
            spawnedItems.Add(newItem);

            // Cek status unlock
            bool isUnlocked = GameData.IsAnomalyUnlocked(data.sceneNameID);

            // Setup tampilan item (Anda perlu script helper di prefab item atau GetComponent)
            // Asumsi prefab punya komponen 'AchievementItemUI'
            AchievementItemUI itemUI = newItem.GetComponent<AchievementItemUI>();
            if (itemUI != null)
            {
                if (isUnlocked)
                    itemUI.Setup(data.anomalyImage, data.anomalyTitle, true);
                else
                    itemUI.Setup(data.lockedImage, data.lockedTitle, false);
            }
        }
    }

    void MoveSelection(int direction)
    {
        selectedIndex += direction;

        // Clamp / Wrap
        if (selectedIndex < 0) selectedIndex = spawnedItems.Count - 1;
        if (selectedIndex >= spawnedItems.Count) selectedIndex = 0;

        UpdateSelectionVisual();
        AutoScroll();
    }

    void UpdateSelectionVisual()
    {
        for (int i = 0; i < spawnedItems.Count; i++)
        {
            AchievementItemUI itemUI = spawnedItems[i].GetComponent<AchievementItemUI>();
            if (i == selectedIndex)
                itemUI.SetSelected(true, selectedColor);
            else
                itemUI.SetSelected(false, normalColor);
        }
    }

    void AutoScroll()
    {
        // Logika sederhana untuk scroll ke item terpilih
        if (scrollRect == null) return;

        // Hitung posisi normalized (0-1) berdasarkan index
        float normalizedPos = 1f - ((float)selectedIndex / (spawnedItems.Count - 1));
        // Bisa diperhalus dengan Lerp di Update jika mau
        scrollRect.verticalNormalizedPosition = normalizedPos;
    }

    void OpenDetailView()
    {
        isDetailOpen = true;
        detailPanel.SetActive(true);

        AnomalyData data = allAnomalies[selectedIndex];
        bool isUnlocked = GameData.IsAnomalyUnlocked(data.sceneNameID);

        if (isUnlocked)
        {
            detailImage.sprite = data.anomalyImage;
            detailTitle.text = data.anomalyTitle;
            detailDescription.text = data.anomalyDescription;
        }
        else
        {
            detailImage.sprite = data.lockedImage;
            detailTitle.text = data.lockedTitle;
            detailDescription.text = data.lockedDescription;
        }
    }

    void CloseDetailView()
    {
        isDetailOpen = false;
        detailPanel.SetActive(false);
    }
}