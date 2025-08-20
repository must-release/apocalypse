using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class StageScene : MonoBehaviour, IScene
{
    /****** Public Members ******/

    public bool CanMoveToNextScene => true;
    public SceneType CurrentSceneType => SceneType.StageScene;
    public Transform PlayerTransform => _playerTransform;
    public bool HasPlayerSupport => true;

    public async UniTask AsyncInitializeScene()
    {
        await AsyncLoadEssentialStages();
        await AsyncLoadPlayer();

        PlaceStageObjects();
    }

    public async UniTask UpdateStagesForTransitionAsync()
    {
        Debug.Assert(null != _currentStage, "Current stage is not initialized.");

        PlayerManager.Instance.GetPlayerData(out ChapterType chapter, out int stage, out PlayerAvatarType _);

        _prevStage?.DestroyStage();
        _prevStage = _currentStage;
        _currentStage = _nextStage;

        if (ChapterStageCount.IsStageIndexValid(chapter, stage + 1))
        {
            string nextStagePath = $"Stage/{chapter}_{stage + 1}";
            _nextStage = await AsyncLoadStage(nextStagePath);
            _nextStage.gameObject.SetActive(false);
            _nextStage.SnapToPoint(_currentStage.ExitSnapPoint);
        }
        else
        {
            _nextStage = null;
        }
    }

    public void ActivateScene()
    {
        Debug.Assert(null != _currentStage, "Current stage is not initialized.");
        Debug.Assert(null != _playerTransform, "Player transform is not initialized.");

        _currentStage.gameObject.SetActive(true);
        _currentStage.BlockReturnToPreviousStage();
        _nextStage?.gameObject.SetActive(true);
        _playerTransform.gameObject.SetActive(true);

        StartPlayerPositionMonitoring();
    }

    public void StopPlayerPositionMonitoring()
    {
        _isMonitoringPlayer = false;
    }

    public void RespawnPlayer()
    {
        Debug.Assert(null != _currentStage, "Current stage is not initialized.");
        Debug.Assert(null != _playerTransform, "Player transform is not initialized.");

        _playerTransform.position = _currentStage.PlayerStartPosition;
        Logger.Write(LogCategory.GameScene, $"Respawning player at {_currentStage.PlayerStartPosition}", LogLevel.Log, true);
    }

    public AD.Camera.ICamera[] GetSceneCameras()
    {
        Debug.Assert(null != _currentStage, "Current stage is not initialized.");
        
        return _currentStage.GetStageCameras();
    }

    public AD.GamePlay.IActor[] GetCurrentStageActors()
    {
        Debug.Assert(null != _currentStage, "Current stage is not initialized.");

        return _currentStage.GetStageActors();
    }

    /****** Private Members ******/

    private StageManager _currentStage, _prevStage, _nextStage;
    private Transform _playerTransform;
    private bool _isMonitoringPlayer = false;

    private async UniTask AsyncLoadEssentialStages()
    {
        PlayerManager.Instance.GetPlayerData(out ChapterType chapter, out int stage, out PlayerAvatarType _);

        string currentStagePath = $"Stage/{chapter}_{stage}";
        _currentStage = await AsyncLoadStage(currentStagePath);
        _currentStage.gameObject.SetActive(false);

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
        Debug.Assert(null != _playerTransform, "Player transform is not initialized. Ensure AsyncLoadPlayer is called before placing stage objects.");

        _playerTransform.position = _currentStage.PlayerStartPosition;
        _prevStage?.SnapToPoint(_currentStage.EnterSnapPoint);
        _nextStage?.SnapToPoint(_currentStage.ExitSnapPoint);

        Logger.Write(LogCategory.GameScene, $"Placing player at {_currentStage.PlayerStartPosition} in stage", LogLevel.Log, true);
    }
    
    private void StartPlayerPositionMonitoring()
    {
        if (_isMonitoringPlayer) return;
        
        _isMonitoringPlayer = true;
        MonitorPlayerPosition().Forget();
    }

    private async UniTask MonitorPlayerPosition()
    {
        while (_isMonitoringPlayer && null != _playerTransform)
        {
            CheckPlayerStageTransition();
            await UniTask.Delay(100, cancellationToken: this.GetCancellationTokenOnDestroy());
        }
    }

    private void CheckPlayerStageTransition()
    {
        Vector3 playerPos = _playerTransform.position;
        
        if (null != _prevStage && IsPlayerInStageBoundary(playerPos, _prevStage.StageBoundary))
        {
            if (_currentStage != _prevStage)
            {
                TriggerStageTransition(_prevStage.ChapterType, _prevStage.StageIndex);
            }
        }
        else if (null != _nextStage && IsPlayerInStageBoundary(playerPos, _nextStage.StageBoundary))
        {
            if (_currentStage != _nextStage)
            {
                TriggerStageTransition(_nextStage.ChapterType, _nextStage.StageIndex);
            }
        }
    }

    private bool IsPlayerInStageBoundary(Vector3 playerPosition, BoxCollider2D stageBoundary)
    {
        Debug.Assert(null != stageBoundary, "Stage boundary cannot be null when checking player position.");
        
        Bounds bounds = stageBoundary.bounds;
        return bounds.Contains(playerPosition);
    }

    private void TriggerStageTransition(ChapterType targetChapter, int targetStage)
    {
        if (GameEventManager.Instance.IsEventTypeActive(GameEventType.StageTransition))
            return;

        StopPlayerPositionMonitoring();
        
        var stageTransitionEvent = GameEventFactory.CreateStageTransitionEvent(targetChapter, targetStage);
        GameEventManager.Instance.Submit(stageTransitionEvent);
    }

}
