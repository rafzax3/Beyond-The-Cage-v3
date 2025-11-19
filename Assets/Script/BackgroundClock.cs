using UnityEngine;

// Script ini ditempel di objek 'BackgroundClock'
public class BackgroundClock : MonoBehaviour
{
    [Header("Referensi Komponen")]
    [SerializeField] private SpriteRenderer clockSpriteRenderer;

    [Header("Sprite Jam (Total 7)")]
    [Tooltip("Sprite untuk jam 12 (index 0)")]
    [SerializeField] private Sprite hour_12; // index 0

    [Tooltip("Sprite untuk jam 1 (index 1)")]
    [SerializeField] private Sprite hour_01; // index 1
    
    [Tooltip("Sprite untuk jam 2 (index 2)")]
    [SerializeField] private Sprite hour_02; // index 2
    
    [Tooltip("Sprite untuk jam 3 (index 3)")]
    [SerializeField] private Sprite hour_03; // index 3
    
    [Tooltip("Sprite untuk jam 4 (index 4)")]
    [SerializeField] private Sprite hour_04; // index 4
    
    [Tooltip("Sprite untuk jam 5 (index 5)")]
    [SerializeField] private Sprite hour_05; // index 5
    
    [Tooltip("Sprite untuk jam 6 (index 6)")]
    [SerializeField] private Sprite hour_06; // index 6

    // Kita akan membuat 'Array' (daftar) untuk menyimpan sprite ini
    private Sprite[] clockSprites;

    void Awake()
    {
        // Isi daftar 'clockSprites' kita
        clockSprites = new Sprite[7];
        clockSprites[0] = hour_12;
        clockSprites[1] = hour_01;
        clockSprites[2] = hour_02;
        clockSprites[3] = hour_03;
        clockSprites[4] = hour_04;
        clockSprites[5] = hour_05;
        clockSprites[6] = hour_06;

        if (clockSpriteRenderer == null)
        {
            clockSpriteRenderer = GetComponent<SpriteRenderer>();
        }
    }

    // Fungsi ini akan dipanggil oleh GameManager
    public void SetClockHour(int hour)
    {
        if (clockSpriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer di BackgroundClock belum di-set!");
            return;
        }

        if (hour >= 0 && hour < clockSprites.Length)
        {
            if(clockSprites[hour] != null)
            {
                clockSpriteRenderer.sprite = clockSprites[hour];
            }
        }
        else
        {
            Debug.LogWarning($"Jam {hour} tidak valid, menggunakan jam 12 sebagai default.");
            if(clockSprites[0] != null)
            {
                clockSpriteRenderer.sprite = clockSprites[0]; // Default ke jam 12
            }
        }
    }
}