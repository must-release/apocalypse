using Cysharp.Threading.Tasks;
using NUnit.Framework;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

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
    public SnapPoint EnterSnapPoint { get; private set; }
    public SnapPoint ExitSnapPoint  { get; private set; }
    
    public async UniTask AsyncInitializeStage()
    {
        InitializeTilemap();
        await WaitForAsyncObjects();

        Assert.IsTrue(null != _playerStart, $"PlayerStart component is missing in the {_chapterType}_{_stageIndex}.");
        Assert.IsTrue(null != EnterSnapPoint, $"EnterSnapPoint component is missing in the {_chapterType}_{_stageIndex}.");
        Assert.IsTrue(null != ExitSnapPoint, $"ExitSnapPoint component is missing in the {_chapterType}_{_stageIndex}.");
    }

    public void DestroyStage()
    {
        Destroy(gameObject);
    }


    /****** Private Members ******/

    [Header("Stage Settings")]
    [SerializeField] private ChapterType _chapterType;
    [SerializeField] private int _stageIndex;
    [SerializeField] private bool _canGoBackToPreviousStage;

    private Tilemap _tilemap;
    private PlayerStart _playerStart;

    private void InitializeTilemap()
    {
        _tilemap = GetComponentInChildren<Tilemap>();
        Assert.IsTrue(null != _tilemap, $"Tilemap component is missing in the {_chapterType}_{_stageIndex}.");

        BoundsInt bounds = _tilemap.cellBounds;
        TileBase[] allTiles = _tilemap.GetTilesBlock(bounds);

        int width = bounds.size.x;

        for (int i = 0; i < allTiles.Length; i++)
        {
            TileBase tile = allTiles[i];
            ObjectReplacementTile replacementTile = tile as ObjectReplacementTile;
            if (null == replacementTile)
                continue;

            int x = i % width;
            int y = i / width;
            Vector3Int cellPos = new Vector3Int(bounds.x + x, bounds.y + y, bounds.z);

            replacementTile.HideTile();
            replacementTile.RefreshTile(cellPos, _tilemap);

            Vector3 worldPos = _tilemap.GetCellCenterWorld(cellPos);
            GameObject replacingObject = Instantiate(replacementTile.ReplacingObject, worldPos, Quaternion.identity, _tilemap.transform);

            if (replacingObject.TryGetComponent(out PlayerStart playerStart))
            {
                SetPlayerStart(playerStart);
            }
            else if (replacingObject.TryGetComponent(out SnapPoint snapPoint))
            {
                SetSnapPoint(snapPoint);
            }
        }
    }

    private void SetPlayerStart(PlayerStart playerStart)
    {
        Assert.IsTrue(null != playerStart, "PlayerStart cannot be null.");
        _playerStart = playerStart;
    }

    private void SetSnapPoint(SnapPoint snapPoint)
    {
        Assert.IsTrue(null != snapPoint, "SnapPoint cannot be null.");
        Assert.IsTrue(2 == (int)SnapPoint.SnapPointType.SnapPointTypeCount, "SnapPointType enum should have exactly 2 values.");

        if (snapPoint.Type == SnapPoint.SnapPointType.Enter)
        {
            EnterSnapPoint = snapPoint;
        }
        else
        {
            ExitSnapPoint = snapPoint;
        }
    }

    private async UniTask WaitForAsyncObjects()
    {
        var sceneObjects = GetComponentsInChildren<IAsyncLoadObject>();
        var waitTasks = sceneObjects
            .Select(obj => UniTask.WaitUntil(() => obj.IsLoaded, cancellationToken: this.GetCancellationTokenOnDestroy()))
            .ToArray();

        await UniTask.WhenAll(waitTasks);
    }
}