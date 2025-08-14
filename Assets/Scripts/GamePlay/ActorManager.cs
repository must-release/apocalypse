using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using AD.GamePlay;

public class ActorManager : MonoBehaviour
{
    /****** Public Members ******/

    public static ActorManager Instance { get; private set; }

    public void RegisterActor(IActor actor)
    {
        Debug.Assert(null != actor, "Cannot register null actor.");

        if (false == _registeredActors.Contains(actor))
        {
            _registeredActors.Add(actor);
        }
    }

    public void RegisterActors(IActor[] actors)
    {
        Debug.Assert(null != actors, "Cannot register null actor array.");

        foreach (var actor in actors)
        {
            if (null != actor)
            {
                RegisterActor(actor);
            }
        }
    }

    public void SetPlayerCharacter(Transform player, PlayerAvatarType who)
    {
        Debug.Assert(null != player, "Player transform is null.");
        Debug.Assert(null != player.GetComponent<PlayerController>(), "Player does not have a PlayerController component.");

        _playerController = player.GetComponent<PlayerController>();
        _playerController.InitializePlayer(who);
    }

    public void ExecutePlayerControl(IReadOnlyControlInfo controlInfo)
    {
        Debug.Assert(null != controlInfo, "Control info is null");
        Debug.Assert(null != _playerController, "Player controller is not initialized");

        _playerController.ControlCharacter(controlInfo);
    }

    public void ExecuteActorControl(ControlInfo controlInfo, string actorName)
    {
        var actor = GetActorByName(actorName) as ICharacter;
        Debug.Assert(null != actor, $"Actor with name {actorName} not found.");
        
        actor.ControlCharacter(controlInfo);
    }

    public Vector3 GetPlayerPosition()
    {
        Debug.Assert(null != _playerController, "Player controller is not initialized.");

        return _playerController.transform.position;
    }

    public void ClearActors(bool clearPlayer = false)
    {
        _registeredActors.Clear();

        if (clearPlayer)
        {
            _playerController = null;
        }
    }

    public IActor GetActorByName(string actorName)
    {
        return _registeredActors.FirstOrDefault(actor => actor.ActorName == actorName);
    }


    /****** Private Members ******/

    private readonly List<IActor> _registeredActors = new List<IActor>();

    private PlayerController _playerController;

    private void Awake()
    {
        if (null == Instance)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}