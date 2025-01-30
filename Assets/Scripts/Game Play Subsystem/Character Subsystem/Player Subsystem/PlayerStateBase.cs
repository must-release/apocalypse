using UnityEngine;
using CharacterEums;
using System.Collections;

public abstract class PlayerLowerStateBase : MonoBehaviour
{
    protected Transform playerTransform;
    protected PlayerController playerController;
    protected Rigidbody2D playerRigid;

    /*** Abstract Funtions ***/
    public abstract PLAYER_LOWER_STATE GetState();
    public abstract bool DisableUpperBody();
    public abstract void OnEnter();
    public abstract void OnUpdate();
    public abstract void OnExit();
    public abstract void Jump();
    public abstract void OnAir();
    public abstract void Aim(bool isAiming);
    public abstract void Move(int move);
    public abstract void Tag();
    public abstract void Climb(bool climb);
    public abstract void OnGround();
    public abstract void Stop();
    public abstract void Push(bool push);
    public abstract void UpDown(int upDown);
    protected abstract void StartLowerState();


    /*** Virtual Functions ***/
    public virtual void Damaged() 
    {
        PLAYER_LOWER_STATE nextState;
        
        if ( 0 < playerController.HitPoint )
            nextState = PLAYER_LOWER_STATE.DAMAGED;
        else
            nextState = PLAYER_LOWER_STATE.DEAD;

        playerController.ChangeLowerState(nextState);
    }


    /*** private Function ***/
    private void Start()
    {
        playerTransform = transform.parent.parent;
        playerController = playerTransform.GetComponent<PlayerController>();
        playerRigid = playerTransform.GetComponent<Rigidbody2D>();

        StartLowerState();
    }
}


public abstract class PlayerUpperStateBase : MonoBehaviour
{
    protected Transform playerTransform;
    protected PlayerController playerController;

    /*** Abstract Funtions ***/
    public abstract PLAYER_UPPER_STATE GetState();
    public abstract void OnEnter();
    public abstract void OnUpdate();
    public abstract void OnExit(PLAYER_UPPER_STATE nextState);
    public abstract void Move();
    public abstract void OnAir();
    public abstract void LookUp(bool lookUp);
    public abstract void Attack();
    public abstract void Jump();
    public abstract void Stop();
    public abstract void OnGround();
    public abstract void Enable();
    protected abstract void StartUpperState();


    /*** Virtual Functions ***/
    public virtual void Disable() 
    { 
        playerController.ChangeUpperState(PLAYER_UPPER_STATE.DISABLED); 
    }

    public virtual void Aim(Vector3 aim) 
    {  
        if ( Vector3.zero == aim || null == playerController.StandingGround )
            return;
        
        playerController.ChangeUpperState(PLAYER_UPPER_STATE.AIMING);
    }


    /*** private Function ***/
    private void Start()
    {
        playerTransform = transform.parent.parent;
        playerController = playerTransform.GetComponent<PlayerController>();

        StartUpperState();
    }
}