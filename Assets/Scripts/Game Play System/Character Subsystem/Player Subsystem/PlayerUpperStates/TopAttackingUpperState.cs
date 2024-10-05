using UnityEngine;
using CharacterEums;

public class TopAttackingUpperState : MonoBehaviour, IPlayerUpperState
{
    private Transform playerTransform;
    private PlayerController playerController;
    private float attackCoolTime;

    // Start is called before the first frame update

    public void Start()
    {
        playerTransform = transform.parent.parent;
        playerController = playerTransform.GetComponent<PlayerController>();
        playerController.AddUpperState(CHARACTER_UPPER_STATE.TOP_ATTACKING, this);
    }

    public CHARACTER_UPPER_STATE GetState() { return CHARACTER_UPPER_STATE.TOP_ATTACKING; }

    public void StartState()
    {
        attackCoolTime = playerController.CurrentPlayer.Attack();
    }
    public void UpdateState()
    {
        attackCoolTime -= Time.deltaTime;
        if (attackCoolTime < 0)
        {
            playerController.ChangeUpperState(CHARACTER_UPPER_STATE.LOOKING_UP);
        }
    }
    public void EndState()
    {

    }
    public void Disable() { playerController.ChangeUpperState(CHARACTER_UPPER_STATE.DISABLED); }
    public void Aim(Vector3 aim)
    {
        if(aim != Vector3.zero && playerController.StandingGround != null)
            playerController.ChangeUpperState(CHARACTER_UPPER_STATE.AIMING);
    }
    public void Attack() { playerController.ChangeUpperState(CHARACTER_UPPER_STATE.TOP_ATTACKING); }
    public void LookUp(bool lookUp)
    { 
        if(!lookUp) 
        {
            playerController.CurrentPlayer.RotateUpperBody(0);

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
    public void Stop() { return; }
    public void Jump() { return; }
    public void OnAir() { return; }
    public void Move() { return; }
    public void OnGround() { return; }
    public void Enable() {return;}
}
