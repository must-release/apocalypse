using System.Collections;
using AssetEnums;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Assertions;
using UnityEngine.ResourceManagement.AsyncOperations;

public class GamePlayManager : MonoBehaviour, IAsyncLoadObject
{
    /****** Public Members ******/

    public static GamePlayManager Instance { get; private set; } = null;

    public bool IsCutscenePlaying { get; private set; } = false;

    public bool IsLoaded => _isEventLoaded && _isPoolLoaded;

    public void PlayCutscene() { StartCoroutine(CutsceneCoroutine()); }

    // Initialize Player transform, info
    public void InitializePlayerCharacter(Transform player, PlayerType character)
    {
        CharacterManager.Instance.SetPlayerCharacter(player, character);

        CameraController.Instance.AttachCamera(player, 0.1f);
    }

    // Control player character
    public void ControlPlayerCharacter(ControlInfo controlInfo)
    {
        // Control player
        CharacterManager.Instance.ExecutePlayerControl(controlInfo);
    }

    public void ProcessGameOver()
    {
        Assert.IsTrue(null != _gameOverEventInfo, "Game over event info is not set");

        GameEventManager.Instance.Submit(GameEventFactory.CreateFromInfo(_gameOverEventInfo));
    }


    /****** Private Members ******/

    private GameEventInfo _gameOverEventInfo = null;

    private bool _isEventLoaded = false;
    private bool _isPoolLoaded  = false;

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
        StartCoroutine(AsyncLoadGameEventInfos());
        StartCoroutine(AsyncLoadObjectPool());
    }

    private IEnumerator AsyncLoadGameEventInfos()
    {
        // Load game over event info
        var gameOverEventInfo = Addressables.LoadAssetAsync<GameEventInfo>(SequentialEventInfoAsset.Common.GameOver);
        yield return gameOverEventInfo;
        if (gameOverEventInfo.Status == AsyncOperationStatus.Succeeded)
        {
            _gameOverEventInfo = gameOverEventInfo.Result;
        }
        else
        {
            Debug.LogError("Failed to load Game Over Event Info");
            yield break;
        }

        _isEventLoaded = true;
    }

    private IEnumerator AsyncLoadObjectPool()
    {
        yield return PooInitializer.AsyncLoadPool();

        _isPoolLoaded = true;
    }

    private IEnumerator CutsceneCoroutine()
    {
        IsCutscenePlaying = true;
        Debug.Log("Playing cutscene");
        yield return new WaitForSeconds(3);
        IsCutscenePlaying = false;
    }
}
