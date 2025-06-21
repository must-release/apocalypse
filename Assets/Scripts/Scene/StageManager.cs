using Cysharp.Threading.Tasks;
using NUnit.Framework;
using System.Linq;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    /****** Public Members ******/

    public Vector3 PlayerStartPosition
    {
        get
        {
            Assert.IsTrue(null != _playerStart, $"PlayerStart component is missing in the {_chapterType}_{_stageIndex}.");
            return _playerStart.StartPosition;
        }
    }

    public async UniTask AsyncInitializeStage()
    {
        var sceneObjects = GetComponentsInChildren<IAsyncLoadObject>();
        _playerStart = GetComponentInChildren<PlayerStart>();
        Assert.IsTrue(null != _playerStart, $"PlayerStart component is missing in the {_chapterType}_{_stageIndex}.");

        var waitTasks = sceneObjects
            .Select(obj => UniTask.WaitUntil(() => obj.IsLoaded, cancellationToken: this.GetCancellationTokenOnDestroy()))
            .ToArray();

        await UniTask.WhenAll(waitTasks);
    }

    public void DestroyStage()
    {
        Destroy(gameObject);
    }

    /****** Private Members ******/

    [SerializeField] ChapterType _chapterType;
    [SerializeField] int _stageIndex;

    private PlayerStart _playerStart;
}
