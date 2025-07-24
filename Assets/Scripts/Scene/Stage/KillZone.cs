using UnityEngine;
using Cysharp.Threading.Tasks;

public class KillZone : MonoBehaviour
{
    private const int _KillDelay = 500; // 0.5 seconds

    private async void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out ICharacter character))
        {
            if (character.IsPlayer)
            {
                await UniTask.Delay(_KillDelay, cancellationToken: this.GetCancellationTokenOnDestroy());
                GameEventManager.Instance.Submit(GameEventFactory.CreateCommonEvent(CommonEventType.Continue));
            }
        }
    }
}
