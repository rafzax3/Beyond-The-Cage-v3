using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class LightingZone : MonoBehaviour
{
    [Header("Pengaturan Zona")]
    [SerializeField] private Color zoneColor = Color.white; 
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private bool isAmbientZone = false;

    void Awake()
    {
        GetComponent<BoxCollider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerLightReceiver receiver = other.GetComponent<PlayerLightReceiver>();
            if (receiver != null)
            {
                receiver.SetTargetColor(zoneColor, fadeDuration, isAmbientZone);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!isAmbientZone && other.CompareTag("Player"))
        {
            PlayerLightReceiver receiver = other.GetComponent<PlayerLightReceiver>();
            if (receiver != null)
            {
                receiver.ReturnToAmbient(fadeDuration);
            }
        }
    }
}