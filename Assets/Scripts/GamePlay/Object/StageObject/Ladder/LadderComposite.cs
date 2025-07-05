using UnityEngine;
using UnityEngine.Assertions;
using System.Collections.Generic;

public class LadderComposite : MonoBehaviour, IStageObject, ICompositeObject, IClimbable
{
    /****** Public Members ******/

    public void AddPart(IPartObject partObject)
    {
        LadderPart ladderPart = partObject as LadderPart;
        Assert.IsTrue(null != ladderPart, $"{partObject} is not a ladder part.");

        ladderPart.transform.SetParent(transform, true);
        ladderPart.OnClimberEnter += HandleClimberEnter;
        ladderPart.OnClimberExit += HandleClimberExit;

        if (partObject is LadderTopPart) _topPart = partObject as LadderTopPart;
        else if (partObject is LadderDownPart) _downPart = partObject as LadderDownPart;
    }

    public void Initialize()
    {
        Assert.IsTrue(null != _topPart, "Top part of the ladder is missing.");
        Assert.IsTrue(null != _downPart, "Down part of the ladder is missing.");
    }

    public Vector3 GetClimbReferencePoint()
    {
        return _topPart.transform.position;
    }

    public void OnClimbStart(IClimber climber)
    {
        Assert.IsTrue(_climberCounts.ContainsKey(climber), "Unknown climber is trying to climb the ladder");

        Debug.Log(1);

        _topPart.IgnoreCollisionWithClimber(climber, true);
    }

    public void OnClimbEnd(IClimber climber)
    {
        Assert.IsTrue(_climberCounts.ContainsKey(climber), "Unknown climber is trying to stop climbing the ladder");

        Debug.Log(2);

        _topPart.IgnoreCollisionWithClimber(climber, false);
    }

    public bool CanClimbFurther(Vector3 climberPosition, VerticalDirection moveDirection)
    {
        bool isLowerThanLadder = climberPosition.y < _downPart.transform.position.y && VerticalDirection.Down == moveDirection;
        bool isUpperThanLadder = _topPart.transform.position.y < climberPosition.y && VerticalDirection.Up == moveDirection;

        return false == isLowerThanLadder && false == isUpperThanLadder;
    }

    /****** Private Members ******/

    private readonly Dictionary<IClimber, int> _climberCounts = new Dictionary<IClimber, int>();

    private LadderTopPart _topPart;
    private LadderDownPart _downPart;

    private void HandleClimberEnter(IClimber climber)
    {
        Assert.IsTrue(null != climber, "Climber is null in the ladderComposite.");

        if (false == _climberCounts.ContainsKey(climber))
        {
            climber.CurrentClimbableObject = this;
            _climberCounts.Add(climber, 1);
        }
        else
        {
            _climberCounts[climber]++;
        }
    }

    private void HandleClimberExit(IClimber climber)
    {
        Assert.IsTrue(null != climber, "Climber is null in the ladderComposite.");
        Assert.IsTrue(_climberCounts.ContainsKey(climber), "Unknown climber is trying to exit the ladder.");

        _climberCounts[climber]--;

        if (0 >= _climberCounts[climber])
        {
            _climberCounts.Remove(climber);
            if (Object.ReferenceEquals(climber.CurrentClimbableObject, this))
            {
                climber.CurrentClimbableObject = null;
            }
        }
    }
}
