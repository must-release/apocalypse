using System.Collections;
using System.Collections.Generic;
using CharacterEnums;
using UnityEngine;
using UnityEngine.Assertions;

public class PlayerController : CharacterBase, IAsyncLoadObject
{
    /****** Public Members ******/

    public IPlayerAvatar        CurrentAvatar       { get; private set; } = null;
    public PlayerType           CurrentPlayerType   { get; private set; } = PlayerType.PlayerCount;
    
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

    // Called once when player is on air
    public override void OnAir()
    {
        CurrentAvatar.OnAir();
    }

    // Called once when player is on ground
    public override void OnGround() 
    { 
        CurrentAvatar.OnGround();
    }

    public override void OnDamaged(DamageInfo damageInfo) 
    {
        if ( _isDamageImmune ) return;

        RecentDamagedInfo = damageInfo;
        CurrentHitPoint -= RecentDamagedInfo.damageValue;

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
        
        _avatarDictionary           = new Dictionary<PlayerType, IPlayerAvatar>();
        _playerRigid                = GetComponent<Rigidbody2D>();
    }

    protected override void Start()
    {
        base.Start();

        CharacterHeight             = GetComponent<CapsuleCollider2D>().size.y * transform.localScale.y;
        MovingSpeed                 = 15f;
        JumpingSpeed                = 30f;
        Gravity                     = 10f;
        MaxHitPoint                 = _MaxHitPoint;
        CurrentHitPoint             = MaxHitPoint;
        _playerRigid.gravityScale   = Gravity;
        _isInitilized               = false;
        _isDamageImmune             = false;
        _flickeringSpeed            = 0.5f; 
        _flickeringCount            = 3;
        _damagedColor               = Color.red;
        _originalColor              = Color.white;

        //RegisterAvatar(PlayerType.Hero, _heroTransform);
        RegisterAvatar(PlayerType.Heroine, _heroineTransform);
    }


    /****** Private Members ******/
    private const int _MaxHitPoint = 3;

    [SerializeField] private Transform _heroTransform;
    [SerializeField] private Transform _heroineTransform;

    private Dictionary<PlayerType, IPlayerAvatar>       _avatarDictionary; 
    
    private Rigidbody2D _playerRigid;
    private SpriteRenderer _lowerSpriteRenderer;
    private SpriteRenderer _upperSpriteRenderer;
    private Color _damagedColor;
    private Color _originalColor;
    private bool _isInitilized;
    private bool _isDamageImmune;
    private float _flickeringSpeed;
    private int _flickeringCount;

    private void Update()
    {
        if (false == _isInitilized) return;

        CurrentAvatar.OnUpdate();
    }

    private IEnumerator StartDamageImmuneState()
    {
        _isDamageImmune = true;

        // Change character's color
        for (int i = 0; i < _flickeringCount; i++)
        {
            yield return StartCoroutine(LerpColor(_originalColor, _damagedColor, _flickeringSpeed));
            yield return StartCoroutine(LerpColor(_damagedColor, _originalColor, _flickeringSpeed));
        }

        _isDamageImmune = false;
    }

    private IEnumerator LerpColor(Color startColor, Color targetColor, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            _lowerSpriteRenderer.material.color = Color.Lerp(startColor, targetColor, elapsedTime / duration);
            _upperSpriteRenderer.material.color = Color.Lerp(startColor, targetColor, elapsedTime / duration);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        _lowerSpriteRenderer.material.color = targetColor;
        _upperSpriteRenderer.material.color = targetColor;
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
    void OnAir();
    void OnGround();
    void OnDamaged(DamageInfo damageInfo);
}