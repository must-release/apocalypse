using UnityEngine;
using CharacterEums;

public class JumpingUpperState : MonoBehaviour, IPlayerUpperState
{
    private Transform playerTransform;
    private PlayerController playerController;

    // Start is called before the first frame update

    public void Start()
    {
        playerTransform = transform.parent.parent;
        playerController = playerTransform.GetComponent<PlayerController>();
        playerController.AddUpperState(CHARACTER_UPPER_STATE.JUMPING, this);
    }

    public CHARACTER_UPPER_STATE GetState() { return CHARACTER_UPPER_STATE.JUMPING; }

    public void StartState()
    {

    }
    public void UpdateState()
    {

    }
    public void EndState()
    {

    }
    public void Disable() { playerController.ChangeUpperState(CHARACTER_UPPER_STATE.DISABLED); }
    public void OnGround() { playerController.ChangeUpperState(CHARACTER_UPPER_STATE.IDLE); }
    public void LookUp(bool lookUp) { if(lookUp) playerController.ChangeUpperState(CHARACTER_UPPER_STATE.LOOKING_UP);}
    public void Attack() { playerController.ChangeUpperState(CHARACTER_UPPER_STATE.ATTACKING); }
    public void Aim(Vector3 aim) { return; }
    public void Stop() { return; }
    public void Move() { return; }
    public void Jump() { return; }
    public void OnAir() { return; }
    public void Enable() {return;}
}
