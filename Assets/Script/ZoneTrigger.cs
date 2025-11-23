using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class ZoneTrigger : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private FlickeringLight lightController;

    [Header("Tipe Zona")]
    [SerializeField] private bool isSoundZone = false;
    [SerializeField] private bool isVisualZone = false;

    void Awake()
    {
        GetComponent<BoxCollider2D>().isTrigger = true;
        if (lightController == null)
            lightController = GetComponentInParent<FlickeringLight>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && lightController != null)
        {
            if (isSoundZone) lightController.SetSoundActive(true);
            if (isVisualZone) lightController.SetVisualActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && lightController != null)
        {
            if (isSoundZone) lightController.SetSoundActive(false);
            if (isVisualZone) lightController.SetVisualActive(false);
        }
    }
}