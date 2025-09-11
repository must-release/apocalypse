using System;
using System.Collections.Generic;
using UnityEngine;

namespace AD.GamePlay
{
    [RequireComponent(typeof(ControlInputBuffer))]
    public class PlayerController : CharacterBase, IAsyncLoadObject, IObjectInteractor
    {
        /****** Public Members ******/

        public IPlayerAvatar CurrentAvatar { get; private set; }
        public PlayerAvatarType CurrentPlayerType { get; private set; } = PlayerAvatarType.PlayerAvatarTypeCount;
        public IClimbable CurrentClimbableObject { get; set; }
        public Collider2D ClimberCollider { get; private set; }

        public event Action<int, int> OnHPChanged;

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

        public void InitializePlayer(PlayerAvatarType player)
        {
            // Initially change character
            CurrentPlayerType = player;
            CurrentAvatar = _avatarDictionary[CurrentPlayerType];
            var turningOffType = (CurrentPlayerType == PlayerAvatarType.Hero) ? PlayerAvatarType.Heroine : PlayerAvatarType.Hero;
            var turningOffAvatar = _avatarDictionary[turningOffType];

            CurrentAvatar.ActivateAvatar(true);
            turningOffAvatar.ActivateAvatar(false);

            _isInitilized = true;
        }

        public override void ControlCharacter(IReadOnlyControlInfo controlInfo)
        {
            Debug.Assert(null != controlInfo, "Control info is null");
            Debug.Assert(null != CurrentAvatar, "Current avatar is null");

            CurrentAvatar.ControlAvatar(controlInfo);

            if (controlInfo.IsTagging)
            {
                ChangePlayer();
            }
        }

        public override void ApplyDamage(DamageInfo damageInfo)
        {
            if (CurrentAvatar.IsDamageImmune)
                return;

            Stats.RecentDamagedInfo = damageInfo;
            Stats.CurrentHitPoint -= Stats.RecentDamagedInfo.DamageValue;

            if (Stats.CurrentHitPoint <= 0)
            {
                Stats.CurrentHitPoint = 0;
                OnHPChanged?.Invoke(Stats.CurrentHitPoint, Stats.MaxHitPoint);
                CurrentAvatar.OnDead();
                return;
            }

            OnHPChanged?.Invoke(Stats.CurrentHitPoint, Stats.MaxHitPoint);
            CurrentAvatar.OnDamaged(damageInfo);
        }

        public void ChangePlayer()
        {
            CurrentAvatar.ActivateAvatar(false);

            CurrentPlayerType = (CurrentPlayerType == PlayerAvatarType.Hero) ? PlayerAvatarType.Heroine : PlayerAvatarType.Hero;
            CurrentAvatar = _avatarDictionary[CurrentPlayerType];

            CurrentAvatar.ActivateAvatar(true);
        }


        /****** Protected Members ******/

        protected override void Awake()
        {
            Debug.Assert(null != _heroTransform, "Hero Transform is not assigned in the editor.");
            Debug.Assert(null != _heroineTransform, "Heroine Transform is not assigned in the editor.");

            base.Awake();

            _avatarDictionary = new Dictionary<PlayerAvatarType, IPlayerAvatar>();
            _playerRigid = GetComponent<Rigidbody2D>();
            _inputBuffer = GetComponent<ControlInputBuffer>();
            Stats.CharacterHeight = GetComponent<BoxCollider2D>().size.y * transform.localScale.y;
            ClimberCollider = GetComponent<Collider2D>();
            _playerRigid.gravityScale = Stats.Gravity;
        }

        protected override void Start()
        {
            base.Start();

            RegisterAvatar(PlayerAvatarType.Hero, _heroTransform);
            RegisterAvatar(PlayerAvatarType.Heroine, _heroineTransform);
        }

        protected override void OnAir()
        {
            CurrentAvatar.OnAir();
        }

        protected override void OnGround()
        {
            CurrentAvatar.OnGround();
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            if (false == _isInitilized)
                return;

            CurrentAvatar.OnFixedUpdate();
        }

        /****** Private Members ******/

        [SerializeField] private Transform _heroTransform;
        [SerializeField] private Transform _heroineTransform;

        private Dictionary<PlayerAvatarType, IPlayerAvatar> _avatarDictionary;

        private Rigidbody2D         _playerRigid;
        private ControlInputBuffer  _inputBuffer;

        private bool _isInitilized;

        private void Update()
        {
            if (false == _isInitilized) return;

            CurrentAvatar.OnUpdate();
        }

        private void RegisterAvatar(PlayerAvatarType type, Transform root)
        {
            Debug.Assert(type < PlayerAvatarType.PlayerAvatarTypeCount, $"{type} is not a valid player type");
            Debug.Assert(null != root, $"{type} avatar root is null");

            IPlayerAvatar avatar = root.GetComponent<IPlayerAvatar>();
            Debug.Assert(null != avatar, $"{type} avatar (IPlayerAvatar) not found in {root.name}");
            _avatarDictionary.Add(type, avatar);
            avatar.InitializeAvatar(this, Movement, Stats, _inputBuffer);
        }
    }


    public interface IPlayerAvatar : IAsyncLoadObject
    {
        bool IsDamageImmune { get; }

        void InitializeAvatar(IObjectInteractor objectInteractor, CharacterMovement playerMovement, CharacterStats playerStats, ControlInputBuffer inputBuffer);
        void ControlAvatar(IReadOnlyControlInfo controlInfo);
        void ActivateAvatar(bool value);
        void OnUpdate();
        void OnFixedUpdate();
        void OnAir();
        void OnGround();
        void OnDamaged(DamageInfo damageInfo);
        void OnDead();
    }
}