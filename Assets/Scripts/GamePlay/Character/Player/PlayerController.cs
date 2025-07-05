using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class PlayerController : CharacterBase, IAsyncLoadObject, IObjectInteractor
{
    /****** Public Members ******/

    public IPlayerAvatar        CurrentAvatar           { get; private set; }
    public PlayerType           CurrentPlayerType       { get; private set; } = PlayerType.PlayerCount;
    public IClimbable           CurrentClimbableObject  { get; set; }
    public Collider2D           ClimberCollider         { get; private set; }

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
        var turningOffType  = (CurrentPlayerType == PlayerType.Hero) ? PlayerType.Heroine : PlayerType.Hero;
        var turningOffAvatar = _avatarDictionary[turningOffType];

        CurrentAvatar.ActivateAvatar(true);
        turningOffAvatar.ActivateAvatar(false);

        _isInitilized = true;
    }

    public override void ControlCharacter(IReadOnlyControlInfo controlInfo)
    {
        Assert.IsTrue(null != controlInfo, "Control info is null");
        Assert.IsTrue(null != CurrentAvatar, "Current avatar is null");

        CurrentAvatar.ControlAvatar(controlInfo);

        if (controlInfo.IsTagging)
        {
            ChangePlayer();
        }
    }

    public override void OnDamaged(DamageInfo damageInfo) 
    {
        if (_isDamageImmune) return;

        RecentDamagedInfo = damageInfo;
        CurrentHitPoint -= RecentDamagedInfo.DamageValue;

        if (CurrentHitPoint <= 0)
        {
            CurrentHitPoint = 0;
            CurrentAvatar.OnDead();
            return;
        }

        StartCoroutine(StartDamageImmuneState());

        CurrentAvatar.OnDamaged(damageInfo);
    }

    public void ChangePlayer()
    {
        CurrentAvatar.ActivateAvatar(false);

        CurrentPlayerType   = (CurrentPlayerType == PlayerType.Hero) ? PlayerType.Heroine : PlayerType.Hero;
        CurrentAvatar       = _avatarDictionary[CurrentPlayerType];

        CurrentAvatar.ActivateAvatar(true);
    }


    /****** Protected Members ******/

    protected override void Awake()
    {
        Assert.IsTrue(null != _heroTransform, "Hero Transform is not assigned in the editor.");
        Assert.IsTrue(null != _heroineTransform, "Heroine Transform is not assigned in the editor.");

        base.Awake();
        
        _avatarDictionary   = new Dictionary<PlayerType, IPlayerAvatar>();
        _playerRigid        = GetComponent<Rigidbody2D>();
        CharacterHeight     = GetComponent<BoxCollider2D>().size.y * transform.localScale.y;
        ClimberCollider     = GetComponent<Collider2D>();
        MovingSpeed         = 8f;
        JumpingSpeed        = 14f;
        Gravity             = 4f;
        MaxHitPoint         = _MaxHitPoint;
        CurrentHitPoint     = MaxHitPoint;
        _playerRigid.gravityScale = Gravity;
    }

    protected override void Start()
    {
        base.Start();

        RegisterAvatar(PlayerType.Hero, _heroTransform);
        RegisterAvatar(PlayerType.Heroine, _heroineTransform);
    }

    protected override void OnAir()
    {
        CurrentAvatar.OnAir();
    }

    protected override void OnGround()
    {
        Debug.Log($"PlayerController.OnGround() called for {CurrentPlayerType}");

        CurrentAvatar.OnGround();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (false == _isInitilized) return;
        CurrentAvatar.OnFixedUpdate();
    }

    /****** Private Members ******/

    [SerializeField] private Transform _heroTransform;
    [SerializeField] private Transform _heroineTransform;

    private const int   _MaxHitPoint        = 3;
    private const float _DamageImmuneTime   = 2f;

    private Dictionary<PlayerType, IPlayerAvatar> _avatarDictionary;

    private Rigidbody2D _playerRigid;
    private bool _isInitilized;
    private bool _isDamageImmune;

    private void Update()
    {
        if (false == _isInitilized) return;

        CurrentAvatar.OnUpdate();
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
        avatar.InitializeAvatar(this, this, this);
    }
}


public interface IPlayerAvatar : IAsyncLoadObject
{
    void InitializeAvatar(IObjectInteractor objectInteractor, IMotionController playerMotion, ICharacterInfo playerInfo);
    void ControlAvatar(IReadOnlyControlInfo controlInfo);
    void ActivateAvatar(bool value);
    void OnUpdate();
    void OnFixedUpdate();
    void OnAir();
    void OnGround();
    void OnDamaged(DamageInfo damageInfo);
    void OnDead();
}