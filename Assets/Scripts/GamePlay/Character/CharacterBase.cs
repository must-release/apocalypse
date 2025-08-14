using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using Unity.VisualScripting;

[RequireComponent(typeof(BoxCollider2D), typeof(Rigidbody2D))]
public abstract class CharacterBase : MonoBehaviour, ICharacter, IMotionController, ICharacterInfo
{
    /****** Public Members ******/

    public FacingDirection      CurrentFacingDirection 
    { 
        get 
        { 
            if (0 < transform.localScale.x) return FacingDirection.Right;
            else return FacingDirection.Left; 
        } 
    }
    public GameObject           StandingGround      { get; protected set; }
    public DamageInfo           RecentDamagedInfo   { get; protected set; }
    public Vector2              CurrentVelocity     
    { 
        get 
        { 
            Debug.Assert(null != _rigidbody, "Rigidbody2D is not assigned.");
            return _rigidbody.linearVelocity; 
        } 
    }
    public Vector3              CurrentPosition     { get { return transform.position; } }


    public virtual float MovingSpeed { get; protected set; }
    public virtual float    JumpingSpeed        { get; protected set; }
    public virtual bool     IsMoving            { get { return Mathf.Abs(CurrentVelocity.x) > 0.01f; } }
    public virtual float    Gravity             { get; protected set; }
    public virtual int      MaxHitPoint         { get; protected set; }
    public int              CurrentHitPoint     { get; protected set; }
    public float            CharacterHeight     { get; protected set; }
    public string           ActorName => gameObject.name;

    public abstract bool IsPlayer { get; }
    public abstract void ControlCharacter(IReadOnlyControlInfo controlInfo);
    public abstract void OnDamaged(DamageInfo damageInfo);

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

    public void SetVelocity(Vector2 velocity)
    {
        Debug.Assert(null != _rigidbody, "Rigidbody2D is not assigned.");

        _rigidbody.linearVelocity = velocity;
    }

    public void AddForce(Vector2 force, ForceMode2D mode = ForceMode2D.Force)
    {
        Debug.Assert(null != _rigidbody, "Rigidbody2D is not assigned.");

        _rigidbody.AddForce(force, mode);
    }

    public void ResetVelocity()
    {
        Debug.Assert(null != _rigidbody, "Rigidbody2D is not assigned.");

        _rigidbody.linearVelocity = Vector2.zero;
    }

    public void SetAngularVelocity(float angularVelocity)
    {
        Debug.Assert(null != _rigidbody, "Rigidbody2D is not assigned.");

        _rigidbody.angularVelocity = angularVelocity;
    }

    public void SetGravityScale(float scale)
    {
        Debug.Assert(null != _rigidbody, "Rigidbody2D is not assigned.");

        _rigidbody.gravityScale = scale;
    }

    public void TeleportTo(Vector2 position)
    {
        Debug.Assert(null != _rigidbody, "Rigidbody2D is not assigned.");

        _rigidbody.MovePosition(position);
    }

    public void SetFacingDirection(FacingDirection direction)
    {
        if (direction == FacingDirection.Right)
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        else if (direction == FacingDirection.Left)
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }


    /****** Protected Members ******/

    protected virtual void Awake() 
    { 
        _rigidbody  = GetComponent<Rigidbody2D>();
        _bodyCollider   = GetComponent<BoxCollider2D>();

        gameObject.layer = LayerMask.NameToLayer(Layer.Character); 

    }

    protected virtual void Start() 
    {
        CreateGroundSensor();
    }

    protected abstract void OnAir();
    protected abstract void OnGround();

    protected virtual void FixedUpdate()
    {
        GroundCheck();
    }

    /****** Private Members ******/

    private List<InteractionObject> _interactableObjects   = new List<InteractionObject>();
    private List<InteractionObject> _interactingObjects    = new List<InteractionObject>();
    private Rigidbody2D _rigidbody;
    private BoxCollider2D _bodyCollider;
    private Transform _groundCheckPoint;
    private Vector2 _groundCheckSize;
    private bool _wasGrounded;

    private void CreateGroundSensor()
    {
        Debug.Assert(0 != CharacterHeight, $"CharacterHeight of {gameObject.name} is not set.");

        _groundCheckPoint = new GameObject("GroundCheckPoint").transform;
        _groundCheckPoint.SetParent(transform, false);
        _groundCheckPoint.localPosition = Vector3.zero;
        _groundCheckPoint.Translate(Vector3.down * CharacterHeight / 2f, Space.Self);

        _groundCheckSize = new Vector2(_bodyCollider.size.x * 0.9f, 0.1f);
    }    

    private void GroundCheck()
    {
        Collider2D hit = Physics2D.OverlapBox(_groundCheckPoint.position, _groundCheckSize, 0, LayerMask.GetMask(Layer.Ground));

        bool isGrounded = hit != null;

        if (false == _wasGrounded && isGrounded)
        {
            OnGround();
            StandingGround = hit.gameObject;
        }
        else if (_wasGrounded && false == isGrounded)
        {
            StandingGround = null;
            if (gameObject.activeInHierarchy)
                StartCoroutine(OnAirDelay());
        }

        _wasGrounded = isGrounded;
    }
   
    private IEnumerator OnAirDelay()
    {
        yield return new WaitForSeconds(0.1f);
        if(StandingGround == null) OnAir();
    }
}