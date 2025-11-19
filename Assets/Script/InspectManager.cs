using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

// VERSI SEDERHANA (Hanya bekerja dengan 1 skrip InteractableObject)
// PERBAIKAN: Fungsi duplikat telah dihapus
public class InspectManager : MonoBehaviour
{
    public static InspectManager instance;

    [Header("Referensi UI")]
    [SerializeField] private GameObject inspectCanvas;
    [SerializeField] private Image inspectImage;
    [SerializeField] private TextMeshProUGUI inspectText;
    [SerializeField] private CanvasGroup inspectCanvasGroup;
    
    [Header("Audio")]
    [SerializeField] private AudioSource inspectSFXSource;

    [Header("Pengaturan Efek")]
    [SerializeField] private float fadeDuration = 0.3f; 
    [SerializeField] private float typingSpeed = 0.05f; 

    // (Variabel Status Internal)
    private PlayerMovement playerMovement;
    private bool isInspecting = false;
    private bool isTransitioning = false; 
    private bool isTyping = false;
    private InteractableObject currentInteractable;
    private Coroutine typingCoroutine;
    private string[] currentPages;
    private int currentPageIndex;
    private string currentPageFullText;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (inspectCanvasGroup == null && inspectCanvas != null)
            inspectCanvasGroup = inspectCanvas.GetComponent<CanvasGroup>();
        
        if (inspectSFXSource == null)
            inspectSFXSource = GetComponent<AudioSource>();

