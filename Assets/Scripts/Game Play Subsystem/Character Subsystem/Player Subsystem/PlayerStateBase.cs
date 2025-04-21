using UnityEngine;
using CharacterEums;

public abstract class PlayerLowerStateBase : MonoBehaviour
{
    /****** Public Memebers ******/

    public abstract PlayerLowerState GetStateType();
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

    public virtual void Damaged() 
    {
        PlayerLowerState nextState = PlayerLowerState.PlayerLowerStateCount;
        
        if ( 0 < OwnerController.HitPoint )
            nextState = PlayerLowerState.Damaged;
        else
            nextState = PlayerLowerState.Dead;

        OwnerController.ChangeLowerState(nextState);
    }


    /****** Protected Members ******/

    protected Transform         OwnerTransform     => _ownerTransform;
    protected PlayerController  OwnerController    => _ownerController;
    protected Rigidbody2D       OwnerRigid         => _ownerRigid;

    protected virtual void Start()
    {
        _ownerTransform     = transform.parent.parent;
        _ownerController    = _ownerTransform.GetComponent<PlayerController>();
        _ownerRigid         = _ownerTransform.GetComponent<Rigidbody2D>();
    }


    /****** Private Members ******/

    private Transform           _ownerTransform    = null;
    private PlayerController    _ownerController   = null;
    private Rigidbody2D         _ownerRigid        = null;
}


public abstract class PlayerUpperStateBase : MonoBehaviour
{
    protected Transform playerTransform;
    protected PlayerController playerController;

    /*** Abstract Funtions ***/
    public abstract PlayerUpperState GetStateType();
    public abstract void OnEnter();
    public abstract void OnUpdate();
    public abstract void OnExit(PlayerUpperState nextState);
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
        playerController.ChangeUpperState(PlayerUpperState.Disabled); 
    }

    public virtual void Aim(Vector3 aim) 
    {  
        if ( Vector3.zero == aim || null == playerController.StandingGround )
            return;
        
        playerController.ChangeUpperState(PlayerUpperState.Aiming);
    }


    /*** private Function ***/
    private void Start()
    {
        playerTransform = transform.parent.parent;
        playerController = playerTransform.GetComponent<PlayerController>();

        StartUpperState();
    }
}