using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class FlickeringLight : MonoBehaviour
{
    [Header("Cahaya (Light 2D / Sprite)")]
    [SerializeField] private GameObject lightObject; 

    [Header("Efek Ruangan & Player")]
    [SerializeField] private SpriteRenderer roomDarknessOverlay;
    
    // GANTI INI: Bukan Renderer jendela, tapi Overlay Gelap KHUSUS Jendela
    [Tooltip("Sprite Hitam yang menempel di depan jendela video (untuk menggelapkannya)")]
    [SerializeField] private SpriteRenderer windowDarknessOverlay; 

    [Range(0f, 1f)] [SerializeField] private float darknessIntensity = 0.7f;
    [SerializeField] private Color playerDarkColor = new Color(0.4f, 0.4f, 0.4f, 1f);

    [Header("Pengaturan Kedip")]
    [SerializeField] private float minOnTime = 0.1f;
    [SerializeField] private float maxOnTime = 2.0f;
    [SerializeField] private float minOffTime = 0.1f;
    [SerializeField] private float maxOffTime = 0.5f;
    
    [Header("Audio")]
    [SerializeField] private AudioClip electricBuzzClip;
    [SerializeField] private AudioClip flickOnClip;
    [Range(1f, 5f)] [SerializeField] private float buzzVolumeMultiplier = 2.0f;

    private AudioSource audioSource;
    private Coroutine flickerCoroutine;
    private PlayerLightReceiver playerReceiver;

    private bool isSoundActive = false;
    private bool isVisualActive = false;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        
        if (audioSource != null)
        {
            audioSource.loop = true;
            audioSource.playOnAwake = false;
            audioSource.clip = electricBuzzClip;
            audioSource.volume = 0f; 
            audioSource.Stop(); 
        }
        
        SetLightState(false);
    }

    public void SetSoundActive(bool active)
    {
        isSoundActive = active;
        UpdateEffectState();
    }

    public void SetVisualActive(bool active)
    {
        isVisualActive = active;

        if (active)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerReceiver = player.GetComponent<PlayerLightReceiver>();
            }
        }
        else
        {
            if (playerReceiver != null) playerReceiver.SetDarknessOverride(false, Color.white);
            if (playerReceiver != null) playerReceiver.ReturnToAmbient(0.2f);
            playerReceiver = null;
        }

        UpdateEffectState();
    }

    private void UpdateEffectState()
    {
        if (isSoundActive || isVisualActive)
        {
            if (flickerCoroutine == null)
            {
                if (isSoundActive && electricBuzzClip != null && !audioSource.isPlaying)
                    audioSource.Play();

                flickerCoroutine = StartCoroutine(FlickerRoutine());
            }
        }
        else
        {
            if (flickerCoroutine != null)
            {
                StopCoroutine(flickerCoroutine);
                flickerCoroutine = null;
            }
            
            SetLightState(true); 
            
            if (audioSource.isPlaying) audioSource.Stop();
        }
    }

    private IEnumerator FlickerRoutine()
    {
        while (isSoundActive || isVisualActive)
        {
            if (isVisualActive) SetLightState(true);
            
            if (isSoundActive && isVisualActive && flickOnClip != null) 
                audioSource.PlayOneShot(flickOnClip, 1.0f); 
            
            if (isSoundActive) audioSource.volume = 1.0f * buzzVolumeMultiplier;
            else audioSource.volume = 0f;

            float onDuration = Random.Range(minOnTime, maxOnTime);
            yield return new WaitForSeconds(onDuration);

            if (isVisualActive) SetLightState(false);

            audioSource.volume = 0f;

            float offDuration = Random.Range(minOffTime, maxOffTime);
            yield return new WaitForSeconds(offDuration);
        }
    }

    private void SetLightState(bool isOn)
    {
        if (lightObject != null) lightObject.SetActive(isOn);

        float targetAlpha = isOn ? 0f : darknessIntensity;

        // 1. Overlay Ruangan
        if (roomDarknessOverlay != null)
        {
            roomDarknessOverlay.color = new Color(0, 0, 0, targetAlpha);
        }

        // 2. Overlay Jendela (BARU)
        if (windowDarknessOverlay != null)
        {
            windowDarknessOverlay.color = new Color(0, 0, 0, targetAlpha);
        }

        // 3. Player
        if (playerReceiver != null)
        {
            if (isOn)
            {
                playerReceiver.SetDarknessOverride(false, Color.white);
            }
            else
            {
                playerReceiver.SetDarknessOverride(true, playerDarkColor);
            }
        }
    }
}