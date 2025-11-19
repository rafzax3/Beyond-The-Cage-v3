using UnityEngine;
using System.Collections; // Diperlukan untuk Coroutine

// Versi Final: Menggunakan State Machine (Aktor Cerdas)
// Menangani 'Turn' dan 'StartRun' serta 'Lock' dari GameManager
public class PlayerMovement : MonoBehaviour
{
    // State internal untuk mengunci input
    private enum ControlState { PlayerControl, IsTurning, LockedByManager }
    private ControlState currentState = ControlState.LockedByManager; // Mulai terkunci
    
    [Header("Referensi Komponen")]
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer; 
    private Animator anim; 

    [Header("Pengaturan Gerak")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 8f; 
    
    [Header("Audio")]
    [Tooltip("Komponen AudioSource di player ini")]
    [SerializeField] private AudioSource sfxSource;
    [Tooltip("SFX untuk berjalan")]
    [SerializeField] private AudioClip walkSfx;
    [Tooltip("SFX untuk berlari")]
    [SerializeField] private AudioClip runSfx;

    // Variabel privat
    private float horizontalInput;
    private bool isFacingRight = false; 
    
    // Variabel HASH (optimasi)
    private int animHash_Turn = Animator.StringToHash("CatTurn"); 
    private int animHash_moveState = Animator.StringToHash("moveState");
    private int animHash_doTurn = Animator.StringToHash("doTurn");


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>(); 
        spriteRenderer = GetComponent<SpriteRenderer>(); 
        
        if (sfxSource == null)
        {
            sfxSource = GetComponent<AudioSource>();
        }
    }

    void Update()
    {
        // 1. Dapatkan Input Arah
        horizontalInput = Input.GetAxisRaw("Horizontal");
        bool isRunning = Input.GetKey(KeyCode.LeftShift);

        // --- INI PERBAIKAN BUG AUDIO ---
        // Jika kita tidak diizinkan bergerak (di-lock oleh Pause ATAU Inspect)
        if (currentState != ControlState.PlayerControl)
        {
            // Hentikan suara langkah kaki
            if (sfxSource != null && sfxSource.isPlaying)
            {
                sfxSource.Stop();
            }
            // ------------------

            // Cek apakah kita HANYA sedang 'Turn' (dan tidak dikunci Manajer)
            if (currentState == ControlState.IsTurning)
            {
                // Cek apakah animasi 'Turn' sudah SELESAI
                AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
                
                if (stateInfo.shortNameHash == animHash_Turn && stateInfo.normalizedTime >= 1.0f)
                {
                    OnTurnAnimationFinished(); 
                }
                else if (stateInfo.shortNameHash != animHash_Turn)
                {
                    OnTurnAnimationFinished();
                }
            }
            
            horizontalInput = 0; 
            return; // Keluar dari Update() (INI MENGHENTIKAN INPUT A/D SAAT PAUSE)
        }
        
        // --- KONTROL PLAYER AKTIF ---
        GameData.isSprinting = isRunning; 

        // 3. --- LOGIKA GERAKAN DAN BALIK BADAN ---
        if (horizontalInput != 0)
        {
            // Cek apakah player ingin berbalik
            bool wantsToTurn = (isFacingRight && horizontalInput < 0f) || (!isFacingRight && horizontalInput > 0f);

            if (wantsToTurn) 
            {
                // Player ingin berbalik!
                currentState = ControlState.IsTurning; 
                anim.SetTrigger(animHash_doTurn); 
                
                if (sfxSource != null) sfxSource.Stop(); // Hentikan SFX saat berbalik
            }
            else 
            {
                // Player bergerak ke arah yang sama (tidak berbalik)
                if (isRunning) { anim.SetInteger(animHash_moveState, 2); } // Run
                else { anim.SetInteger(animHash_moveState, 1); } // Walk

                AudioClip clipToPlay = isRunning ? runSfx : walkSfx;
                if (sfxSource != null && clipToPlay != null)
                {
                    if (!sfxSource.isPlaying || sfxSource.clip != clipToPlay)
                    {
                        sfxSource.clip = clipToPlay;
                        sfxSource.loop = true;
                        sfxSource.Play();
                    }
                }
            }
        }
        else // horizontalInput == 0
        {
            // Player diam
            anim.SetInteger(animHash_moveState, 0); // Idle
            
            if (sfxSource != null && sfxSource.isPlaying)
            {
                sfxSource.Stop(); // Hentikan SFX saat diam
            }
        }
    }

    void FixedUpdate()
    {
        if (currentState != ControlState.PlayerControl) 
        {
            rb.velocity = Vector2.zero; 
            rb.MovePosition(rb.position); 
            return;
        }

        // Terapkan Gerakan
        float currentSpeed = GameData.isSprinting ? runSpeed : walkSpeed;
        Vector2 newPosition = rb.position + new Vector2(horizontalInput * currentSpeed * Time.fixedDeltaTime, 0f);
        rb.MovePosition(newPosition);
    }

    private void OnTurnAnimationFinished()
    {
        if (currentState != ControlState.IsTurning) return; 
        isFacingRight = !isFacingRight;
        ForceFaceDirection(isFacingRight); 
        currentState = ControlState.PlayerControl;
    }
    
    public void ForceFaceDirection(bool faceRight)
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        isFacingRight = faceRight;
        spriteRenderer.flipX = faceRight; 
    }
    
    public void SetLock(bool isLocked, bool forceFacingRight)
    {
        if (isLocked)
        {
            currentState = ControlState.LockedByManager;
            ForceFaceDirection(forceFacingRight);
            
            if (sfxSource != null) sfxSource.Stop(); // Hentikan SFX saat di-lock
        }
        else
        {
            currentState = ControlState.PlayerControl;
        }
    }

    // Fungsi ini dibutuhkan oleh InspectManager
    public bool IsFacingRight()
    {
        return isFacingRight;
    }
}