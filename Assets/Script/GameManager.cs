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
    [SerializeField] private List<string> anomalySceneNames;
    [SerializeField] private string endingRoomName = "EndingRoom"; 

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
    
    [Header("Transition Settings")]
    [SerializeField] private float autoWalkSpeed = 3f;
    [SerializeField] private float autoWalkDuration = 1f;
    
    private bool isThisSceneAnomaly = false;
    private bool isTransitioning = false; 

    void Awake()
    {
        if (isPrologRoom)
        {
            GameData.currentHour = 0;
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
        if (isPrologRoom)
        {
            if (player != null) player.transform.localScale = new Vector3(prologPlayerScale.x, prologPlayerScale.y, 1f);
        }
        else
        {
            if (player != null) player.transform.localScale = new Vector3(normalPlayerScale.x, normalPlayerScale.y, 1f);
            
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
            if (allClocksInScene != null)
            {
                foreach (BackgroundClock clock in allClocksInScene)
                    if(clock != null) clock.gameObject.SetActive(false);
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
            GameData.isAnomalyPresent = (Random.value < 0.5f);
            
            if (GameData.isAnomalyPresent)
            {
                if (anomalySceneNames == null || anomalySceneNames.Count == 0)
                {
                    Debug.LogError("Daftar Anomaly Scene kosong!");
                    sceneToLoad = normalSceneName; 
                }
                else
                {
                    int randomIndex = Random.Range(0, anomalySceneNames.Count);
                    sceneToLoad = anomalySceneNames[randomIndex];
                }
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