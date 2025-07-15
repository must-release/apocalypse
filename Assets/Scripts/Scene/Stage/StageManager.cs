using Cysharp.Threading.Tasks;
using UnityEngine.Assertions;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

using static SnapPoint;

public class StageManager : MonoBehaviour
{
    /****** Public Members ******/

    public Vector3      PlayerStartPosition
    {
        get
        {
            Assert.IsTrue(null != _playerStart, $"PlayerStart component is missing in the {_chapterType}_{_stageIndex}.");
            return _playerStart.StartPosition;
        }
    }
    public SnapPoint    EnterSnapPoint { get; private set; }
    public SnapPoint    ExitSnapPoint  { get; private set; }
    
    public bool CanGoBackToPreviousStage => _canGoBackToPreviousStage;

    public async UniTask AsyncInitializeStage()
    {
        InitializeTilemap();
        SetStageTransitionInfo();
        await WaitForAsyncObjects();

        Assert.IsTrue(null != _playerStart, $"PlayerStart component is missing in the {_chapterType}_{_stageIndex}.");
        Assert.IsTrue(null != EnterSnapPoint, $"EnterSnapPoint component is missing in the {_chapterType}_{_stageIndex}.");
        Assert.IsTrue(null != ExitSnapPoint, $"ExitSnapPoint component is missing in the {_chapterType}_{_stageIndex}.");
    }

    public void DestroyStage()
    {
        Destroy(gameObject);
    }

    public void SetupEntranceBarrier()
    {
        Assert.IsTrue(null != EnterSnapPoint, $"EnterSnapPoint is not set in the {_chapterType}_{_stageIndex}.");
        Assert.IsTrue(false == _canGoBackToPreviousStage, $"Cannot setup entrance barrier in the {_chapterType}_{_stageIndex}. Going back to previous stage is allowed.");
        Assert.IsTrue(null != _tilemap, $"Tilemap component is missing it the {_chapterType}_{_stageIndex}.");
        Assert.IsTrue(4 == (int)SnapDirection.SnapDirectionCount, "SnapDirection enum should have exactly 4 values.");

        GameObject      barrier     = new GameObject("EntranceBarrier");
        BoxCollider2D   collider    = barrier.AddComponent<BoxCollider2D>();
        barrier.transform.SetParent(gameObject.transform, false);
        barrier.transform.position = EnterSnapPoint.GetPairPosition();

        BoundsInt   bounds  = _tilemap.cellBounds;
        Vector3     barrPos = barrier.transform.position;

        if (EnterSnapPoint.Direction == SnapDirection.Left || EnterSnapPoint.Direction == SnapDirection.Right)
        {
            collider.size   = new Vector2(1, bounds.size.y);
            barrPos.y       = transform.position.y;
        }
        else
        {
            collider.size   = new Vector2(bounds.size.x, 1);
            barrPos.x       = transform.position.x;
        }
        barrier.transform.position = barrPos;
    }

    public void SnapToPoint(SnapPoint targetPoint)
    {
        Assert.IsTrue(null != targetPoint, $"Trying to snap null point in the {_chapterType}_{_stageIndex}.");
        Assert.IsTrue(null != EnterSnapPoint, $"EnterSnapPoint is not set in the {_chapterType}_{_stageIndex}.");
        Assert.IsTrue(null != ExitSnapPoint, $"ExitSnapPoint is not set in the {_chapterType}_{_stageIndex}.");
        Assert.IsTrue(2 == (int)SnapPointType.SnapPointTypeCount, "SnapPointType enum should have exactly 2 values.");

        Vector3 moveVec = (targetPoint.Type == SnapPointType.Enter)
            ? targetPoint.GetPairPosition() - ExitSnapPoint.transform.position
            : targetPoint.GetPairPosition() - EnterSnapPoint.transform.position;

        transform.Translate(moveVec);
    }


    /****** Private Members ******/

    [Header("Stage Settings")]
    [SerializeField] private ChapterType    _chapterType;
    [SerializeField] private int            _stageIndex;
    [SerializeField] private bool           _canGoBackToPreviousStage;

    private const int _StageTranisitionTriggerCount = 2;

    private List<StageTransitionTrigger> _stageTransitionTriggers = new List<StageTransitionTrigger>();
    private Tilemap                     _tilemap;
    private PlayerStart                 _playerStart;

    private void OnValidate()
    {
        gameObject.name = $"{_chapterType}_{_stageIndex}";
    }

