using UnityEngine;

public class PlayerStart : MonoBehaviour, IStageElement
{
    public Vector3 StartPosition => transform.position + new Vector3(0f, 0.5f);
}
