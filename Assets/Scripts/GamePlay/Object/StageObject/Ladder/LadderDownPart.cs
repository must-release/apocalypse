using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(BoxCollider2D))]
public class LadderDownPart : LadderPart
{
    /****** Private Members ******/

    private void Awake()
    {
        BoxCollider2D downTrigger = GetComponent<BoxCollider2D>();
        downTrigger.isTrigger = true;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {

    }

    private void OnTriggerExit2D(Collider2D collision)
    {

    }
}
