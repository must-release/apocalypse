using Cysharp.Threading.Tasks;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
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
        await AsyncLoadStageAssets();
    }

    public void ActivateScene()
    {
        foreach (StageManager stage in _stageQueue)
        {
            stage.gameObject.SetActive(true);
        }

        _playerTransform.gameObject.SetActive(true);
    }


    /****** Private Members ******/

    private const int _MaxStageCount = 4;

    private Queue<StageManager> _stageQueue = new Queue<StageManager>(_MaxStageCount);

    private Transform _playerTransform;

    private async UniTask AsyncLoadStageAssets()
    {
        PlayerManager.Instance.GetPlayerData(out ChapterType chapter, out int stage, out PlayerType _);
        string firstStage   = $"Stage/{chapter}_{stage}";
        string secondStage  = $"Stage/{chapter}_{stage + 1}";

        await AsyncLoadStage(firstStage);
        //await AsyncLoadStage(secondStage);
        await AsyncLoadPlayer();

        PlaceStageObjects();
    }

    private async UniTask AsyncLoadStage(string stage)
    {
        AsyncOperationHandle<GameObject> loadHandle = Addressables.InstantiateAsync(stage, transform);
        await loadHandle.ToUniTask();

        if (AsyncOperationStatus.Failed == loadHandle.Status)
        {
            Logger.Write(LogCategory.AssetLoad, $"Failed to load {stage}", LogLevel.Error, true);
            await UniTask.CompletedTask;
        }

        if (loadHandle.Result.TryGetComponent(out StageManager stageManager))
        {
            if (_MaxStageCount <= _stageQueue.Count)
            {
                _stageQueue.Dequeue().DestroyStage();
            }
            _stageQueue.Enqueue(stageManager);

            await stageManager.AsyncInitializeStage();

            stageManager.gameObject.SetActive(false);
        }
        else
        {
            Logger.Write(LogCategory.AssetLoad, $"Stage {stage} does not have a StageManager component", LogLevel.Error, true);
        }
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
        Assert.IsTrue(0 < _stageQueue.Count, "Stage queue is empty. Ensure stages are loaded before placing objects.");

        StageManager currentStage = _stageQueue.Peek();
        _playerTransform.position = currentStage.PlayerStartPosition;
    }
}
