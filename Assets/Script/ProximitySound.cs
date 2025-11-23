using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(BoxCollider2D))]
public class ProximitySound : MonoBehaviour
{
    private AudioSource myAudioSource;
    private Collider2D myCollider;

    void Reset()
    {
        SetupComponents();
    }

    void Awake()
    {
        SetupComponents();
    }

    void SetupComponents()
    {
        myAudioSource = GetComponent<AudioSource>();
        myCollider = GetComponent<Collider2D>();

        myCollider.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (myAudioSource != null && !myAudioSource.isPlaying)
            {
                myAudioSource.Play();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (myAudioSource != null && myAudioSource.isPlaying)
            {
                myAudioSource.Stop();
            }
        }
    }
}