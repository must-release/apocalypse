using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Tilemaps;
using System; // Add this for Action

public abstract class LadderPart : MonoBehaviour, IPartObject
{
    /****** Public Members ******/

    public Action<IClimber> OnClimberEnter;
    public Action<IClimber> OnClimberExit;

    public ICompositeObject CreateCompositeObjectFrame()
    {
        Assert.IsTrue(null != transform.parent.GetComponent<Tilemap>(), "Ladder part must be a child of a Tilemap.");

        GameObject compositeObject = new GameObject("LadderObject");
        compositeObject.transform.parent = transform.parent;
        return compositeObject.AddComponent<LadderComposite>();
    }
    
    public bool IsPartOfSameCompositeObject(IPartObject partObject)
    {
        return partObject is LadderPart;
    }
}

