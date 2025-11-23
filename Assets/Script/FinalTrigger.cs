using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic; 

public class FinalTrigger : MonoBehaviour
{
    [Header("Pengaturan Ending")]
    [SerializeField] private string endingTextSceneName = "EndingTextScene"; 
    
    [SerializeField] private float whiteFadeDuration = 2f; 
    
    [Header("Pengaturan Auto-Walk")]
    [SerializeField] private GameObject player;
    [SerializeField] private Transform spawnPointLeft;
    [SerializeField] private Transform spawnPointRight; 

    [SerializeField] private float autoWalkSpeed = 3f;
    [SerializeField] private float autoWalkDuration = 1f;

    [SerializeField] private Vector2 normalPlayerScale = new Vector2(1f, 1f);

    [Header("Scene References")]
    [SerializeField] private List<BackgroundClock> allClocksInScene;
    [SerializeField] private Collider2D doorTriggerLeft;
    [SerializeField] private Collider2D doorTriggerRight;

    private PlayerMovement playerMovement;
    private Animator playerAnimator;

    void Awake()
    {
        if (player != null)
        {
            player.transform.position = spawnPointLeft.position;
            playerMovement = player.GetComponent<PlayerMovement>();
            playerAnimator = player.GetComponent<Animator>();
        }

        if (playerMovement != null)
        {
            playerMovement.ForceFaceDirection(true); 
            playerMovement.SetLock(true, true);
        }
    }

    void Start()
    {
        if (MusicManager.instance != null)
        {
            MusicManager.instance.PlayEndingMusic();
        }

        if (player != null) player.transform.localScale = new Vector3(normalPlayerScale.x, normalPlayerScale.y, 1f);
        
        CameraFollow mainCameraFollow = FindObjectOfType<CameraFollow>();
        if (mainCameraFollow != null && player != null) mainCameraFollow.target = player.transform;

        if (allClocksInScene != null)
        {
            foreach (BackgroundClock clock in allClocksInScene)
                if (clock != null) { clock.gameObject.SetActive(true); clock.SetClockHour(GameData.currentHour); }
        }
        
        if (doorTriggerRight != null) doorTriggerRight.enabled = false;
        if (doorTriggerLeft != null) doorTriggerLeft.enabled = true;

        StartCoroutine(Sequence_PlayerWalkIn());
    }

    private IEnumerator Sequence_PlayerWalkIn()
    {
        float walkDirection = 1f; 
        
        if (playerMovement != null) playerMovement.SetScriptedAnimation(1);
        
        if (FadeManager.instance != null) yield return StartCoroutine(FadeManager.instance.FadeIn());

        float timer = 0f;
        while (timer < autoWalkDuration)
        {
            player.transform.Translate(new Vector3(walkDirection * autoWalkSpeed * Time.deltaTime, 0, 0));
            timer += Time.deltaTime;
            yield return null;
        }
        
        if (playerMovement != null) playerMovement.SetScriptedAnimation(0);
        
        if (playerMovement != null)
        {
            playerMovement.SetLock(false, true); 
        }
    }
    
    public void GoToTheEnd(string doorID)
    {
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
        
        if (playerMovement != null) playerMovement.SetScriptedAnimation(1);

        if (MusicManager.instance != null)
        {
            MusicManager.instance.PlayEndingMusic();
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
        if (sisaWaktuFade > 0) yield return new WaitForSeconds(sisaWaktuFade);

        SceneManager.LoadScene(endingTextSceneName);
    }
}