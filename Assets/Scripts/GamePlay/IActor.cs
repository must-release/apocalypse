using UnityEngine;

namespace AD.GamePlay
{
    public interface IActor
    {
        Transform       ActorTransform { get; }
        ActorMovement   Movement { get; }
        
        string ActorName { get; }
    }
}