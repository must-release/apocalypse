using System;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(Collider2D))]
public class StageTransitionTrigger : MonoBehaviour, IStageElement
{
    /****** Public Members ******/

    public event Action OnStageTransitionTriggered;

    public void RegisterTransitionEvent(Action transitionEvent)
    {
        Assert.IsTrue(null != transitionEvent, "Cannot register null transition event.");
        OnStageTransitionTriggered += transitionEvent;
    }

    public void UnregisterTransitionEvent(Action transitionEvent)
    {
        Assert.IsTrue(null != transitionEvent, "Cannot unregister null transition event.");
        OnStageTransitionTriggered -= transitionEvent;
    }


    /****** Private Members ******/
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out IClimber climber))
        {
            OnStageTransitionTriggered?.Invoke();
        }
    }
}