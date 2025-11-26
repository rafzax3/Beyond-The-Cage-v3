using System.Collections;
using UnityEngine;
using UnityEngine.Video;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(AudioSource))]
public class WindowJumpscare : MonoBehaviour
{
    [Header("Referensi Wajib")]
    [SerializeField] private VideoPlayer targetVideoPlayer;

    [Header("Mode")]
    [Tooltip("Jika dicentang, ini hanya jendela biasa (bukan jumpscare)")]
    [SerializeField] private bool isDecorationOnly = false;

    [Header("Video Settings")]
    [SerializeField] private VideoClip rainWindowClip;
    [SerializeField] private VideoClip scareWindowClip;

    [Header("Efek Petir (Flash)")]
    [SerializeField] private SpriteRenderer flashOverlay;

    [Header("Audio")]
    [SerializeField] private AudioClip thunderSfx;

    private AudioSource audioSource;
    private bool hasTriggered = false;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        GetComponent<BoxCollider2D>().isTrigger = true;

        if (targetVideoPlayer == null)
        {
            targetVideoPlayer = GetComponentInChildren<VideoPlayer>();
        }

        // Setup Awal
        if (targetVideoPlayer != null && rainWindowClip != null)
        {
            targetVideoPlayer.clip = rainWindowClip;
            targetVideoPlayer.isLooping = true;
            targetVideoPlayer.Play();
        }

        if (flashOverlay != null)
        {
            flashOverlay.color = new Color(1, 1, 1, 0);
            flashOverlay.sortingLayerName = "Foreground";
            flashOverlay.sortingOrder = 100;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Jika ini cuma dekorasi, jangan lakukan apa-apa
        if (isDecorationOnly) return;

        // Jika bukan anomaly, jangan jumpscare
        if (!GameData.isAnomalyPresent) return;

        if (!hasTriggered && other.CompareTag("Player"))
        {
            hasTriggered = true;
            StartCoroutine(PlayJumpscare());
        }
    }

    private IEnumerator PlayJumpscare()
    {
        if (audioSource != null && thunderSfx != null)
        {
            audioSource.PlayOneShot(thunderSfx);
        }

        if (flashOverlay != null)
        {
            flashOverlay.color = Color.white;

            if (targetVideoPlayer != null && scareWindowClip != null)
            {
                targetVideoPlayer.Stop();
                targetVideoPlayer.clip = scareWindowClip;
                targetVideoPlayer.isLooping = false;
                targetVideoPlayer.Play();
            }

            yield return new WaitForSeconds(0.1f);

            flashOverlay.color = new Color(1, 1, 1, 0);
            yield return new WaitForSeconds(0.05f);
            flashOverlay.color = new Color(1, 1, 1, 0.5f);
            yield return new WaitForSeconds(0.05f);
            flashOverlay.color = new Color(1, 1, 1, 0);
        }
    }
}