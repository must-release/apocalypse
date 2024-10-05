using CharacterEums;
using UnityEngine;

public class DisabledUpperState : MonoBehaviour, IPlayerUpperState
{
    private Transform playerTransform;
    private PlayerController playerController;

    // Start is called before the first frame update

    public void Start()
    {
        playerTransform = transform.parent.parent;
        playerController = playerTransform.GetComponent<PlayerController>();
        playerController.AddUpperState(CHARACTER_UPPER_STATE.DISABLED, this);
    }

    public CHARACTER_UPPER_STATE GetState() { return CHARACTER_UPPER_STATE.DISABLED; }

    public void StartState()
    {

    }
    public void UpdateState()
    {

    }
    public void EndState()
    {

    }
    public void Enable()
    {
        if(playerController.StandingGround != null)
        {
            playerController.ChangeUpperState(CHARACTER_UPPER_STATE.IDLE);
        }
        else
        {
            playerController.ChangeUpperState(CHARACTER_UPPER_STATE.JUMPING);
        }
    }
    public void Move() { return; }
    public void Jump() { return; }
    public void OnAir() { return; }
    public void LookUp(bool lookUp) { return; }
    public void Aim(Vector3 aim) {return;}
    public void Attack() { return;}
    public void Stop() { return; }
    public void Disable() { return; }
    public void OnGround() { return; }
}
