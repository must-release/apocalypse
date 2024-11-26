using UnityEngine;
using CharacterEums;

public class AttackingUpperState : MonoBehaviour, IPlayerUpperState
{
    private Transform playerTransform;
    private PlayerController playerController;
    private float attackingTime;

    public void Start()
    {
        playerTransform = transform.parent.parent;
        playerController = playerTransform.GetComponent<PlayerController>();
        playerController.AddUpperState(PLAYER_UPPER_STATE.ATTACKING, this);
    }

    public PLAYER_UPPER_STATE GetState() { return PLAYER_UPPER_STATE.ATTACKING; }

    public void StartState()
    {
        // Execute attack and get attacking motion time
        attackingTime = playerController.CurrentPlayer.Attack();
    }

    public void UpdateState()
    {
        // Wait for attacking animation
        attackingTime -= Time.deltaTime;
        if (attackingTime < 0)
        {
            if (playerController.StandingGround == null)
                playerController.ChangeUpperState(PLAYER_UPPER_STATE.JUMPING);
            else
                playerController.ChangeUpperState(PLAYER_UPPER_STATE.IDLE);
        }
    }

    public void EndState(PLAYER_UPPER_STATE _)
    {
        
    }

    public void Disable() { playerController.ChangeUpperState(PLAYER_UPPER_STATE.DISABLED); }

    public void LookUp(bool lookUp) { if(lookUp) playerController.ChangeUpperState(PLAYER_UPPER_STATE.LOOKING_UP);}
    
    public void Aim(Vector3 aim)
    {
        if(aim != Vector3.zero && playerController.StandingGround != null)
            playerController.ChangeUpperState(PLAYER_UPPER_STATE.AIMING);
    }
    
    public void Attack() { playerController.ChangeUpperState(PLAYER_UPPER_STATE.ATTACKING); }


    /***** Inavailable State Change *****/
    public void Stop() { return; }
    public void Jump() { return; }
    public void OnAir() { return; }
    public void Move() { return; }
    public void OnGround() { return; }
    public void Enable() {return;}
}
