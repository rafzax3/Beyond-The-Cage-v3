using UnityEngine;

    [RequireComponent(typeof(BoxCollider2D))]
    public class TutorialTrigger : MonoBehaviour
    {
        private bool hasTriggered = false;

        void Awake()
        {
            GetComponent<BoxCollider2D>().isTrigger = true;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!hasTriggered && other.CompareTag("Player"))
            {
                if (TutorialManager.instance != null)
                {
                    hasTriggered = true;
                    TutorialManager.instance.TriggerLetterTutorial();
                }
            }
        }
    }