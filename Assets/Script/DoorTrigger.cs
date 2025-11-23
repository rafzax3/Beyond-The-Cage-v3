using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    public string doorID;

    private GameManager gameManagerInScene;
    private FinalTrigger finalTriggerInScene;
    private Collider2D doorCollider;

    void Start()
    {
        gameManagerInScene = FindObjectOfType<GameManager>();
        finalTriggerInScene = FindObjectOfType<FinalTrigger>();
        
        doorCollider = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (doorCollider != null) doorCollider.enabled = false;
            
            if (gameManagerInScene != null)
            {
                gameManagerInScene.PlayerHitDoor(doorID);
            }
            else if (finalTriggerInScene != null)
            {
                finalTriggerInScene.GoToTheEnd(doorID);
            }
        }
    }
}