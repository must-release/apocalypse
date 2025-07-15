using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class StageScene : MonoBehaviour, IScene
{
    /****** Public Members ******/

    public bool         CanMoveToNextScene  => true;
    public SceneType    CurrentSceneType    => SceneType.StageScene;
    public Transform    PlayerTransform     => _playerTransform;

    public async UniTask AsyncInitializeScene()
    {
        await AsyncLoadEssentialStages();
        await AsyncLoadPlayer();

        PlaceStageObjects();
    }

    public void ActivateScene()
    {
        _currentStage.gameObject.SetActive(true);
        _prevStage?.gameObject.SetActive(_currentStage.CanGoBackToPreviousStage);
        _nextStage?.gameObject.SetActive(true);
        _playerTransform.gameObject.SetActive(true);
    }


    /****** Private Members ******/

    private StageManager _currentStage, _prevStage, _nextStage;
    private Transform _playerTransform;

    private async UniTask AsyncLoadEssentialStages()
    {
        PlayerManager.Instance.GetPlayerData(out ChapterType chapter, out int stage, out PlayerAvatarType _);
        
        string currentStagePath = $"Stage/{chapter}_{stage}";
        _currentStage = await AsyncLoadStage(currentStagePath);
        _currentStage.gameObject.SetActive(false);

        if (_currentStage.CanGoBackToPreviousStage && ChapterStageCount.IsStageIndexValid(chapter, stage - 1))
        {
            string prevStagePath = $"Stage/{chapter}_{stage - 1}";
            _prevStage = await AsyncLoadStage(prevStagePath);
            _prevStage.gameObject.SetActive(false);
        }
        else
        {
            _currentStage.SetupEntranceBarrier();
        }

        if (ChapterStageCount.IsStageIndexValid(chapter, stage + 1))
        {
            string nextStagePath = $"Stage/{chapter}_{stage + 1}";
            _nextStage = await AsyncLoadStage(nextStagePath);
            _nextStage.gameObject.SetActive(false);
        }
    }

    private async UniTask<StageManager> AsyncLoadStage(string stage)
    {
        AsyncOperationHandle<GameObject> loadHandle = Addressables.InstantiateAsync(stage, transform);
        await loadHandle.ToUniTask();

        if (AsyncOperationStatus.Failed == loadHandle.Status)
        {
            Logger.Write(LogCategory.AssetLoad, $"Failed to load {stage}", LogLevel.Error, true);
            await UniTask.CompletedTask;
        }

        StageManager stageManager = loadHandle.Result.GetComponent<StageManager>();
        if (null == stageManager)
        {
            Logger.Write(LogCategory.AssetLoad, $"Stage {stage} does not have a StageManager component", LogLevel.Error, true);
            await UniTask.CompletedTask;
        }

        await stageManager.AsyncInitializeStage();

        return stageManager;
    }

    private async UniTask AsyncLoadPlayer()
    {
        AsyncOperationHandle<GameObject> loadHandle = Addressables.InstantiateAsync("Character/Player", transform);
        await loadHandle.ToUniTask();

        if (AsyncOperationStatus.Failed == loadHandle.Status)
        {
            Logger.Write(LogCategory.AssetLoad, "Failed to load player", LogLevel.Error, true);
            await UniTask.CompletedTask;
        }

        _playerTransform = loadHandle.Result.transform;
        if (_playerTransform.TryGetComponent(out IAsyncLoadObject sceneObject))
        {
            await UniTask.WaitUntil(() => sceneObject.IsLoaded, cancellationToken: this.GetCancellationTokenOnDestroy());
        }

        _playerTransform.gameObject.SetActive(false);
    }

    private void PlaceStageObjects()
    {
        Assert.IsTrue(null != _playerTransform, "Player transform is not initialized. Ensure AsyncLoadPlayer is called before placing stage objects.");

        _playerTransform.position = _currentStage.PlayerStartPosition;
        _prevStage?.SnapToPoint(_currentStage.EnterSnapPoint);
        _nextStage?.SnapToPoint(_currentStage.ExitSnapPoint);

        Logger.Write(LogCategory.GameScene, $"Placing player at {_currentStage.PlayerStartPosition} in stage", LogLevel.Log, true);
    }

}
