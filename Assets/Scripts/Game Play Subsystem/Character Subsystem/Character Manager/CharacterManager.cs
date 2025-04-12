using System.Collections.Generic;
using CharacterEums;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    public static CharacterManager Instance;

    private PlayerController playerController;
    private Dictionary<string, CharacterBase> actorDictionary;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            actorDictionary = new Dictionary<string, CharacterBase>();
        }
    }

    // Set player character transform, info
    public void SetPlayerCharacter(Transform player, PLAYER who)
    {
        // Find player controller
        playerController = player.GetComponent<PlayerController>();
         if(playerController == null)
         {
            Debug.LogError("Failed to find PlayerController");
         }

        // Set player State
        playerController.SetPlayer(who);
    }

    // Add cutscene actor to the dictionary
    public void SetActorCharacter(string actorName, Transform actor)
    {
        CharacterBase character = actor.GetComponent<CharacterBase>();
        actorDictionary.Add(actorName, character);
    }

    // Reset actor dictionary
    public void ClearActorCharacters()
    {
        actorDictionary.Clear();
    }

    // Execute control of the player character
    public void ExecutePlayerControl(ControlInfo controlInfo)
    {
        if (playerController == null)
        {
            Debug.LogError("playerController not initialized");
        }
        playerController.ControlCharacter(controlInfo);
    }

    // Execute control of the actor character
    public void ExecuteActorControl(ControlInfo controlInfo, string actorName)
    {
        if( actorDictionary.ContainsKey(actorName) )
        {
            actorDictionary[actorName].ControlCharacter(controlInfo);
        }
        else
        {
            Debug.LogError("No such actor: " + actorName);
        }
    }

    public Vector3 GetPlayerPosition()
    {
        if(playerController != null)
        {
            return playerController.transform.position;
        }
        else
        {
            Debug.LogError("playerController is not initialized");
            return Vector3.zero;
        }
    }

    public void ProcessPlayersDeath()
    {
        GamePlayManager.Instance.ProcessGameOver();
    }
}
