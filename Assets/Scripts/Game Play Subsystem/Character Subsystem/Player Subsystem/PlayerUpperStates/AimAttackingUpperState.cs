using UnityEngine;
using CharacterEums;

public class AimAttackingUpperState : MonoBehaviour, IPlayerUpperState
{
    private Transform playerTransform;
    private PlayerController playerController;
    private float attackCoolTime;
    private Vector3 aimingPosition;
    private int direction;

    public void Start()
    {
        playerTransform = transform.parent.parent;
        playerController = playerTransform.GetComponent<PlayerController>();
        playerController.AddUpperState(CHARACTER_UPPER_STATE.AIM_ATTACKING, this);
    }

    public CHARACTER_UPPER_STATE GetState() { return CHARACTER_UPPER_STATE.AIM_ATTACKING; }

    public void StartState()
    {
        attackCoolTime = playerController.CurrentPlayer.Attack();
    }
    public void UpdateState()
    {
        attackCoolTime -= Time.deltaTime;
        if (attackCoolTime < 0)
        {
            playerController.ChangeUpperState(CHARACTER_UPPER_STATE.AIMING);
        }

        direction = aimingPosition.x > playerTransform.position.x ? 1 : -1;
        playerTransform.localScale = new Vector3(direction * Mathf.Abs(playerTransform.localScale.x),
            playerTransform.localScale.y, playerTransform.localScale.z);
        playerController.CurrentPlayer.RotateUpperBody(aimingPosition);
    }
    public void EndState()
    {

    }
    public void Disable() { playerController.ChangeUpperState(CHARACTER_UPPER_STATE.DISABLED); }
    public void Attack() { playerController.ChangeUpperState(CHARACTER_UPPER_STATE.AIM_ATTACKING); }
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
    public void LookUp(bool lookUp) { return;}
    public void Stop() { return; }
    public void Jump() { return; }
    public void OnAir() { return; }
    public void Move() { return; }
    public void OnGround() { return; }
    public void Enable() {return;}
}
