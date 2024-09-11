using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public float interactionRadius = 3f; 
    private bool isPlayerNearby = false; 

    void Update()
    {
        CheckPlayerDistance();
    }

    private void CheckPlayerDistance()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        float distance = Vector3.Distance(player.transform.position, transform.position);

        if (distance <= interactionRadius)
        {
            if (!isPlayerNearby)
            {
                OnPlayerEnterProximity();
                isPlayerNearby = true;
            }
        }
        else
        {
            if (isPlayerNearby)
            {
                OnPlayerExitProximity();
                isPlayerNearby = false;
            }
        }
    }

    private void OnPlayerEnterProximity()
    {
        Debug.Log("Player is near, show interaction indicator.");
    }

    private void OnPlayerExitProximity()
    {
        Debug.Log("Player is far, hide interaction indicator.");
    }
}
