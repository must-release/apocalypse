using System.Collections;
using UnityEngine;

namespace AD.GamePlay
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public abstract class GranadeBase : ProjectileBase
    {
        /****** Public Members ******/

        public abstract EffectType GranadeEffect { get; }

        public override bool  CanDamagePlayer   => false;
        public override float FireSpeed         => 15f;
        public override float GravityScale      => _GravityScale;
        public override float FireDuration      => 1f;
        public override float PostFireDelay     => 0.5f;

        public override void Fire(Vector3 direction)
        {
            base.Fire(direction);

            _granadeRigid.linearVelocity    = direction * FireSpeed;
            _granadeRigid.angularVelocity   = _AngleSpeed;
        }

        public override void SetOwner(GameObject owner)
        {
            base.SetOwner(owner);

            _ownerCollider = owner.GetComponent<Collider2D>();
            Physics2D.IgnoreCollision(_ownerCollider, _granadeCollider);
        }

        public override void OnGetFromPool()
        {
            base.OnGetFromPool();

            _granadeRigid.angularDamping = _InitialAngularDrag;
        }

        /****** Protected Members ******/

        protected override void Awake()
        {
            base.Awake();

            _granadeRigid       = GetComponent<Rigidbody2D>();
            _granadeCollider    = GetComponent<Collider2D>();

            _granadeRigid.gravityScale          = _GravityScale;
            ProjectileDamageInfo.DamageValue    = _Damage;
        }

        protected virtual void Explode()
        {
            if (null != _countDownCoroutine)
            {
                StopCoroutine(_countDownCoroutine);
                _countDownCoroutine = null;
            }

            ShowExplosionEffect();
            ActiveDamageArea();
            ExpireProjectile();
        }


        /****** Private Members ******/

        private const float _GravityScale           = 2f;
        private const float _AngleSpeed             = 240;
        private const float _InitialAngularDrag     = 0.05f;
        private const float _AngularDragAfterHit    = 3f;
        private const float _CountDownTime          = 0.5f;
        private const int   _Damage                 = 1;
        private const float _DamageRadius           = 3f;

        private Rigidbody2D _granadeRigid;
        private Collider2D  _granadeCollider;
        private Collider2D  _ownerCollider;
        private Coroutine   _countDownCoroutine;

        private IEnumerator StartCountDown()
        {
            yield return new WaitForSeconds(_CountDownTime);

            Explode();
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            _granadeRigid.angularDamping = _AngularDragAfterHit;

            if (collision.transform.TryGetComponent(out CharacterBase character))
            {
                if (false == character.CompareTag("Player"))
                {
                    Explode();
                    return;
                }
            }

            if (null == _countDownCoroutine && gameObject.activeInHierarchy)
                    _countDownCoroutine = StartCoroutine(StartCountDown());
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (LayerMask.NameToLayer(Layer.Character) == collision.gameObject.layer)
            {
                if (false == collision.CompareTag("Player"))
                {
                    Explode();
                    return;
                }
            }

            if (LayerMask.NameToLayer(Layer.Projectile) == collision.gameObject.layer)
            {
                Explode();
                return;
            }
        }

        private void ShowExplosionEffect()
        {
            IEffect explosionEffect = PoolManager.Instance.Get<EffectType, IEffect>(EffectType.NormalExplosion);
            explosionEffect.SetPosition(transform.position);
            explosionEffect.Play();
            explosionEffect.OnEffectComplete += () =>
            {
                PoolManager.Instance.Return(EffectType.NormalExplosion, explosionEffect);
            };
        }

        private void ActiveDamageArea()
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, _DamageRadius, 1 << LayerMask.NameToLayer(Layer.Character));
            foreach (var hit in hits)
            {
                var enemy = hit.GetComponent<ICharacter>();
                if (false == enemy?.IsPlayer)
                {
                    enemy.ApplyDamage(ProjectileDamageInfo);
                }
            }
        }
    }
}

