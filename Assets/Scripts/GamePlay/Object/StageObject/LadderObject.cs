using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class LadderObject : MonoBehaviour, IStageObject
{
    /****** Public Members ******/


    /****** Private Members ******/

    private enum LaddderType { Top, Middle, Bottom }

    [SerializeField] private LaddderType _type;

    private BoxCollider2D _collider;

    private void Awake()
    {
        _collider = GetComponent<BoxCollider2D>();
        _collider.isTrigger = true;

        if (LaddderType.Bottom == _type) SetBottonLadder();
        else if (LaddderType.Top == _type) SetTopLadder();
    }

    private void SetTopLadder()
    {
        
    }

    private void SetBottonLadder()
    {

    }
}