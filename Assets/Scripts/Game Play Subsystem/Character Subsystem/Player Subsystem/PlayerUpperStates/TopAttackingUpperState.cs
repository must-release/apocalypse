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
        playerController.AddUpperState(PLAYER_UPPER_STATE.TOP_ATTACKING, this);
    }

    public PLAYER_UPPER_STATE GetState() { return PLAYER_UPPER_STATE.TOP_ATTACKING; }

    public void StartState()
    {
        // Rotate upper body by 90 degree
        playerController.CurrentPlayer.RotateUpperBody(90);

        // Execute top attack and get attacking motion time
        attackCoolTime = playerController.CurrentPlayer.Attack();
    }

    public void UpdateState()
    {
        // Wait for attacking animation
        attackCoolTime -= Time.deltaTime;
        if (attackCoolTime < 0)
        {
            playerController.ChangeUpperState(PLAYER_UPPER_STATE.LOOKING_UP);
        }
    }

    public void EndState(PLAYER_UPPER_STATE nextState)
    {
        // Recover upper body rotation when not looking up
        if (nextState != PLAYER_UPPER_STATE.LOOKING_UP)
            playerController.CurrentPlayer.RotateUpperBody(0);
    }

    public void Disable() 
    { 
        playerController.ChangeUpperState(PLAYER_UPPER_STATE.DISABLED); 
    }
    
    public void Aim(Vector3 aim)
    {
        if(aim != Vector3.zero && playerController.StandingGround != null)
            playerController.ChangeUpperState(PLAYER_UPPER_STATE.AIMING);
    }

    public void Attack() { playerController.ChangeUpperState(PLAYER_UPPER_STATE.TOP_ATTACKING); }
    
    public void LookUp(bool lookUp)
    { 
        if(!lookUp)
        {
            if (playerController.StandingGround == null)
                playerController.ChangeUpperState(PLAYER_UPPER_STATE.JUMPING);
            else
                playerController.ChangeUpperState(PLAYER_UPPER_STATE.IDLE);
        }
    }


    /***** Inavailable State Change *****/
    public void Stop() { return; }
    public void Jump() { return; }
    public void OnAir() { return; }
    public void Move() { return; }
    public void OnGround() { return; }
    public void Enable() {return;}
}
