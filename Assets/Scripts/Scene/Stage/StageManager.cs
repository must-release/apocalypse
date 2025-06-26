using Cysharp.Threading.Tasks;
using NUnit.Framework;
using System.Linq;
using Unity.VisualScripting;
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

    public async UniTask AsyncInitializeStage()
    {
        InitializeTilemap();

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

    [SerializeField] private ChapterType _chapterType;
    [SerializeField] private int _stageIndex;

    private Tilemap _tilemap;
    private PlayerStart _playerStart;

    private void InitializeTilemap()
    {
        _tilemap = GetComponentInChildren<Tilemap>();
        Assert.IsTrue(null != _tilemap, $"Tilemap component is missing in the {_chapterType}_{_stageIndex}.");

        BoundsInt bounds    = _tilemap.cellBounds;
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

            _tilemap.SetTile(cellPos, null);
            Vector3 worldPos = _tilemap.GetCellCenterWorld(cellPos);
            Instantiate(replacementTile.ReplacingObject, worldPos, Quaternion.identity, _tilemap.transform);
        }
    }
}