using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace AD.GamePlay
{
    [RequireComponent(typeof(BoxCollider2D), typeof(Rigidbody2D), typeof(CharacterMovement))]
    public abstract class CharacterBase : MonoBehaviour
    {
        /****** Public Members ******/
        public CharacterStats      Stats       { get; private set; }
        public CharacterMovement   Movement    { get; private set; }

        public string ActorName => gameObject.name;

        public void RecognizeInteractionObject(InteractionObject obj)
        {
            bool notInteractable = !_interactableObjects.Contains(obj);
            bool notInteracting = !_interactingObjects.Contains(obj);

            if(notInteractable && notInteracting)
            {
                _interactableObjects.Add(obj);
            }
            else
            {
                Debug.LogError("Detecting duplicate Object");
            }
        }
        
        public void ForgetInteractionObject(InteractionObject obj)
        {
            bool removedFromInteractable = _interactableObjects.Remove(obj);
            bool removedFromInteracting = _interactingObjects.Remove(obj);

            if (!removedFromInteractable && !removedFromInteracting)
            {
                Debug.LogError("Removing unknown Object");
            }
        }

        public abstract bool IsPlayer { get; }
        public abstract void ControlCharacter(IReadOnlyControlInfo controlInfo);
        public abstract void OnDamaged(DamageInfo damageInfo);


        /****** Protected Members ******/

        protected virtual void Awake() 
        { 
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

        private List<InteractionObject> _interactableObjects = new List<InteractionObject>();
        private List<InteractionObject> _interactingObjects    = new List<InteractionObject>();
        private BoxCollider2D _bodyCollider;
        private Transform _groundCheckPoint;
        private Vector2 _groundCheckSize;
        private bool _wasGrounded;

        private void OnValidate()
        {
            Debug.Assert(null != _characterData, $"CharacterData must be set in {ActorName}.");
        }

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
            if(Movement.StandingGround == null) OnAir();
        }
    }
}