using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class FlickeringLight : MonoBehaviour
{
    [Header("Mode")]
    [Tooltip("Untuk HP: Centang ini agar suara/cahaya main 1x saja, tidak loop.")]
    [SerializeField] private bool isOneShot = false;
    [Tooltip("Untuk Lampu Hantu: Centang ini agar nyala terus tanpa kedip.")]
    [SerializeField] private bool isSteadyLight = false; 

    [Header("Cahaya (Light 2D / Sprite)")]
    [SerializeField] private GameObject lightObject; 

    [Header("Efek Ruangan & Player")]
    [SerializeField] private SpriteRenderer roomDarknessOverlay;
    [SerializeField] private SpriteRenderer windowDarknessOverlay; 
    
    [Range(0f, 1f)] [SerializeField] private float darknessIntensity = 0.7f;
    [SerializeField] private Color playerDarkColor = new Color(0.4f, 0.4f, 0.4f, 1f);

    [Header("Anomaly Merah (Red Light)")]
    [Range(0f, 1f)] [SerializeField] private float redBlinkChance = 0.2f; 
    [SerializeField] private Color redLightColor = new Color(1f, 0f, 0f, 0.5f); 
    [SerializeField] private Color playerRedColor = new Color(1f, 0.5f, 0.5f, 1f); 

    [Header("Pengaturan Kedip (Lampu Biasa)")]
    [SerializeField] private float minOnTime = 0.1f;
    [SerializeField] private float maxOnTime = 2.0f;
    [SerializeField] private float minOffTime = 0.1f;
    [SerializeField] private float maxOffTime = 0.5f;
    
    [Header("Audio Default")]
    [SerializeField] private AudioClip electricBuzzClip;
    [SerializeField] private AudioClip flickOnClip;
    
    [Header("Anomaly Audio (HP/TV)")]
    [SerializeField] private AudioClip normalAudioClip;
    [SerializeField] private AudioClip anomalyAudioClip;

    [Range(1f, 5f)] [SerializeField] private float buzzVolumeMultiplier = 2.0f;

    private AudioSource audioSource;
    private Coroutine flickerCoroutine;
    private PlayerLightReceiver playerReceiver;

    private bool isSoundActive = false;
    private bool isVisualActive = false;
    private bool isAnomalyRoom = false; 
    
    // Status untuk One Shot
    private bool hasPlayed = false;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        isAnomalyRoom = GameData.isAnomalyPresent;

        if (audioSource != null)
        {
            audioSource.loop = !isOneShot; // Jika OneShot, jangan loop
            audioSource.playOnAwake = false;
            
            // Pilih Audio
            if (normalAudioClip != null || anomalyAudioClip != null)
            {
                if (isAnomalyRoom && anomalyAudioClip != null)
                    audioSource.clip = anomalyAudioClip;
                else if (normalAudioClip != null)
                    audioSource.clip = normalAudioClip;
            }
            else 
            {
                audioSource.clip = electricBuzzClip;
            }

            audioSource.volume = 0f; 
            audioSource.Stop(); 
        }
        
        // Default mati
        SetLightState(false, false);
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
            if (player != null) playerReceiver = player.GetComponent<PlayerLightReceiver>();
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
            // Jika Mode HP (OneShot) dan sudah pernah bunyi, jangan bunyi lagi
            if (isOneShot && hasPlayed) return;

            if (flickerCoroutine == null)
            {
                // Untuk mode biasa (Buzz), mainkan audio di sini
                if (!isOneShot && isSoundActive && audioSource.clip != null && !audioSource.isPlaying)
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
            
            // Jika HP/OneShot -> Matikan total saat keluar
            // Jika Lampu -> Nyalakan (biar terlihat normal dari jauh)
            SetLightState(!isOneShot, false); 
            
            if (audioSource.isPlaying) audioSource.Stop();
        }
    }

    private IEnumerator FlickerRoutine()
    {
        // --- MODE HP / ONE SHOT ---
        if (isOneShot)
        {
            hasPlayed = true;
            audioSource.volume = 1.0f * buzzVolumeMultiplier;
            audioSource.Play();

            // Selama audio masih main, nyalakan cahaya
            while (audioSource.isPlaying)
            {
                SetLightState(true, false); 
                yield return null;
            }
            
            // Audio selesai -> Matikan cahaya
            SetLightState(false, false);
            flickerCoroutine = null;
            yield break;
        }

        // --- MODE STEADY ---
        if (isSteadyLight)
        {
            while (isSoundActive || isVisualActive)
            {
                if (isVisualActive) SetLightState(true, false);
                
                if (isSoundActive) audioSource.volume = 1.0f * buzzVolumeMultiplier;
                else audioSource.volume = 0f;

                yield return null; 
            }
            yield break; 
        }

        // --- MODE KEDIP (LAMPU RUSAK) ---
        while (isSoundActive || isVisualActive)
        {
            bool isRedBlink = false;
            if (isAnomalyRoom) isRedBlink = (Random.value < redBlinkChance);

            if (isVisualActive) SetLightState(true, isRedBlink);
            
            if (isSoundActive && isVisualActive && flickOnClip != null) 
                audioSource.PlayOneShot(flickOnClip, 1.0f); 
            
            if (isSoundActive) audioSource.volume = 1.0f * buzzVolumeMultiplier;
            else audioSource.volume = 0f;

            float onDuration = Random.Range(minOnTime, maxOnTime);
            yield return new WaitForSeconds(onDuration);

            if (isVisualActive) SetLightState(false, false); 

            audioSource.volume = 0f;

            float offDuration = Random.Range(minOffTime, maxOffTime);
            yield return new WaitForSeconds(offDuration);
        }
    }

    private void SetLightState(bool isOn, bool isRed)
    {
        if (lightObject != null) 
        {
            lightObject.SetActive(isOn);
            SpriteRenderer lightSr = lightObject.GetComponent<SpriteRenderer>();
            if (lightSr != null)
            {
                lightSr.color = (isOn && isRed) ? redLightColor : Color.white;
            }
        }

        float targetAlpha = isOn ? 0f : darknessIntensity;
        Color overlayColor = Color.black; 

        if (isOn && isRed)
        {
             targetAlpha = 0.2f; 
             overlayColor = new Color(0.5f, 0f, 0f, 1f);
        }

        if (roomDarknessOverlay != null)
            roomDarknessOverlay.color = new Color(overlayColor.r, overlayColor.g, overlayColor.b, targetAlpha);
        
        if (windowDarknessOverlay != null)
            windowDarknessOverlay.color = new Color(overlayColor.r, overlayColor.g, overlayColor.b, targetAlpha);

        if (playerReceiver != null)
        {
            if (isOn)
            {
                if (isRed) playerReceiver.SetDarknessOverride(true, playerRedColor); 
                else playerReceiver.SetDarknessOverride(false, Color.white); 
            }
            else
            {
                playerReceiver.SetDarknessOverride(true, playerDarkColor); 
            }
        }
    }
}