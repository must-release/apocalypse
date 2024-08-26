using System.Collections;
using System.Collections.Generic;
using CharacterEums;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    public static CharacterManager Instance;

    private PlayerController playerController;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    // Set player character transform, info
    public void SetPlayerCharacter(Transform transform, CHARACTER who)
    {
        // Find player controller
         playerController = transform.GetComponent<PlayerController>();
         if(playerController == null)
         {
            Debug.LogError("Falied to find PlayerController");
         }

        // Set player State
        playerController.SetPlayerState(who);
    }
}
