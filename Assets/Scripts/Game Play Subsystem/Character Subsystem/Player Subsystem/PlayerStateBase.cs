using UnityEngine;

public abstract class PlayerLowerStateBase<TEnum> : MonoBehaviour where TEnum : System.Enum
{
    /****** Public Members ******/

    public abstract TEnum GetStateType();
    public abstract bool ShouldDisableUpperBody();
    public abstract void OnEnter();
    public abstract void OnUpdate();
    public abstract void OnExit(CommonPlayerLowerState nextState);
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
        CommonPlayerLowerState nextState = CommonPlayerLowerState.PlayerLowerStateCount;
        
        if ( 0 < OwnerController.HitPoint )
            nextState = CommonPlayerLowerState.Damaged;
        else
            nextState = CommonPlayerLowerState.Dead;

        OwnerController.ChangeLowerState(nextState);
    }

    public virtual void SetOwner(PlayerController playerController)
    {
        _ownerController    = playerController;
        _ownerTransform     = _ownerController.transform;
        _ownerRigid         = _ownerController.GetComponent<Rigidbody2D>();
    }


    /****** Protected Members ******/

    protected Transform         OwnerTransform     => _ownerTransform;
    protected PlayerController  OwnerController    => _ownerController;
    protected Rigidbody2D       OwnerRigid         => _ownerRigid;


    /****** Private Members ******/

    private Transform           _ownerTransform    = null;
    private PlayerController    _ownerController   = null;
    private Rigidbody2D         _ownerRigid        = null;
}


public abstract class PlayerUpperStateBase<TEnum> : MonoBehaviour where TEnum : System.Enum
{
    /****** Public Members ******/

    public abstract TEnum  GetStateType();
    public abstract void OnEnter();
    public abstract void OnUpdate();
    public abstract void OnExit(CommonPlayerUpperState nextState);
    public abstract void Move();
    public abstract void OnAir();
    public abstract void LookUp(bool lookUp);
    public abstract void Attack();
    public abstract void Jump();
    public abstract void Stop();
    public abstract void OnGround();
    public abstract void Enable();

    public virtual void Disable() 
    { 
        OwnerController.ChangeUpperState(CommonPlayerUpperState.Disabled); 
    }

    public virtual void Aim(Vector3 aim) 
    {  
        if ( Vector3.zero == aim || null == OwnerController.StandingGround )
            return;
        
        OwnerController.ChangeUpperState(CommonPlayerUpperState.Aiming);
    }
    public virtual void SetOwner(PlayerController playerController)
    {
        _ownerController    = playerController;
        _ownerTransform     = _ownerController.transform;
    }


    /****** Protected Members ******/

    protected Transform OwnerTransform          => _ownerTransform;
    protected PlayerController OwnerController  => _ownerController;


    /****** Private Members ******/

    private Transform _ownerTransform           = null;
    private PlayerController _ownerController   = null;
}