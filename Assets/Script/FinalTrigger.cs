using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic; // Untuk List

public class FinalTrigger : MonoBehaviour
{
    [Header("Pengaturan Ending")]
    [Tooltip("Nama scene 'Main Menu'")]
    [SerializeField] private string mainMenuSceneName = "MainMenu"; 
    
    [Tooltip("Durasi fade putih menuju 'Main Menu' (misal: 2 detik)")]
    [SerializeField] private float whiteFadeDuration = 2f; 
    
    [Header("Pengaturan Auto-Walk")]
    [Tooltip("Seret (drag) Player dari scene ini")]
    [SerializeField] private GameObject player;
    [Tooltip("Seret (drag) SpawnPoint_Left dari scene ini")]
    [SerializeField] private Transform spawnPointLeft;
    [Tooltip("Seret (drag) SpawnPoint_Right dari scene ini")]
    [SerializeField] private Transform spawnPointRight; 

    [SerializeField] private float autoWalkSpeed = 3f;
    [SerializeField] private float autoWalkDuration = 1f;

    [Tooltip("Ukuran player normal (misal: 1, 1). Samakan dengan GameManager.")]
    [SerializeField] private Vector2 normalPlayerScale = new Vector2(1f, 1f);

    [Header("Scene References")]
    [Tooltip("Seret (drag) SEMUA jam di scene ini ke sini")]
    [SerializeField] private List<BackgroundClock> allClocksInScene;

    [Header("Door Colliders")]
    [Tooltip("Seret (drag) objek Pintu KIRI (yang punya DoorTrigger.cs) ke sini")]
    [SerializeField] private Collider2D doorTriggerLeft;
    [Tooltip("Seret (drag) objek Pintu KANAN (yang punya DoorTrigger.cs) ke sini")]
    [SerializeField] private Collider2D doorTriggerRight;

    // Referensi Internal
    private PlayerMovement playerMovement;
    private Animator playerAnimator;
    private bool isTransitioning = false;

    // --- FUNGSI AWAKE (DIPERBARUI) ---
    void Awake()
    {
        Debug.Log("Ending Room: Memaksa spawn di 'SpawnPoint_Left'");

        if (player != null)
        {
            player.transform.position = spawnPointLeft.position;
            playerMovement = player.GetComponent<PlayerMovement>();
            playerAnimator = player.GetComponent<Animator>();
        }
        else Debug.LogError("Player belum di-set di FinalTrigger!");

        bool shouldFaceRight = true; 
        if (playerMovement != null)
        {
            playerMovement.ForceFaceDirection(shouldFaceRight);
            playerMovement.SetLock(true, shouldFaceRight);
            
            // --- PERBAIKAN BUG (Bagian 1) ---
            // Matikan skrip player AGAR TIDAK "BERKELAHI" dengan auto-walk
            playerMovement.enabled = false;
            // ---------------------------------
        }
    }

    // (Fungsi Start tidak berubah)
    void Start()
    {
        if (MusicManager.instance != null)
        {
            MusicManager.instance.PlayEndingMusic();
        }

        if (player != null)
        {
            player.transform.localScale = new Vector3(normalPlayerScale.x, normalPlayerScale.y, 1f);
        }
        CameraFollow mainCameraFollow = FindObjectOfType<CameraFollow>();
        if (mainCameraFollow != null && player != null)
        {
            mainCameraFollow.target = player.transform;
        }
        else if(mainCameraFollow == null)
        {
            Debug.LogWarning("Anda lupa menambahkan 'CameraFollow.cs' ke Main Camera di EndingRoom!");
        }

        if (allClocksInScene != null)
        {
            foreach (BackgroundClock clock in allClocksInScene)
            {
                if (clock != null)
                {
                    clock.gameObject.SetActive(true);
                    clock.SetClockHour(GameData.currentHour); 
                }
            }
        }
        
        Debug.Log("Ending Room: Mematikan pintu kanan, hanya pintu kiri yang aktif.");
        if (doorTriggerRight != null)
        {
            doorTriggerRight.enabled = false;
        }
        if (doorTriggerLeft != null)
        {
            doorTriggerLeft.enabled = true;
        }

        StartCoroutine(Sequence_PlayerWalkIn());
    }

    // --- FUNGSI INI DIPERBARUI ---
    private IEnumerator Sequence_PlayerWalkIn()
    {
        // (Logika walkDirection = 1f sudah benar)
        float walkDirection = 1f; 
        
        // PERBAIKAN BUG: Animasi akan berjalan sekarang
        // karena skrip PlayerMovement (yang mungkin menimpanya) sudah dimatikan.
        if (playerAnimator != null) playerAnimator.SetInteger("moveState", 1); 
        
        if (FadeManager.instance != null)
        {
            yield return StartCoroutine(FadeManager.instance.FadeIn());
        }

        // PERBAIKAN BUG: Gerakan akan mulus sekarang
        // karena tidak ada skrip lain yang "berkelahi".
        float timer = 0f;
        while (timer < autoWalkDuration)
        {
            player.transform.Translate(new Vector3(walkDirection * autoWalkSpeed * Time.deltaTime, 0, 0));
            timer += Time.deltaTime;
            yield return null;
        }
        if(playerAnimator != null) playerAnimator.SetInteger("moveState", 0); 
        
        if (playerMovement != null)
        {
            // --- PERBAIKAN BUG (Bagian 2) ---
            // Nyalakan kembali skrip player
            playerMovement.enabled = true;
            // ---------------------------------
            
            // Beri dia kontrol
            playerMovement.SetLock(false, true); 
        }
    }
    
    // (Fungsi GoToTheEnd tidak berubah)
    public void GoToTheEnd(string doorID)
    {
        if (isTransitioning) return;
        isTransitioning = true;

        bool faceRight = (doorID == "Right");
        
        // --- PERBAIKAN BUG (Bagian 3) ---
        // Kita harus menyalakan skripnya lagi agar bisa di-lock!
        if (playerMovement != null)
        {
            playerMovement.enabled = true; 
            playerMovement.SetLock(true, faceRight); 
        }
        // ---------------------------------
        
        StartCoroutine(Sequence_PlayerWalkOut(doorID));
    }
    
    // (Fungsi Sequence_PlayerWalkOut tidak berubah)
    private IEnumerator Sequence_PlayerWalkOut(string doorID)
    {
        float walkDirection = (doorID == "Right") ? 1f : -1f;
        
        if(playerAnimator != null) playerAnimator.SetInteger("moveState", 1);

        if (MusicManager.instance != null)
        {
            MusicManager.instance.PlayMenuMusic();
        }

        if (FadeManager.instance != null)
        {
            StartCoroutine(FadeManager.instance.FadeOut(Color.white, whiteFadeDuration));
        }
        
        float timer = 0f;
        while (timer < autoWalkDuration) 
        {
            player.transform.Translate(new Vector3(walkDirection * autoWalkSpeed * Time.deltaTime, 0, 0));
            timer += Time.deltaTime;
            yield return null;
        }

        float sisaWaktuFade = whiteFadeDuration - autoWalkDuration;
        if (sisaWaktuFade > 0)
        {
            yield return new WaitForSeconds(sisaWaktuFade);
        }

        SceneManager.LoadScene(mainMenuSceneName);
    }
}