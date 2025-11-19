using UnityEngine;

// DITEMPEL DI OBJEK APAPUN YANG BISA DI-INSPEKSI
//
// Versi Sederhana (V2 "Pintar")
// Akan otomatis menambahkan BoxCollider2D jika belum ada
public class InteractableObject : MonoBehaviour
{
    [Header("UI Prompt")]
    [Tooltip("Seret (drag) UI 'E' prompt ke sini")]
    [SerializeField] private GameObject promptUI; 

    [Header("Inspect Data (Statis)")]
    [Tooltip("Gambar yang akan ditampilkan saat di-inspeksi")]
    public Sprite inspectSprite;
    
    [Tooltip("Teks yang akan ditampilkan (setiap elemen adalah 1 halaman)")]
    [TextArea(3, 5)] 
    public string[] inspectPages;
    
    [Header("Audio")]
    [Tooltip("Suara yang diputar saat inspeksi dimulai")]
    public AudioClip inspectSound; 

    private Collider2D myCollider; // Referensi ke collider

    // Fungsi 'Reset' dipanggil di Editor saat skrip ditambahkan
    void Reset()
    {
        SetupComponents();
    }

    void Awake()
    {
        SetupComponents();
        if (promptUI != null) promptUI.SetActive(false);
    }

    // Fungsi helper baru untuk setup
    void SetupComponents()
    {
        myCollider = GetComponent<Collider2D>();
        if (myCollider == null)
        {
            // Jika tidak ada collider, tambahkan satu
            Debug.LogWarning("InteractableObject: Collider2D tidak ditemukan. Menambahkan BoxCollider2D.");
            myCollider = gameObject.AddComponent<BoxCollider2D>();
        }
        
        // Pastikan collider ini adalah Trigger
        myCollider.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Beri tahu manager tentang diri kita (skrip ini)
        if (other.CompareTag("Player") && InspectManager.instance != null)
        {
            InspectManager.instance.SetCurrentInteractable(this);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && InspectManager.instance != null)
        {
            InspectManager.instance.ClearCurrentInteractable(this);
        }
    }

    public void ShowPrompt(bool show)
    {
        if (promptUI != null)
        {
            promptUI.SetActive(show);
        }
    }
}