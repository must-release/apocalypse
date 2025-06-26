using UnityEngine;

public class KillZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out ICharacter character))
        {
            if (character.IsPlayer)
            {
                GameEventManager.Instance.Submit(GameEventFactory.CreateCommonEvent(CommonEventType.Continue));
            }
        }
    }
}
