using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using AD.Camera;
using AD.GamePlay;

using static SnapPoint;


public class StageManager : MonoBehaviour
{
    /****** Public Members ******/

    public Vector3          PlayerStartPosition
    {
        get
        {
            Debug.Assert(null != _playerStart, $"PlayerStart component is missing in the {_chapterType}_{_stageIndex}.");
            return _playerStart.StartPosition;
        }
    }
    public SnapPoint        EnterSnapPoint  { get; private set; }
    public SnapPoint        ExitSnapPoint   { get; private set; }
    public BoxCollider2D    StageBoundary   { get; private set; }
    public ChapterType      ChapterType     => _chapterType;
    public int              StageIndex      => _stageIndex;

    public bool CanGoBackToPreviousStage => _canGoBackToPreviousStage;

    public async UniTask AsyncInitializeStage()
    {
        InitializeTilemap();
        SetStageBoundary();
        SetupCameras();
        await WaitForAsyncObjects();

        Debug.Assert(null != _playerStart, $"PlayerStart component is missing in the {_chapterType}_{_stageIndex}.");
        Debug.Assert(null != EnterSnapPoint, $"EnterSnapPoint component is missing in the {_chapterType}_{_stageIndex}.");
        Debug.Assert(null != ExitSnapPoint, $"ExitSnapPoint component is missing in the {_chapterType}_{_stageIndex}.");
    }

    public void DestroyStage()
    {
        Destroy(gameObject);
    }

    public void BlockReturnToPreviousStage()
    {
        Debug.Assert(null != EnterSnapPoint, $"EnterSnapPoint is not set in the {_chapterType}_{_stageIndex}.");
        Debug.Assert(false == _canGoBackToPreviousStage, $"Cannot block return to previous stage in the {_chapterType}_{_stageIndex}. Going back to previous stage is allowed.");
        Debug.Assert(null != _tilemap, $"Tilemap component is missing it the {_chapterType}_{_stageIndex}.");
        Debug.Assert(4 == (int)SnapDirection.SnapDirectionCount, "SnapDirection enum should have exactly 4 values.");

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
        Debug.Assert(null != targetPoint, $"Trying to snap null point in the {_chapterType}_{_stageIndex}.");
        Debug.Assert(null != EnterSnapPoint, $"EnterSnapPoint is not set in the {_chapterType}_{_stageIndex}.");
        Debug.Assert(null != ExitSnapPoint, $"ExitSnapPoint is not set in the {_chapterType}_{_stageIndex}.");
        Debug.Assert(2 == (int)SnapPointType.SnapPointTypeCount, "SnapPointType enum should have exactly 2 values.");

        Vector3 moveVec = (targetPoint.Type == SnapPointType.Enter)
            ? targetPoint.GetPairPosition() - ExitSnapPoint.transform.position
            : targetPoint.GetPairPosition() - EnterSnapPoint.transform.position;

        transform.Translate(moveVec);
    }

    public ICamera[] GetStageCameras()
    {
        Debug.Assert(null != _camerasContainer, $"GameCameras container is not set in {_chapterType}_{_stageIndex}.");
        
        return _camerasContainer.GetComponentsInChildren<ICamera>();
    }

    public IActor[] GetStageActors()
    {
        Debug.Assert(null != _actorsContainer, $"Actors container is not set in {_chapterType}_{_stageIndex}.");

        return _actorsContainer.GetComponentsInChildren<IActor>();
    }

    /****** Private Members ******/

    [Header("Stage Settings")]
    [SerializeField] private ChapterType    _chapterType;
    [SerializeField] private int            _stageIndex;
    [SerializeField] private bool           _canGoBackToPreviousStage;
    [SerializeField] private Transform      _camerasContainer;
    [SerializeField] private Transform      _actorsContainer;

    private Tilemap     _tilemap;
    private PlayerStart _playerStart;

    private void OnValidate()
    {
        Debug.Assert(null != _camerasContainer, $"Cameras container is not set in {_chapterType}_{_stageIndex}.");
        Debug.Assert(null != _camerasContainer.GetComponentInChildren<FollowCamera>(), $"GameCameras container should have follow camera in {_chapterType}_{_stageIndex}.");
        Debug.Assert(null != _actorsContainer, $"Actors container is not set in {_chapterType}_{_stageIndex}.");

        gameObject.name = $"{_chapterType}_{_stageIndex}";
    }

    private void InitializeTilemap()
    {
        _tilemap = GetComponentInChildren<Tilemap>();
        Debug.Assert(null != _tilemap, $"Tilemap component is missing in the {_chapterType}_{_stageIndex}.");

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
                else if (stageElement is IPartObject partObject)
                {
                    SearchPartsAndMakeCompositeObject(i, visitedTile, partObject);
                }
            }
        }
    }

    private void SetPlayerStart(PlayerStart playerStart)
    {
        Debug.Assert(null != playerStart, "PlayerStart cannot be null.");
        _playerStart = playerStart;
    }

    private void SetSnapPoint(SnapPoint snapPoint)
    {
        Debug.Assert(null != snapPoint, "SnapPoint cannot be null.");
        Debug.Assert(2 == (int)SnapPointType.SnapPointTypeCount, "SnapPointType enum should have exactly 2 values.");

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
        Debug.Assert(null != _tilemap, $"Tilemap is missing in the {_chapterType}_{_stageIndex}.");

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

    private void SetStageBoundary()
    {
        StageBoundary = GetComponentInChildren<StageBoundary>()?.GetComponent<BoxCollider2D>();
        Debug.Assert(null != StageBoundary, $"StageBoundary is not set in the {_chapterType}_{_stageIndex}.");
    }

    private void SetupCameras()
    {
        Debug.Assert(null != StageBoundary, $"StageBoundary is not set in the {_chapterType}_{_stageIndex}.");
        Debug.Assert(null != _camerasContainer, $"Cameras container is not set in {_chapterType}_{_stageIndex}.");

        var cameras = _camerasContainer.GetComponentsInChildren<IGamePlayCamera>();
        cameras.ToList().ForEach(camera => camera.Initialize(StageBoundary));

        Logger.Write(LogCategory.GameScene, $"Initialized {cameras.Length} cameras from GameCameras container in {_chapterType}_{_stageIndex}.", LogLevel.Log, true);
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