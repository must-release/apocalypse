using UnityEngine;

public class PlayerDetector : MonoBehaviour
{
    private GameObject detectedPlayer;
    private Transform detector;

    public void SetPlayerDetector(Vector2 detectRange, Vector2 rangeOffset)
    {
        BoxCollider2D rangeCollider = gameObject.AddComponent<BoxCollider2D>();
        rangeCollider.size = detectRange;
        rangeCollider.offset = rangeOffset;
        rangeCollider.isTrigger = true;
    }

    public GameObject DetectPlayer() { return detectedPlayer; }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            detectedPlayer = other.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.gameObject == detectedPlayer)
                detectedPlayer = null;
            else
                Debug.LogError("Unknown Player Exiting from Detect Range");
        }
    }
}