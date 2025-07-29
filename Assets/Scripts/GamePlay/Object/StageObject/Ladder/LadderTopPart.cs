using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;

public class LadderTopPart : LadderPart
{
    /****** Public Members ******/

    public void IgnoreCollisionWithClimber(IClimber climber, bool value)
    {
        StartCoroutine(AsyncIgnoreColiision(climber, value));
    }

    /****** Private Members ******/
    [SerializeField] private Collider2D _topTrigger;

    private HashSet<IClimber> _enteredClimbers = new();
    private Collider2D _bodyCollider;

    private void Awake()
    {
        Debug.Assert(null != _topTrigger, "Trigger is not assigned.");

        _bodyCollider = GetComponent<Collider2D>();

        if (false == _topTrigger.isTrigger)
        {
            Logger.Write(LogCategory.GamePlay, "The Trigger Collider of LadderTopPart is not set to 'Is Trigger'. Setting it automatically.", LogLevel.Warning);
            _topTrigger.isTrigger = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Assert(null != OnClimberEnter, "On Climber Enter action is not assigned in the LadderTopPart");

        if (collision.gameObject.TryGetComponent(out IClimber climber))
        {
            OnClimberEnter.Invoke(climber);
            _enteredClimbers.Add(climber);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Assert(null != OnClimberExit, "On Climber exit action is not assigned in the LadderTopPart");

        if (collision.gameObject.TryGetComponent(out IClimber climber))
        {
            OnClimberExit.Invoke(climber);
            _enteredClimbers.Remove(climber);
        }
    }

    private IEnumerator AsyncIgnoreColiision(IClimber climber, bool value)
    {
        if (false == value)
        {
            yield return new WaitWhile(() => _enteredClimbers.Contains(climber));
        }

        Physics2D.IgnoreCollision(climber.ClimberCollider, _bodyCollider, value);
    }
}
