using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class CharacterManager : MonoBehaviour
{
    /****** Public Members ******/

    public static CharacterManager Instance { get; private set; }


    public void SetPlayerCharacter(Transform player, PlayerType who)
    {
        // Find player controller
         if(player.TryGetComponent(out _playerController) == false)
         {
            Debug.LogError("Failed to find PlayerController");
         }

        // Set player State
        _playerController.InitializePlayer(who);
    }

    // Add cutscene actor to the dictionary
    public void SetActorCharacter(string actorName, Transform actor)
    {
        CharacterBase character = actor.GetComponent<CharacterBase>();
        _actorDictionary.Add(actorName, character);
    }

    // Reset actor dictionary
    public void ClearActorCharacters()
    {
        _actorDictionary.Clear();
    }

    // Execute control of the player character
    public void ExecutePlayerControl(ControlInfo controlInfo)
    {
        Assert.IsTrue(null != controlInfo, "Control info is null");
        Assert.IsTrue(null != _playerController, "Player controller is not initialized");


        _playerController.ControlCharacter(controlInfo);
    }

    // Execute control of the actor character
    public void ExecuteActorControl(ControlInfo controlInfo, string actorName)
    {
        if( _actorDictionary.ContainsKey(actorName) )
        {
            _actorDictionary[actorName].ControlCharacter(controlInfo);
        }
        else
        {
            Debug.LogError("No such actor: " + actorName);
        }
    }

    public Vector3 GetPlayerPosition()
    {
        if(_playerController != null)
        {
            return _playerController.transform.position;
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


    /****** Private Members ******/

    private PlayerController _playerController = null;
    private Dictionary<string, CharacterBase> _actorDictionary = new Dictionary<string, CharacterBase>();

    private void Awake()
    {
        if (null == Instance )
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

}
