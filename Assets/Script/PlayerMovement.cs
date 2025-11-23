using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    private enum ControlState { PlayerControl, IsTurning, LockedByManager }
    private ControlState currentState = ControlState.LockedByManager;
    
    [Header("Referensi Komponen")]
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer; 
    private Animator anim; 

    [Header("Referensi Bayangan")]
    [SerializeField] private Animator shadowAnim; 
    [SerializeField] private SpriteRenderer shadowSpriteRenderer;

    [Header("Pengaturan Gerak")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 8f; 
    
    [Header("Audio")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip walkSfx;
    [SerializeField] private AudioClip runSfx;

    private float horizontalInput;
    private bool isFacingRight = false; 
    
    private int animHash_Turn = Animator.StringToHash("CatTurn"); 
    private int animHash_moveState = Animator.StringToHash("moveState");
    private int animHash_doTurn = Animator.StringToHash("doTurn");

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>(); 
        spriteRenderer = GetComponent<SpriteRenderer>(); 
        
        if (sfxSource == null) sfxSource = GetComponent<AudioSource>();

        if (shadowAnim == null) shadowAnim = GetComponentInChildren<Animator>();
        if (shadowSpriteRenderer == null && shadowAnim != null)
            shadowSpriteRenderer = shadowAnim.GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (currentState != ControlState.PlayerControl)
        {
            if (sfxSource != null && sfxSource.isPlaying) sfxSource.Stop();

            if (currentState == ControlState.IsTurning)
            {
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
            return; 
        }
        
        horizontalInput = Input.GetAxisRaw("Horizontal");
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        GameData.isSprinting = isRunning; 

        if (horizontalInput != 0)
        {
            bool wantsToTurn = (isFacingRight && horizontalInput < 0f) || (!isFacingRight && horizontalInput > 0f);

            if (wantsToTurn) 
            {
                currentState = ControlState.IsTurning; 
                
                anim.SetTrigger(animHash_doTurn); 
                if (shadowAnim != null) shadowAnim.SetTrigger(animHash_doTurn); 

                if (sfxSource != null) sfxSource.Stop();
            }
            else 
            {
                int moveStateValue = isRunning ? 2 : 1;
                
                anim.SetInteger(animHash_moveState, moveStateValue);
                if (shadowAnim != null) shadowAnim.SetInteger(animHash_moveState, moveStateValue);

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
        else 
        {
            anim.SetInteger(animHash_moveState, 0);
            if (shadowAnim != null) shadowAnim.SetInteger(animHash_moveState, 0);
            
            if (sfxSource != null && sfxSource.isPlaying) sfxSource.Stop();
        }
    }

    void FixedUpdate()
    {
        if (currentState != ControlState.PlayerControl) 
        {
            rb.velocity = Vector2.zero; 
            return;
        }

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
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        
        isFacingRight = faceRight;
        if (spriteRenderer != null) spriteRenderer.flipX = faceRight; 

        if (shadowSpriteRenderer != null) shadowSpriteRenderer.flipX = faceRight;
    }
    
    public void SetScriptedAnimation(int state)
    {
        if (anim != null) anim.SetInteger(animHash_moveState, state);
        if (shadowAnim != null) shadowAnim.SetInteger(animHash_moveState, state);
    }

    public void SetLock(bool isLocked, bool forceFacingRight)
    {
        if (isLocked)
        {
            currentState = ControlState.LockedByManager;
            ForceFaceDirection(forceFacingRight);
            
            if (sfxSource != null) sfxSource.Stop();
        }
        else
        {
            currentState = ControlState.PlayerControl;
            
            anim.SetInteger(animHash_moveState, 0);
            if (shadowAnim != null) shadowAnim.SetInteger(animHash_moveState, 0);
        }
    }

    public bool IsFacingRight()
    {
        return isFacingRight;
    }
}