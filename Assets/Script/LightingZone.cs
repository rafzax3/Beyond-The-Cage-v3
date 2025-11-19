using UnityEngine;

// DITEMPEL DI OBJEK TRIGGER KOSONG DI DALAM SCENE
[RequireComponent(typeof(BoxCollider2D))]
public class LightingZone : MonoBehaviour
{
    [Header("Pengaturan Zona")]
    [Tooltip("Warna yang akan diterapkan ke player saat masuk zona ini")]
    [SerializeField] private Color zoneColor = Color.white; // Default: Terang Penuh

    [Tooltip("Seberapa cepat player berubah warna (detik)")]
    [SerializeField] private float fadeDuration = 0.5f;

    [Tooltip("Centang ini jika zona ini mengatur 'cahaya ambient' baru (cahaya default)")]
    [SerializeField] private bool isAmbientZone = false;

    void Awake()
    {
        // Pastikan collider ini adalah Trigger
        GetComponent<BoxCollider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerLightReceiver receiver = other.GetComponent<PlayerLightReceiver>();
            if (receiver != null)
            {
                // Beri tahu player untuk mengubah warnanya
                receiver.SetTargetColor(zoneColor, fadeDuration, isAmbientZone);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Jika ini BUKAN zona ambient, kembalikan warna player ke default
        if (!isAmbientZone && other.CompareTag("Player"))
        {
            PlayerLightReceiver receiver = other.GetComponent<PlayerLightReceiver>();
            if (receiver != null)
            {
                // Beri tahu player untuk kembali ke warna ambient
                receiver.ReturnToAmbient(fadeDuration);
            }
        }
    }
}