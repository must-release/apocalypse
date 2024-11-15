using UnityEngine;
using CharacterEums;

public class AttackingUpperState : MonoBehaviour, IPlayerUpperState
{
    private Transform playerTransform;
    private PlayerController playerController;
    private float attackCoolTime;

    // Start is called before the first frame update

    public void Start()
    {
        playerTransform = transform.parent.parent;
        playerController = playerTransform.GetComponent<PlayerController>();
        playerController.AddUpperState(CHARACTER_UPPER_STATE.ATTACKING, this);
    }

    public CHARACTER_UPPER_STATE GetState() { return CHARACTER_UPPER_STATE.ATTACKING; }

    public void StartState()
    {
        attackCoolTime = playerController.CurrentPlayer.Attack();
    }
    public void UpdateState()
    {
        attackCoolTime -= Time.deltaTime;
        if (attackCoolTime < 0)
        {
            if (playerController.StandingGround == null)
            {
                playerController.ChangeUpperState(CHARACTER_UPPER_STATE.JUMPING);
            }
            else
            {
                playerController.ChangeUpperState(CHARACTER_UPPER_STATE.IDLE);
            }
        }
    }
    public void EndState()
    {
        
    }
    public void Disable() { playerController.ChangeUpperState(CHARACTER_UPPER_STATE.DISABLED); }
    public void LookUp(bool lookUp) { if(lookUp) playerController.ChangeUpperState(CHARACTER_UPPER_STATE.LOOKING_UP);}
    public void Aim(Vector3 aim)
    {
        if(aim != Vector3.zero && playerController.StandingGround != null)
            playerController.ChangeUpperState(CHARACTER_UPPER_STATE.AIMING);
    }
    public void Attack() { playerController.ChangeUpperState(CHARACTER_UPPER_STATE.ATTACKING); }
    public void Stop() { return; }
    public void Jump() { return; }
    public void OnAir() { return; }
    public void Move() { return; }
    public void OnGround() { return; }
    public void Enable() {return;}
}
