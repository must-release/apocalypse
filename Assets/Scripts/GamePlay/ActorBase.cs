using UnityEngine;

namespace AD.GamePlay
{
    public class ActorBase : MonoBehaviour
    {
        public Transform ActorTransform => transform;
        public string ActorName => gameObject.name;
    }
}