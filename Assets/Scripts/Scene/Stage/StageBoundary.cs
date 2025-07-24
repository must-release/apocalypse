using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
class StageBoundary : MonoBehaviour
{
    /****** Private Members ******/

    [SerializeField] private Vector2 _size      = new Vector2(10f, 10f);
    [SerializeField] private Vector2 _offset    = Vector2.zero;

    private BoxCollider2D _boxCollider;

    private void OnValidate()
    {
        if (null == _boxCollider)
        {
            _boxCollider = GetComponent<BoxCollider2D>();
        }

        _boxCollider.size       = _size;
        _boxCollider.offset     = _offset;
        _boxCollider.isTrigger  = true;
    }
}