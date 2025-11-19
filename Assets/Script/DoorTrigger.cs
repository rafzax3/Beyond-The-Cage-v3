using UnityEngine;

// Versi Baru: Tahu jika dia ada di EndingRoom
public class DoorTrigger : MonoBehaviour
{
    [Tooltip("ID untuk pintu ini ('Left' atau 'Right')")]
    public string doorID;

    // Referensi
    private GameManager gameManagerInScene;
    private FinalTrigger finalTriggerInScene;
    private Collider2D doorCollider;

    void Start()
    {
        // Cari "Otak" yang ada di scene ini
        gameManagerInScene = FindObjectOfType<GameManager>();
        finalTriggerInScene = FindObjectOfType<FinalTrigger>();
        
        doorCollider = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Matikan collider ini agar tidak ter-trigger berkali-kali
            if (doorCollider != null) doorCollider.enabled = false;
            
            // Cek: Apakah kita di scene lorong loop biasa?
            if (gameManagerInScene != null)
            {
                gameManagerInScene.PlayerHitDoor(doorID);
            }
            // Cek: Apakah kita di scene ending?
            else if (finalTriggerInScene != null)
            {
                // --- INI PERUBAHANNYA ---
                // Beri tahu FinalTrigger PINTU MANA yang kita masuki
                finalTriggerInScene.GoToTheEnd(doorID);
                // -------------------------
            }
        }
    }
}