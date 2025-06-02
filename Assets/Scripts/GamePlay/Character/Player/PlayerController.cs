using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class PlayerController : CharacterBase, IAsyncLoadObject
{
    /****** Public Members ******/

    public IPlayerAvatar        CurrentAvatar       { get; private set; } = null;
    public PlayerType           CurrentPlayerType   { get; private set; } = PlayerType.PlayerCount;

    public override bool IsPlayer => true;
    public bool IsLoaded
    {
        get
        {
            foreach (var avatar in _avatarDictionary)
            {
                if (false == avatar.Value.IsLoaded) return false;
            }

            return true;
        }
    }

    public void InitializePlayer(PlayerType player)
    {
        // Initially change character
        CurrentPlayerType   = player;
        CurrentAvatar       = _avatarDictionary[CurrentPlayerType];

        CurrentAvatar.ActivateAvatar(true);

        _isInitilized = true;
    }

    // Control player according to the control info
    public override void ControlCharacter(ControlInfo controlInfo)
    {
        Assert.IsTrue(null != controlInfo, "Control info is null");
        Assert.IsTrue(null != CurrentAvatar, "Current avatar is null");

        CurrentControlInfo = controlInfo;

        ControlInteractionObjects(controlInfo);
        CurrentAvatar.ControlAvatar(controlInfo);
    }

    public override void OnDamaged(DamageInfo damageInfo) 
    {
        if ( _isDamageImmune ) return;

        RecentDamagedInfo = damageInfo;
        CurrentHitPoint -= RecentDamagedInfo.damageValue;

        if (CurrentHitPoint <= 0)
        {
            CurrentHitPoint = 0;
            CurrentAvatar.OnDead();
            return;
        }

        StartCoroutine(StartDamageImmuneState());

        CurrentAvatar.OnDamaged(damageInfo);
    }

    // Change player character
    public void ChangePlayer(PlayerType player)
    {
        CurrentAvatar.ActivateAvatar(false);

        CurrentPlayerType   = player;
        CurrentAvatar       = _avatarDictionary[CurrentPlayerType];

        CurrentAvatar.ActivateAvatar(true);
    }


    /****** Protected Members ******/

    protected override void Awake()
    {
        Assert.IsTrue(null != _heroTransform, "Hero Transform is not assigned in the editor.");
        Assert.IsTrue(null != _heroineTransform, "Heroine Transform is not assigned in the editor.");

        base.Awake();
        
        _avatarDictionary = new Dictionary<PlayerType, IPlayerAvatar>();
        _playerRigid      = GetComponent<Rigidbody2D>();
    }

    protected override void Start()
    {
        base.Start();

        CharacterHeight             = GetComponent<CapsuleCollider2D>().size.y * transform.localScale.y;
        MovingSpeed                 = 8f;
        JumpingSpeed                = 14f;
        Gravity                     = 4f;
        MaxHitPoint                 = _MaxHitPoint;
        CurrentHitPoint             = MaxHitPoint;
        _playerRigid.gravityScale   = Gravity;

        //RegisterAvatar(PlayerType.Hero, _heroTransform);
        RegisterAvatar(PlayerType.Heroine, _heroineTransform);
    }

    protected override void OnAir()
    {
        CurrentAvatar.OnAir();
    }

    protected override void OnGround()
    {
        CurrentAvatar.OnGround();
    }


    /****** Private Members ******/

    [SerializeField] private Transform _heroTransform;
    [SerializeField] private Transform _heroineTransform;

    private const int   _MaxHitPoint        = 3;
    private const float _DamageImmuneTime   = 1f;

    private Dictionary<PlayerType, IPlayerAvatar> _avatarDictionary;

    private Rigidbody2D _playerRigid;
    private bool _isInitilized;
    private bool _isDamageImmune;

    private void Update()
    {
        if (false == _isInitilized) return;

        CurrentAvatar.OnUpdate();
    }

    private void FixedUpdate()
    {
        if (false == _isInitilized) return;

        CurrentAvatar.OnFixedUpdate();
    }

    private IEnumerator StartDamageImmuneState()
    {
        _isDamageImmune = true;

        yield return new WaitForSeconds(_DamageImmuneTime);

        _isDamageImmune = false;
    }

    private void RegisterAvatar(PlayerType type, Transform root)
    {
        Assert.IsTrue(type < PlayerType.PlayerCount, $"{type} is not a valid player type");
        Assert.IsTrue(null != root, $"{type} avatar root is null");

        IPlayerAvatar avatar = root.GetComponent<IPlayerAvatar>();
        Assert.IsTrue(null != avatar, $"{type} avatar (IPlayerAvatar) not found in {root.name}");
        _avatarDictionary.Add(type, avatar);
        avatar.InitializeAvatar(this, this);
    }
}


public interface IPlayerAvatar : IAsyncLoadObject
{
    void InitializeAvatar(IMotionController playerPhysics, ICharacterInfo playerInfo);
    void ControlAvatar(ControlInfo controlInfo);
    void ActivateAvatar(bool value);
    void OnUpdate();
    void OnFixedUpdate();
    void OnAir();
    void OnGround();
    void OnDamaged(DamageInfo damageInfo);
    void OnDead();
}