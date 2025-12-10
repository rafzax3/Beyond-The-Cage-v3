using UnityEngine;
using System.Collections.Generic;

public static class GameData
{
    // ... (Variabel lama tetap ada) ...
    public static int currentHour = 0;
    public static bool isAnomalyPresent = false;
    public static bool spawnPlayerFromLeft = true;
    public static bool isSprinting = false;

    public static bool tutorialWalkShown = false;
    public static bool tutorialRunShown = false;
    public static bool tutorialLetterShown = false;

    public static int consecutiveNormalCount = 0;

    // --- SISTEM ACHIEVEMENT (BARU) ---

    // Menyimpan ID (nama scene) anomali yang sudah terbuka
    // Kita gunakan HashSet agar tidak ada duplikat dan pencarian cepat
    private static HashSet<string> unlockedAnomalies = new HashSet<string>();

    // Key untuk PlayerPrefs
    private const string PREFS_KEY_ACHIEVEMENTS = "UnlockedAnomalies";

    // Panggil ini saat game mulai (di MainMenu Awake atau AchievementManager Awake)
    public static void LoadAchievements()
    {
        string savedData = PlayerPrefs.GetString(PREFS_KEY_ACHIEVEMENTS, "");
        if (!string.IsNullOrEmpty(savedData))
        {
            string[] items = savedData.Split('|');
            unlockedAnomalies.Clear();
            foreach (var item in items)
            {
                if (!string.IsNullOrEmpty(item)) unlockedAnomalies.Add(item);
            }
        }
    }

    // Panggil ini saat player berhasil menebak anomali
    public static void UnlockAnomaly(string anomalyID)
    {
        if (!unlockedAnomalies.Contains(anomalyID))
        {
            unlockedAnomalies.Add(anomalyID);
            SaveAchievements();
            Debug.Log($"Achievement Unlocked: {anomalyID}");
        }
    }

    public static bool IsAnomalyUnlocked(string anomalyID)
    {
        return unlockedAnomalies.Contains(anomalyID);
    }

    private static void SaveAchievements()
    {
        // Gabungkan semua ID dengan pemisah '|'
        string data = string.Join("|", unlockedAnomalies);
        PlayerPrefs.SetString(PREFS_KEY_ACHIEVEMENTS, data);
        PlayerPrefs.Save();
    }

    // --------------------------------

    public static void ResetAllData()
    {
        currentHour = 0;
        isAnomalyPresent = false;
        spawnPlayerFromLeft = true;
        isSprinting = false;
        consecutiveNormalCount = 0;

        tutorialWalkShown = false;
        tutorialRunShown = false;
        tutorialLetterShown = false;

        // PENTING: Kita TIDAK me-reset achievement saat Restart Game biasa.
        // Achievement harus permanen.
        // Jika ingin reset total (Hard Reset), buat fungsi terpisah.
    }
}