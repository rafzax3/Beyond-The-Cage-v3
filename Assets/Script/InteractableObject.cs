using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    [Header("UI Prompt")]
    [SerializeField] private GameObject promptUI; 

    [Header("Inspect Data (Default)")]
    public Sprite inspectSprite; 
    
    [Header("Audio")]
    public AudioClip inspectSound; 

    [Header("Gambar Inspeksi (Opsional Per Jam)")]
    public Sprite sprite_Hour0;
    public Sprite sprite_Hour1;
    public Sprite sprite_Hour2;
    public Sprite sprite_Hour3;
    public Sprite sprite_Hour4;
    public Sprite sprite_Hour5;
    public Sprite sprite_Hour6;

    [Header("Teks Inspeksi (Per Jam)")]
    [TextArea(3, 5)] public string[] pages_Hour0;
    [TextArea(3, 5)] public string[] pages_Hour1;
    [TextArea(3, 5)] public string[] pages_Hour2;
    [TextArea(3, 5)] public string[] pages_Hour3;
    [TextArea(3, 5)] public string[] pages_Hour4;
    [TextArea(3, 5)] public string[] pages_Hour5;
    [TextArea(3, 5)] public string[] pages_Hour6;

    private Collider2D myCollider;

    void Reset() 
    { 
        SetupComponents(); 
    }

    void Awake()
    {
        SetupComponents();
        if (promptUI != null) promptUI.SetActive(false);
    }

    void SetupComponents()
    {
        myCollider = GetComponent<Collider2D>();
        if (myCollider == null)
        {
            myCollider = gameObject.AddComponent<BoxCollider2D>();
        }
        
        myCollider.isTrigger = true;
    }

    public string[] GetPagesForCurrentHour(int hour)
    {
        switch (hour)
        {
            case 0:  return pages_Hour0;
            case 1:  return pages_Hour1;
            case 2:  return pages_Hour2;
            case 3:  return pages_Hour3;
            case 4:  return pages_Hour4;
            case 5:  return pages_Hour5;
            case 6:  return pages_Hour6;
            default: return pages_Hour0;
        }
    }

    public Sprite GetSpriteForCurrentHour(int hour)
    {
        Sprite specificSprite = null;
        switch (hour)
        {
            case 0:  specificSprite = sprite_Hour0; break;
            case 1:  specificSprite = sprite_Hour1; break;
            case 2:  specificSprite = sprite_Hour2; break;
            case 3:  specificSprite = sprite_Hour3; break;
            case 4:  specificSprite = sprite_Hour4; break;
            case 5:  specificSprite = sprite_Hour5; break;
            case 6:  specificSprite = sprite_Hour6; break;
        }

        if (specificSprite != null)
        {
            return specificSprite;
        }
        
        return inspectSprite; 
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
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