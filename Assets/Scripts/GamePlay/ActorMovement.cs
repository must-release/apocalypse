using UnityEngine;

namespace AD.GamePlay
{
    [RequireComponent(typeof(Rigidbody2D))]
    public abstract class ActorMovement : MonoBehaviour
    {
        /****** Public Members ******/

        public Vector3 CurrentPosition => transform.position;
        public Vector2 CurrentVelocity => _rigidbody.linearVelocity;
        public FacingDirection CurrentFacingDirection 
        { 
            get 
            { 
                if (0 < transform.localScale.x) return FacingDirection.Right;
                else return FacingDirection.Left; 
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

        protected Rigidbody2D _rigidbody;

        protected virtual void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            Debug.Assert(null != _rigidbody, "Rigidbody2D component not found.");
        }
    }
}