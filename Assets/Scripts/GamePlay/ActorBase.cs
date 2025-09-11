using UnityEngine;

namespace AD.GamePlay
{

    public class ActorBase : MonoBehaviour, IActor
    {
        /****** Public Members ******/

        public Transform ActorTransform => transform;
        public ActorMovement Movement => _movement;
        public string ActorName => gameObject.name;


        /****** Protected Members ******/

        protected virtual void OnValidate()
        {
            Debug.Assert(null != GetComponent<ActorMovement>(), $"{ActorName} does not have ActorMovement component.");
        }

        protected virtual void Awake()
        {
            _movement = GetComponent<ActorMovement>();
        }


        /****** Private Members ******/

        private ActorMovement _movement;
    }
}