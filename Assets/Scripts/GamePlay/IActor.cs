using UnityEngine;

namespace AD.GamePlay
{
    public interface IActor
    {
        string ActorName { get; }
        Transform ActorTransform { get; }
    }
}