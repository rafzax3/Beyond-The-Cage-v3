using UnityEngine;
using TMPro; 
using UnityEngine.SceneManagement; 
using System.Collections.Generic;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [Header("Tipe Lorong")]
    [SerializeField] private bool isPrologRoom = false;
    
    [SerializeField] private string nextPrologSceneName = "SecondRoom";

    [Header("Pengaturan Ukuran Player")]
    [SerializeField] private Vector2 prologPlayerScale = new Vector2(1.5f, 1.5f);
    [SerializeField] private Vector2 normalPlayerScale = new Vector2(1f, 1f);
    
    [Header("Scene Names")]
    [SerializeField] private string normalSceneName = "Lorong_Normal";
    [SerializeField] private string endingRoomName = "EndingRoom"; 

    // --- FITUR BARU: KATEGORI KESULITAN ---
    [Header("Daftar Scene Anomali (Per Kesulitan)")]
    [Tooltip("Anomali untuk Jam 12 & 1")]
    [SerializeField] private List<string> easyAnomalies;
    [Tooltip("Anomali untuk Jam 2 & 3")]
    [SerializeField] private List<string> mediumAnomalies;
    [Tooltip("Anomali untuk Jam 4 & 5")]
    [SerializeField] private List<string> hardAnomalies;
    // --------------------------------------

    [Header("Scene References")]
    [SerializeField] private GameObject player; 
    [SerializeField] private PlayerMovement playerMovement; 
    [SerializeField] private Animator playerAnimator; 
    [SerializeField] private Transform spawnPointLeft; 
    [SerializeField] private Transform spawnPointRight; 
    [SerializeField] private TextMeshProUGUI loopCounterText; 
    [SerializeField] private List<BackgroundClock> allClocksInScene;

    [Header("Game Rules")]
    [SerializeField] private int finalHour = 6; 
    
    [Header("Probabilitas Dinamis")]
    [Tooltip("Berapa % peluang normal turun setiap kali dapat normal? (0.15 = 15%)")]
    [SerializeField] private float probabilityReductionPerStack = 0.15f;
    [Tooltip("Maksimal stack pengurangan (2x = 30%)")]
    [SerializeField] private int maxProbabilityStack = 2;

    [Header("Audio Khusus")]
    [SerializeField] private AudioClip startSceneSfx; 

    [Header("Transition Settings")]
    [SerializeField] private float autoWalkSpeed = 3f;
    [SerializeField] private float autoWalkDuration = 1f;
    
    private bool isThisSceneAnomaly = false;
    private bool isTransitioning = false; 
    private AudioSource sfxSource; 

    void Awake()
    {
        sfxSource = GetComponent<AudioSource>();

        if (isPrologRoom)
        {
            string currentScene = SceneManager.GetActiveScene().name;
            if (currentScene == "PrologRoom") 
            {
                GameData.ResetAllData(); 
            }
            
            GameData.spawnPlayerFromLeft = true;
            GameData.isAnomalyPresent = false;
            isThisSceneAnomaly = false; 
        }
        else
        {
            isThisSceneAnomaly = GameData.isAnomalyPresent;
        }

        if (GameData.spawnPlayerFromLeft)
        {
            player.transform.position = spawnPointLeft.position;
            if (playerMovement != null) playerMovement.SetLock(true, true); 
        }
        else
        {
            player.transform.position = spawnPointRight.position;
            if (playerMovement != null) playerMovement.SetLock(true, false); 
        }
    }

    void Start()
    {
        if (sfxSource != null && startSceneSfx != null)
        {
            sfxSource.PlayOneShot(startSceneSfx);
        }

        if (isPrologRoom)
        {
            if (player != null) player.transform.localScale = new Vector3(prologPlayerScale.x, prologPlayerScale.y, 1f);
            
            string currentScene = SceneManager.GetActiveScene().name;
            if (currentScene == "PrologRoom") 
            {
                if (playerMovement != null) playerMovement.canRun = false; 
            }
            else
            {
                if (playerMovement != null) playerMovement.canRun = true; 
            }
        }
        else
        {
            if (player != null) player.transform.localScale = new Vector3(normalPlayerScale.x, normalPlayerScale.y, 1f);
            if (playerMovement != null) playerMovement.canRun = true; 

            CameraFollow mainCameraFollow = FindObjectOfType<CameraFollow>();
            if (mainCameraFollow != null && player != null) mainCameraFollow.target = player.transform;
        }

        if (MusicManager.instance != null)
        {
            MusicManager.instance.PlayGameplayMusic();
        }

        if (isPrologRoom)
        {
            if (loopCounterText != null) loopCounterText.gameObject.SetActive(false);
            
            bool showClock = (SceneManager.GetActiveScene().name == "SecondRoom"); 

            if (allClocksInScene != null)
            {
                foreach (BackgroundClock clock in allClocksInScene)
                {
                    if (clock != null) 
                    {
                        clock.gameObject.SetActive(showClock);
                        if(showClock) clock.SetClockHour(0); 
                    }
                }
            }
        }
        else
        {
            if (loopCounterText != null) loopCounterText.gameObject.SetActive(true);
            UpdateLoopUI(GameData.currentHour);

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
        }
        
        StartCoroutine(Sequence_PlayerWalkIn());
    }

    private IEnumerator Sequence_PlayerWalkIn()
    {
        float walkDirection = GameData.spawnPlayerFromLeft ? 1f : -1f;
        
        StartCoroutine(FadeManager.instance.FadeIn());
        
        if (playerMovement != null) playerMovement.SetScriptedAnimation(1); 

        float timer = 0f;
        while (timer < autoWalkDuration)
        {
            player.transform.Translate(new Vector3(walkDirection * autoWalkSpeed * Time.deltaTime, 0, 0));
            timer += Time.deltaTime;
            yield return null;
        }
        
        if (playerMovement != null) playerMovement.SetScriptedAnimation(0); 
        
        if (playerMovement != null) 
            playerMovement.SetLock(false, GameData.spawnPlayerFromLeft);
            
        if (TutorialManager.instance != null)
        {
            string currentScene = SceneManager.GetActiveScene().name;

            if (currentScene == "PrologRoom")
            {
                TutorialManager.instance.TriggerWalkTutorial();
            }
            else if (currentScene == "FirstRoom")
            {
                TutorialManager.instance.TriggerRunTutorial();
            }
            else if (!isPrologRoom && GameData.currentHour == 1)
            {
                if (!GameData.tutorialLetterShown)
                {
                    TutorialManager.instance.PlayMeowSound();
                }
            }
        }
    }

    public void PlayerHitDoor(string doorID)
    {
        if (isTransitioning) return;
        isTransitioning = true;
        
        bool faceRight = (doorID == "Right");
        if (playerMovement != null)
        {
            playerMovement.SetLock(true, faceRight); 
        }
        
        StartCoroutine(Sequence_PlayerWalkOut(doorID));
    }
    
    private IEnumerator Sequence_PlayerWalkOut(string doorID)
    {
        if (TutorialManager.instance != null)
        {
            TutorialManager.instance.HideAllTutorials();
        }

        float walkDirection = (doorID == "Right") ? 1f : -1f;
        
        if(playerMovement != null) playerMovement.SetScriptedAnimation(1);

        StartCoroutine(FadeManager.instance.FadeOut());
        
        float timer = 0f;
        while (timer < autoWalkDuration)
        {
            player.transform.Translate(new Vector3(walkDirection * autoWalkSpeed * Time.deltaTime, 0, 0));
            timer += Time.deltaTime;
            yield return null;
        }
        
        if (isPrologRoom)
        {
            if (doorID == "Right") 
            {
                GameData.spawnPlayerFromLeft = true; 
                if (nextPrologSceneName == normalSceneName) GameData.currentHour = 0; 
                
                SceneManager.LoadScene(nextPrologSceneName);
            }
            yield break; 
        }
        
        bool correctChoice = false;
        if (doorID == "Right") correctChoice = !isThisSceneAnomaly; 
        else if (doorID == "Left") correctChoice = isThisSceneAnomaly; 

        if (doorID == "Right") GameData.spawnPlayerFromLeft = true;
        else GameData.spawnPlayerFromLeft = false;

        if (correctChoice)
        {
            // --- LOGIKA UNLOCK ACHIEVEMENT (BARU) ---
            // Jika kita berada di ruangan ANOMALY (dan kita memilih benar/kanan),
            // maka kita baru saja "mengalahkan" anomali tersebut.
            if (isThisSceneAnomaly)
            {
                // Gunakan nama scene saat ini sebagai ID
                string currentSceneID = SceneManager.GetActiveScene().name;
                GameData.UnlockAnomaly(currentSceneID);
            }
            // ----------------------------------------

            GameData.currentHour++;
            if (GameData.currentHour == finalHour)
            {
                LoadNextHallway(true);
                yield break;
            }
        }
        else
        {
            GameData.currentHour = 0; 
        }
        
        LoadNextHallway(false);
    }
    
    void LoadNextHallway(bool loadEnding)
    {
        string sceneToLoad;

        if (loadEnding)
        {
            sceneToLoad = endingRoomName;
        }
        else 
        {
            // --- LOGIKA PROBABILITAS DINAMIS ---
            
            // Peluang dasar Anomaly = 50% (0.5)
            float anomalyChance = 0.5f;
            
            // Tambahkan peluang berdasarkan stack normal
            // Semakin banyak normal berturut-turut, semakin tinggi peluang anomali
            // (Peluang Normal turun = Peluang Anomali naik)
            int stacks = Mathf.Clamp(GameData.consecutiveNormalCount, 0, maxProbabilityStack);
            float extraChance = stacks * probabilityReductionPerStack;
            
            anomalyChance += extraChance; // Misal: 0.5 + 0.15 = 0.65 (65% Anomali)

            // Roll dadu
            GameData.isAnomalyPresent = (Random.value < anomalyChance);

            // Reset atau Tambah Stack
            if (GameData.isAnomalyPresent)
            {
                GameData.consecutiveNormalCount = 0; // Reset jika dapat anomali
            }
            else
            {
                GameData.consecutiveNormalCount++; // Tambah stack jika dapat normal
            }
            // -----------------------------------

            if (GameData.isAnomalyPresent)
            {
                // --- LOGIKA PEMILIHAN TINGKAT KESULITAN ---
                List<string> chosenList = new List<string>();

                // Pilih list berdasarkan jam
                // Jam 12 (0) & 1 -> Easy
                if (GameData.currentHour <= 1) 
                {
                    chosenList = easyAnomalies;
                }
                // Jam 2 & 3 -> Medium
                else if (GameData.currentHour <= 3) 
                {
                    chosenList = mediumAnomalies;
                }
                // Jam 4 & 5 -> Hard
                else 
                {
                    chosenList = hardAnomalies;
                }

                // Fallback: Jika list terpilih kosong (misal belum diisi), 
                // gabungkan dengan list yang lebih mudah agar game tidak error
                if (chosenList == null || chosenList.Count == 0)
                {
                    if (mediumAnomalies != null) chosenList.AddRange(mediumAnomalies);
                    if (easyAnomalies != null) chosenList.AddRange(easyAnomalies);
                }

                // Load
                if (chosenList == null || chosenList.Count == 0)
                {
                    Debug.LogError("SEMUA Daftar Anomaly Scene kosong! Memuat normal scene.");
                    sceneToLoad = normalSceneName; 
                }
                else
                {
                    int randomIndex = Random.Range(0, chosenList.Count);
                    sceneToLoad = chosenList[randomIndex];
                }
                // ------------------------------------------
            }
            else
            {
                sceneToLoad = normalSceneName;
            }
        } 
        
        SceneManager.LoadScene(sceneToLoad);
    }

    void UpdateLoopUI(int hour)
    {
        if (loopCounterText != null)
        {
            int displayHour = (hour == 0) ? 12 : hour;
            if(hour >= finalHour) displayHour = finalHour; 
            
            loopCounterText.text = "Jam: " + displayHour.ToString("D2") + ":00";
        }
    }
}