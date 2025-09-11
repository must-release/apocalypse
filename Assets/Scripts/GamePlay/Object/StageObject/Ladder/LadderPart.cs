using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Tilemaps;
using System; // Add this for Action

public abstract class LadderPart : MonoBehaviour, IPartObject
{
    /****** Public Members ******/

    public Action<IObjectClimber> OnClimberEnter;
    public Action<IObjectClimber> OnClimberExit;

    public ICompositeObject CreateCompositeObjectFrame()
    {
        Debug.Assert(null != transform.parent.GetComponent<Tilemap>(), "Ladder part must be a child of a Tilemap.");

        GameObject compositeObject = new GameObject("LadderObject");
        compositeObject.transform.parent = transform.parent;
        return compositeObject.AddComponent<LadderComposite>();
    }
    
    public bool IsPartOfSameCompositeObject(IPartObject partObject)
    {
        return partObject is LadderPart;
    }
}

