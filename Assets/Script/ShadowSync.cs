using UnityEngine;

// DITEMPEL DI OBJEK 'SHADOW' (ANAK PLAYER)
[RequireComponent(typeof(SpriteRenderer))]
public class ShadowSync : MonoBehaviour
{
    [Header("Referensi")]
    [Tooltip("SpriteRenderer milik Player (Induk)")]
    [SerializeField] private SpriteRenderer playerRenderer;

    [Header("Pengaturan Bayangan")]
    [Tooltip("Warna bayangan (biasanya hitam)")]
    [SerializeField] private Color shadowColor = new Color(0, 0, 0, 0.5f);
    
    [Tooltip("Offset posisi bayangan")]
    [SerializeField] private Vector3 offset = new Vector3(0, -0.4f, 0);
    
    [Tooltip("Skala bayangan (untuk efek gepeng)")]
    [SerializeField] private Vector3 shadowScale = new Vector3(1f, 0.6f, 1f);

    private SpriteRenderer myRenderer;

    void Awake()
    {
        myRenderer = GetComponent<SpriteRenderer>();
        
        // Cari player otomatis jika lupa di-drag
        if (playerRenderer == null)
            playerRenderer = GetComponentInParent<SpriteRenderer>();
            
        // Terapkan warna dan material awal
        myRenderer.color = shadowColor;
    }

    void LateUpdate()
    {
        if (playerRenderer == null) return;

        // 1. Salin Sprite (Ini kuncinya! Animasi ikut tercopy)
        myRenderer.sprite = playerRenderer.sprite;

        // 2. Salin Arah Hadap (Flip)
        myRenderer.flipX = playerRenderer.flipX;
        
        // 3. Paksa Posisi & Skala (agar tetap di bawah kaki)
        transform.localPosition = offset;
        transform.localScale = shadowScale;
        
        // 4. Pastikan warna tetap hitam transparan (karena kadang sprite asli meresetnya)
        myRenderer.color = shadowColor;
    }
}