using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;

namespace AD.GamePlay
{
    [RequireComponent(typeof(BoxCollider2D), typeof(Rigidbody2D))]
    public abstract class CharacterBase : ActorBase, ICharacter
    {
        /****** Public Members ******/
        public CharacterStats Stats { get; private set; }
        public new CharacterMovement Movement { get; private set; }

        public event Action OnCharacterDeath;
        public event Action OnCharacterDamaged;

        public virtual void ApplyDamage(DamageInfo damageInfo)
        {
            Debug.Assert(null != OnCharacterDeath, $"OnCharacterDeath event is not assigned in {ActorName}.");
            Debug.Assert(null != OnCharacterDamaged, $"OnCharacterDamaged event is not assigned in {ActorName}.");

            Stats.RecentDamagedInfo.Attacker = damageInfo.Attacker;
            Stats.CurrentHitPoint -= damageInfo.DamageValue;

            if (Stats.CurrentHitPoint <= 0)
            {
                OnCharacterDeath.Invoke();
            }
            else
            {
                OnCharacterDamaged.Invoke();
            }
        }

        public abstract bool IsPlayer { get; }
        public abstract void ControlCharacter(IReadOnlyControlInfo controlInfo);


        /****** Protected Members ******/

        protected override void OnValidate()
        {
            base.OnValidate();

            Debug.Assert(null != _characterData, $"CharacterData must be set in {ActorName}.");
            Debug.Assert(null != GetComponent<CharacterMovement>(), $"{ActorName} does not have CharacterMovement component.");
        }

        protected override void Awake()
        {
            base.Awake();

            _bodyCollider   = GetComponent<BoxCollider2D>();
            Movement        = GetComponent<CharacterMovement>();
            Stats           = CreateStats(_characterData);

            gameObject.layer = LayerMask.NameToLayer(Layer.Character);
        }

        protected virtual void Start()
        {
            CreateGroundSensor();
        }

        protected virtual void FixedUpdate()
        {
            GroundCheck();
        }

        protected virtual CharacterStats CreateStats(CharacterData data)
        {
            return new CharacterStats(data);
        }

        protected abstract void OnAir();
        protected abstract void OnGround();


        /****** Private Members ******/

        [SerializeField] private CharacterData _characterData;

        private BoxCollider2D _bodyCollider;
        private Transform _groundCheckPoint;
        private Vector2 _groundCheckSize;
        private bool _wasGrounded;

        private void CreateGroundSensor()
        {
            Debug.Assert(0 != Stats.CharacterHeight, $"CharacterHeight of {gameObject.name} is not set.");

            _groundCheckPoint = new GameObject("GroundCheckPoint").transform;
            _groundCheckPoint.SetParent(transform, false);
            _groundCheckPoint.localPosition = Vector3.zero;
            _groundCheckPoint.Translate(Vector3.down * Stats.CharacterHeight / 2f, Space.Self);

            _groundCheckSize = new Vector2(_bodyCollider.size.x * 0.9f, 0.1f);
        }

        private void GroundCheck()
        {
            Collider2D hit = Physics2D.OverlapBox(_groundCheckPoint.position, _groundCheckSize, 0, LayerMask.GetMask(Layer.Ground));

            bool isGrounded = hit != null;

            if (false == _wasGrounded && isGrounded)
            {
                OnGround();
                Movement.StandingGround = hit.gameObject;
            }
            else if (_wasGrounded && false == isGrounded)
            {
                Movement.StandingGround = null;
                if (gameObject.activeInHierarchy)
                    StartCoroutine(OnAirDelay());
            }

            _wasGrounded = isGrounded;
        }

        private IEnumerator OnAirDelay()
        {
            yield return new WaitForSeconds(0.1f);
            if (Movement.StandingGround == null) OnAir();
        }
    }
}