        if (inspectCanvas != null) inspectCanvas.SetActive(true); 
        if (inspectCanvasGroup != null)
        {
            inspectCanvasGroup.alpha = 0;
            inspectCanvasGroup.blocksRaycasts = false;
        }
    }

    // --- FUNGSI PUBLIK (VERSI SEDERHANA) ---
    public bool IsInspecting() 
    { 
        return isInspecting || isTransitioning; 
    }

    public void SetCurrentInteractable(InteractableObject interactable)
    {
        currentInteractable = interactable;
        if (!isInspecting && currentInteractable != null)
        {
            currentInteractable.ShowPrompt(true);
        }
    }

    public void ClearCurrentInteractable(InteractableObject interactable)
    {
        if (currentInteractable == interactable)
        {
            if (currentInteractable != null)
            {
                currentInteractable.ShowPrompt(false);
            }
            currentInteractable = null;
        }
    }
    // ------------------------------------

    void Update()
    {
        if (PauseMenu.instance != null && PauseMenu.instance.IsGamePaused())
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (isTransitioning) return;

            if (isInspecting)
            {
                // Jika sedang inspeksi, lanjutkan/tutup
                if (isTyping)
                {
                    if(typingCoroutine != null) StopCoroutine(typingCoroutine);
                    inspectText.text = currentPageFullText;
                    isTyping = false;
                }
                else
                {
                    AdvancePageOrHide();
                }
            }
            else if (currentInteractable != null)
            {
                // Mulai inspeksi
                // Cari PlayerMovement HANYA jika kita belum menyimpannya
                if (playerMovement == null) playerMovement = FindObjectOfType<PlayerMovement>();
                StartCoroutine(FadeInInspect(currentInteractable));
            }
        }
    }
    
    private void AdvancePageOrHide()
    {
        if (currentPages != null && (currentPageIndex + 1) < currentPages.Length)
        {
            currentPageIndex++;
            currentPageFullText = currentPages[currentPageIndex];
            typingCoroutine = StartCoroutine(TypeText(currentPageFullText));
        }
        else
        {
            StartCoroutine(FadeOutInspect());
        }
    }

    private IEnumerator FadeInInspect(InteractableObject objectToInspect)
    {
        isTransitioning = true;
        isInspecting = true;
        
        if(currentInteractable != null) currentInteractable.ShowPrompt(false);

        // --- PERBAIKAN BUG AUDIO ---
        // Gunakan SetLock (via State Machine) alih-alih .enabled = false
        if (playerMovement != null)
        {
            // Kunci player, tapi pertahankan arah hadapnya saat ini
            playerMovement.SetLock(true, playerMovement.IsFacingRight()); 
            
            // Atur animasi ke idle (ini aman)
            Animator playerAnim = playerMovement.GetComponent<Animator>();
            if (playerAnim != null) playerAnim.SetInteger("moveState", 0);
        }
        // -------------------------

        // Ambil data statis langsung dari objek
        Sprite spriteToShow = objectToInspect.inspectSprite;
        currentPages = objectToInspect.inspectPages;
        AudioClip soundToPlay = objectToInspect.inspectSound;

        // Putar suara
        if (inspectSFXSource != null && soundToPlay != null)
        {
            inspectSFXSource.PlayOneShot(soundToPlay);
        }

        // Tampilkan gambar
        inspectImage.sprite = spriteToShow;
        inspectImage.enabled = (spriteToShow != null);
        
        // (Sisa kode setup teks tidak berubah)
        currentPageIndex = 0;
        if (currentPages != null && currentPages.Length > 0 && !string.IsNullOrEmpty(currentPages[0]))
        {
            inspectText.gameObject.SetActive(true);
            currentPageFullText = currentPages[0];
            inspectText.text = "";
        }
        else
        {
            inspectText.gameObject.SetActive(false);
            currentPageFullText = "";
            inspectText.text = "";
        }

        // Mulai Fade In
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.unscaledDeltaTime;
            inspectCanvasGroup.alpha = Mathf.Lerp(0, 1, timer / fadeDuration);
            yield return null;
        }
        inspectCanvasGroup.alpha = 1;
        inspectCanvasGroup.blocksRaycasts = true; 
        isTransitioning = false;

        // Mulai Tampilkan Teks
        if (currentPages != null && currentPages.Length > 0 && !string.IsNullOrEmpty(currentPages[0]))
        {
            typingCoroutine = StartCoroutine(TypeText(currentPageFullText));
        }
    }

    private IEnumerator FadeOutInspect() 
    { 
        isTransitioning = true;
        isInspecting = false;
        if (isTyping)
        {
            if(typingCoroutine != null) StopCoroutine(typingCoroutine);
            isTyping = false;
        }
        if(inspectSFXSource != null) inspectSFXSource.Stop();
        
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.unscaledDeltaTime; 
            inspectCanvasGroup.alpha = Mathf.Lerp(1, 0, timer / fadeDuration);
            yield return null;
        }
        inspectCanvasGroup.alpha = 0;
        inspectCanvasGroup.blocksRaycasts = false;
        isTransitioning = false;

        // --- PERBAIKAN BUG AUDIO ---
        // Kembalikan kontrol ke player menggunakan SetLock
        if (playerMovement != null)
        {
            playerMovement.SetLock(false, playerMovement.IsFacingRight());
        }
        // -------------------------
        
        if (currentInteractable != null)
        {
            currentInteractable.ShowPrompt(true);
        }
    }
    
    private IEnumerator TypeText(string textToType) 
    { 
        isTyping = true;
        inspectText.text = "";
        
        float timer = 0;
        int charIndex = 0;
        while (charIndex < textToType.Length)
        {
            timer += Time.unscaledDeltaTime; 
            if (typingSpeed <= 0)
            {
                inspectText.text = textToType;
                break;
            }

            if (timer >= typingSpeed)
            {
                timer = 0;
                charIndex++;
                inspectText.text = textToType.Substring(0, charIndex);
            }
            yield return null;
        }
        isTyping = false;
    }

    public void CancelInspect() 
    { 
        if (!isInspecting && !isTransitioning) return;

        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        
        // Hentikan coroutine yang mungkin sedang berjalan
        StopAllCoroutines(); 
        
        isInspecting = false;
        isTransitioning = false;
        isTyping = false;
        
        if(inspectSFXSource != null) inspectSFXSource.Stop();

        if (inspectCanvasGroup != null)
        {
            inspectCanvasGroup.alpha = 0;
            inspectCanvasGroup.blocksRaycasts = false;
        }

        if (playerMovement != null)
        {
            // Pastikan kita juga menggunakan SetLock di sini!
            playerMovement.SetLock(false, playerMovement.IsFacingRight());
        }
    }

    // --- BLOK FUNGSI DUPLIKAT TELAH DIHAPUS DARI SINI ---
}