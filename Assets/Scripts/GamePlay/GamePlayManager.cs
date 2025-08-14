using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AD.GamePlay
{
    public class GamePlayManager : MonoBehaviour, IAsyncLoadObject
    {
        /****** Public Members ******/

        public static GamePlayManager Instance { get; private set; }
        public bool IsCutscenePlaying { get; private set; }
        public bool IsLoaded { get; private set; }

        public void PlayCutscene() { }

        public void InitializePlayerCharacter(Transform player, PlayerAvatarType character)
        {
            ActorManager.Instance.SetPlayerCharacter(player, character);
        }

        public void ControlPlayerCharacter(IReadOnlyControlInfo controlInfo)
        {
            ActorManager.Instance.ExecutePlayerControl(controlInfo);
        }

        public void RegisterGamePlayInitializer(IGamePlayInitializer poolingSystem)
        {
            Debug.Assert(false == _initializerList.Contains(poolingSystem), "The pooling system is already Registered.");

            _initializerList.Add(poolingSystem);
        }

        public void RegisterStageActors(IActor[] actors)
        {
            Debug.Assert(null != actors, "Cannot register null actor array.");

            ActorManager.Instance.RegisterActors(actors);
        }

        public IActor GetStageActorByName(string actorName)
        {
            return ActorManager.Instance.GetActorByName(actorName);
        }


        /****** Private Members ******/

        private readonly List<IGamePlayInitializer> _initializerList = new List<IGamePlayInitializer>();


        private void Awake()
        {
            if (Instance == null)
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
                yield return new WaitUntil(() => initializer.IsInitialized);
            }

            IsLoaded = true;
        }
    }
}