using NUnit.Framework;
using UnityEngine;

public class SnapPoint : MonoBehaviour
{
    /****** Public Members ******/

    public enum SnapPointType { Enter, Exit, SnapPointTypeCount }
    public enum SnapDirection { Up, Down, Left, Right, SnapDirectionCount }

    public SnapPointType Type => _type;
    public SnapDirection Direction => _direction;

    public Vector3 GetPairPosition()
    {
        Assert.IsTrue(2 == (int)SnapPointType.SnapPointTypeCount, "SnapPointType enum should have exactly 2 values.");

        switch (_type)
        {
            case SnapPointType.Enter:
                return transform.position - GetDirectionOffset(_direction);
            case SnapPointType.Exit:
                return transform.position + GetDirectionOffset(_direction);
            default:
                return Vector3.zero;
        }
    }


    /****** Private Members ******/

    [SerializeField] private SnapPointType _type;
    [SerializeField] private SnapDirection _direction;

    private Vector3 GetDirectionOffset(SnapDirection direction)
    {
        Assert.IsTrue(4 == (int)SnapDirection.SnapDirectionCount, "SnapDirection enum should have exactly 4 values.");

        switch (direction)
        {
            case SnapDirection.Up:
                return new Vector3(0, 1, 0);
            case SnapDirection.Down:
                return new Vector3(0, -1, 0);
            case SnapDirection.Left:
                return new Vector3(-1, 0, 0);
            case SnapDirection.Right:
                return new Vector3(1, 0, 0);
            default:
                return Vector3.zero;
        }
    }
}
