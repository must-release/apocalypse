using UnityEngine;
using CharacterEums;

public class AimingUpperState : MonoBehaviour, IPlayerUpperState
{
    private Transform playerTransform;
    private PlayerController playerController;
    private Vector3 aimingPosition;
    private int direction;

    // Start is called before the first frame update

    public void Start()
    {
        playerTransform = transform.parent.parent;
        playerController = playerTransform.GetComponent<PlayerController>();
        playerController.AddUpperState(CHARACTER_UPPER_STATE.AIMING, this);
    }

    public CHARACTER_UPPER_STATE GetState() { return CHARACTER_UPPER_STATE.AIMING; }

    public void StartState()
    {

    }
    public void UpdateState()
    {
        direction = aimingPosition.x > playerTransform.position.x ? 1 : -1;
        playerTransform.localScale = new Vector3(direction * Mathf.Abs(playerTransform.localScale.x),
            playerTransform.localScale.y, playerTransform.localScale.z);
        playerController.CurrentPlayer.RotateUpperBody(aimingPosition);
    }
    public void EndState()
    {

    }
    public void Disable() { playerController.ChangeUpperState(CHARACTER_UPPER_STATE.DISABLED); }
    public void OnAir() { playerController.ChangeUpperState(CHARACTER_UPPER_STATE.JUMPING); }
    public void Aim(Vector3 aim)
    {
        if(aim == Vector3.zero)
        {
            playerController.ChangeUpperState(CHARACTER_UPPER_STATE.IDLE); 
            playerController.CurrentPlayer.RotateUpperBody(0);
        }  
        else
            aimingPosition = aim;
    }
    public void Attack()
    {
        playerController.ChangeUpperState(CHARACTER_UPPER_STATE.AIM_ATTACKING);
    }
    public void Jump() { return; }
    public void Stop() { return; }
    public void LookUp(bool lookUp) { return; }
    public void Move() { return; }
    public void OnGround() { return; }
    public void Enable() {return;}
}
