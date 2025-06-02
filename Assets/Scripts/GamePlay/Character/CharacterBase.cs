using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

public abstract class CharacterBase : MonoBehaviour, ICharacter, IMotionController, ICharacterInfo
{
    /****** Public Members ******/

    public FacingDirection  CurrentFacingDirection 
    { 
        get 
        { 
            if (0 < transform.localScale.x) return FacingDirection.Right;
            else return FacingDirection.Left; 
        } 
    }
    public ControlInfo      CurrentControlInfo  { get; protected set; }
    public GameObject       StandingGround      { get; protected set; }
    public DamageInfo       RecentDamagedInfo   { get; protected set; }
    public Vector2          CurrentVelocity     
    { 
        get 
        { 
            Assert.IsTrue(null != _rigidbody, "Rigidbody2D is not assigned.");
            return _rigidbody.linearVelocity; 
        } 
    }
    public Vector2          CurrentPosition     { get { return transform.position; } }

    public virtual float    MovingSpeed         { get; protected set; }
    public virtual float    JumpingSpeed        { get; protected set; }
    public virtual float    Gravity             { get; protected set; }
    public virtual int      MaxHitPoint         { get; protected set; }
    public int              CurrentHitPoint     { get; protected set; }
    public float            CharacterHeight     { get; protected set; }

    public abstract bool IsPlayer { get; }
    public abstract void ControlCharacter(ControlInfo controlInfo);
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
        Assert.IsTrue(null != _rigidbody, "Rigidbody2D is not assigned.");

        _rigidbody.linearVelocity = velocity;
    }

    public void AddForce(Vector2 force, ForceMode2D mode = ForceMode2D.Force)
    {
        Assert.IsTrue(null != _rigidbody, "Rigidbody2D is not assigned.");

        _rigidbody.AddForce(force, mode);
    }

    public void ResetVelocity()
    {
        Assert.IsTrue(null != _rigidbody, "Rigidbody2D is not assigned.");

        _rigidbody.linearVelocity = Vector2.zero;
    }

    public void SetAngularVelocity(float angularVelocity)
    {
        Assert.IsTrue(null != _rigidbody, "Rigidbody2D is not assigned.");

        _rigidbody.angularVelocity = angularVelocity;
    }

    public void SetGravityScale(float scale)
    {
        Assert.IsTrue(null != _rigidbody, "Rigidbody2D is not assigned.");

        _rigidbody.gravityScale = scale;
    }

    public void TeleportTo(Vector2 position)
    {
        Assert.IsTrue(null != _rigidbody, "Rigidbody2D is not assigned.");

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
        _rigidbody = GetComponent<Rigidbody2D>();

        gameObject.layer = LayerMask.NameToLayer(Layer.Character); 
    }

    protected virtual void Start() { }

    protected abstract void OnAir();
    protected abstract void OnGround();

    protected void ControlInteractionObjects(ControlInfo controlInfo)
    {
        // Control interacting objects
        for (int i = _interactingObjects.Count - 1; i >= 0; i--)
        {
            if(!_interactingObjects[i].CheckInteractingControl(this, controlInfo))
            {
                _interactingObjects[i].EndInteraction(this, controlInfo);
                _interactableObjects.Add(_interactingObjects[i]);
                _interactingObjects.RemoveAt(i);
            }
        }

        // Control interactable objects
        for (int i = _interactableObjects.Count - 1; i >= 0; i--)
        {
            if(_interactableObjects[i].CheckInteractableControl(this, controlInfo))
            {
                _interactableObjects[i].StartInteraction(this, controlInfo);
                _interactingObjects.Add(_interactableObjects[i]);
                _interactableObjects.RemoveAt(i);
            }
        }
    }


    /****** Private Members ******/

    private List<InteractionObject> _interactableObjects   = new List<InteractionObject>();
    private List<InteractionObject> _interactingObjects    = new List<InteractionObject>();
    private Rigidbody2D _rigidbody;

    private void OnCollisionExit2D(Collision2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            if(collision.gameObject == StandingGround)
            {
                StandingGround = null;
                if(gameObject.activeSelf)
                    StartCoroutine(OnAirDelay());
            }
        }
    }
   
    private IEnumerator OnAirDelay()
    {
        yield return new WaitForSeconds(0.1f);
        if(StandingGround == null) OnAir();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if ((collision.gameObject.layer == LayerMask.NameToLayer("Ground")) && StandingOnGround(collision))
        {
            OnGround();
            StandingGround = collision.gameObject;
        }
    } 

    // Check angle between character and ground
    private bool StandingOnGround(Collision2D collision)
    {
        Vector2 normal = collision.contacts[0].normal;
        float angle = Vector2.Angle(normal, Vector2.up);
        if(angle > 45) return false;
        else return true;
    }
}