    private void InitializeTilemap()
    {
        _tilemap = GetComponentInChildren<Tilemap>();
        Assert.IsTrue(null != _tilemap, $"Tilemap component is missing in the {_chapterType}_{_stageIndex}.");

        BoundsInt   bounds      = _tilemap.cellBounds;
        TileBase[]  allTiles    = _tilemap.GetTilesBlock(bounds);
        bool[]      visitedTile = new bool[allTiles.Length];

        int width = bounds.size.x;

        for (int i = 0; i < allTiles.Length; i++)
        {
            if (visitedTile[i])
                continue;

            visitedTile[i] = true;
            TileBase tile = allTiles[i];
            ObjectReplacementTile replacementTile = tile as ObjectReplacementTile;
            if (null == replacementTile)
                continue;

            int x = i % width;
            int y = i / width;
            Vector3Int cellPos = new Vector3Int(bounds.x + x, bounds.y + y, bounds.z);

            replacementTile.HideTile();
            replacementTile.RefreshTile(cellPos, _tilemap);

            Vector3     worldPos        = _tilemap.GetCellCenterWorld(cellPos);
            GameObject  replacingObject = Instantiate(replacementTile.ReplacingObject, worldPos, Quaternion.identity, _tilemap.transform);

            if (replacingObject.TryGetComponent(out IStageElement stageElement))
            {
                if (stageElement is PlayerStart playerStart)
                {
                    SetPlayerStart(playerStart);
                }
                else if (stageElement is SnapPoint snapPoint)
                {
                    SetSnapPoint(snapPoint);
                }
                else if (stageElement is StageTransitionTrigger transitionTrigger)
                {
                    Assert.IsTrue(_stageTransitionTriggers.Count < _StageTranisitionTriggerCount, $"More than {_StageTranisitionTriggerCount} stage transition trigger is found.");
                    _stageTransitionTriggers.Add(transitionTrigger);
                }
                else if (stageElement is IPartObject partObject)
                {
                    SearchPartsAndMakeCompositeObject(i, visitedTile, partObject);
                }
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
        Assert.IsTrue(2 == (int)SnapPointType.SnapPointTypeCount, "SnapPointType enum should have exactly 2 values.");

        if (snapPoint.Type == SnapPointType.Enter)
        {
            EnterSnapPoint = snapPoint;
        }
        else
        {
            ExitSnapPoint = snapPoint;
        }
    }

    private void SearchPartsAndMakeCompositeObject(int curTileIdx, bool[] visitedTile, IPartObject partObject)
    {
        Assert.IsTrue(null != _tilemap, $"Tilemap is missing in the {_chapterType}_{_stageIndex}.");

        BoundsInt           bounds      = _tilemap.cellBounds;
        TileBase[]          allTiles    = _tilemap.GetTilesBlock(bounds);
        ICompositeObject    composite   = partObject.CreateCompositeObjectFrame();
        Queue<Vector3Int>   queue       = new();


        int width   = bounds.size.x;
        int height  = bounds.size.y;
        int xPos    = curTileIdx % width;
        int yPos    = curTileIdx / width;
        int[] dx    = { 1, 0, -1, 0 };
        int[] dy    = { 0, 1, 0, -1 };

        composite.AddPart(partObject);
        queue.Enqueue(new Vector3Int(bounds.x + xPos, bounds.y + yPos, bounds.z));
        while (0 < queue.Count)
        {
            Vector3Int pos = queue.Dequeue();
            
            for (int i = 0; i < 4; ++i)
            {
                Vector3Int nextPos = pos;
                nextPos.x += dx[i];
                nextPos.y += dy[i];
                int localX = nextPos.x - bounds.xMin;
                int localY = nextPos.y - bounds.yMin;
                int idx = localY * width + localX;

                if (bounds.Contains(nextPos)
                    && false == visitedTile[idx]
                    && allTiles[idx] is ObjectReplacementTile replacementTile
                    && replacementTile.ReplacingObject.TryGetComponent(out IPartObject otherPart)
                    && partObject.IsPartOfSameCompositeObject(otherPart)
                )
                {
                    visitedTile[idx] = true;

                    replacementTile.HideTile();
                    replacementTile.RefreshTile(nextPos, _tilemap);

                    Vector3 worldPos = _tilemap.GetCellCenterWorld(nextPos);
                    GameObject replacingObject = Instantiate(replacementTile.ReplacingObject, worldPos, Quaternion.identity, _tilemap.transform);

                    composite.AddPart(replacingObject.GetComponent<IPartObject>());
                    queue.Enqueue(nextPos);
                }
            }
        }

        composite.Initialize();
    }

    private void SetStageTransitionInfo()
    {
        Assert.IsTrue(null != EnterSnapPoint, $"EnterSnapPoint is not set in the {_chapterType}_{_stageIndex}.");
        Assert.IsTrue(null != ExitSnapPoint, $"ExitSnapPoint is not set in the {_chapterType}_{_stageIndex}.");
        Assert.IsTrue(_stageTransitionTriggers.Count <= _StageTranisitionTriggerCount, $"Stage transition triggers are not set correctly in the {_chapterType}_{_stageIndex}.");

        foreach (var trigger in _stageTransitionTriggers)
        {
            var distFromEnter = Vector3.Distance(trigger.transform.position, EnterSnapPoint.transform.position);
            var distFromExit = Vector3.Distance(trigger.transform.position, ExitSnapPoint.transform.position);

            if (distFromEnter < distFromExit)
            {
                if (ChapterStageCount.IsStageIndexValid(_chapterType, _stageIndex - 1))
                {
                    ChapterType targetChapter = _chapterType;
                    int targetStage = _stageIndex - 1;
                    trigger.RegisterTransitionEvent(() => CreateAndPlayStageTransitionEvent(targetChapter, targetStage));
                }
            }
            else
            {
                if (ChapterStageCount.IsStageIndexValid(_chapterType, _stageIndex + 1))
                {
                    ChapterType targetChapter = _chapterType;
                    int targetStage = _stageIndex + 1;
                    trigger.RegisterTransitionEvent(() => CreateAndPlayStageTransitionEvent(targetChapter, targetStage));
                }
            }
        }
    }

    private void CreateAndPlayStageTransitionEvent(ChapterType targetChapter, int targetStage)
    {
        var stageTransitionEvent = GameEventFactory.CreateStageTransitionEvent(targetChapter, targetStage);
        GameEventManager.Instance.Submit(stageTransitionEvent);
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