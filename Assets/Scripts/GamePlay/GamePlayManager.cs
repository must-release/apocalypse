using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayManager : MonoBehaviour, IAsyncLoadObject
{
    /****** Public Members ******/

    public static GamePlayManager Instance { get; private set; }
    public bool IsCutscenePlaying { get; private set; }
    public bool IsLoaded { get; private set; }

    public void PlayCutscene() { StartCoroutine(CutsceneCoroutine()); }

    public void InitializePlayerCharacter(Transform player, PlayerAvatarType character)
    {
        CharacterManager.Instance.SetPlayerCharacter(player, character);
    }

    public void ControlPlayerCharacter(IReadOnlyControlInfo controlInfo)
    {
        CharacterManager.Instance.ExecutePlayerControl(controlInfo);
    }

    public void ProcessGameOver()
    {
        GameEventManager.Instance.Submit(GameEventFactory.CreateCommonEvent(CommonEventType.GameOver));
    }

    public void RegisterGamePlayInitializer(IGamePlayInitializer poolingSystem)
    {
        Debug.Assert(false == _initializerList.Contains(poolingSystem), "The pooling system is already Registered.");

        _initializerList.Add(poolingSystem);
    }


    /****** Private Members ******/

    private readonly List<IGamePlayInitializer> _initializerList = new List<IGamePlayInitializer>();


    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        StartCoroutine(AsyncWaitForInitialization());
    }

    private IEnumerator AsyncWaitForInitialization()
    {
        yield return null; // Wait for initializers to be registered

        foreach (var initializer in _initializerList)
        {
            yield return new WaitUntil(()=> initializer.IsInitialized);
        }

        IsLoaded = true;
    }

    private IEnumerator CutsceneCoroutine()
    {
        IsCutscenePlaying = true;
        Debug.Log("Playing cutscene");
        yield return new WaitForSeconds(3);
        IsCutscenePlaying = false;
    }
}
