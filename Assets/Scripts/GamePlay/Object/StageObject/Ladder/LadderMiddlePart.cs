using UnityEngine;
using UnityEngine.Assertions;

public class LadderMiddlePart : LadderPart
{
    /****** Private Members ******/

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Assert.IsTrue(null != OnClimberEnter, "On Climber Enter action is not assigned in the LadderTopPart");

        if (collision.gameObject.TryGetComponent(out IClimber climber))
        {
            OnClimberEnter.Invoke(climber);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Assert.IsTrue(null != OnClimberExit, "On Climber exit action is not assigned in the LadderTopPart");

        if (collision.gameObject.TryGetComponent(out IClimber climber))
        {
            OnClimberExit.Invoke(climber);
        }
    }
}
