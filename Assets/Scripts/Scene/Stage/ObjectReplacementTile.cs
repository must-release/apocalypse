using NUnit.Framework;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "NewObjectReplacementTile", menuName = "2D/Tiles/Object Replacement Tile")]
public class ObjectReplacementTile : TileBase
{
    /****** Public Members ******/

    public GameObject ReplacingObject => gameObject;

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        tileData.sprite         = _sprite;
        tileData.color          = _color;
        tileData.flags          = _flags;
        tileData.colliderType   = _colliderType;
    }

    public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject instantiated)
    {
        _color = Color.white;

        return false;
    }

    public void HideTile()
    {
        _color = Color.clear;
    }


    /****** Private Members ******/

    [Header("Appearance")]
    [SerializeField] private Sprite _sprite;
    [SerializeField] private Color _color = Color.white;

    [Header("Behavior")]
    [SerializeField] private TileFlags _flags = TileFlags.LockAll;
    [SerializeField] private Tile.ColliderType _colliderType = Tile.ColliderType.Sprite;

    [Header("Replacing Prefab")]
    [SerializeField] private GameObject gameObject;
}
