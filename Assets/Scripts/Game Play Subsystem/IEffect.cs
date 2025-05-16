
using System;
using UnityEngine;

public interface IEffect : IPoolable
{
    void Play();
    void Stop();
    void Reset();
    void SetPosition(Vector3 position);

    event Action OnEffectComplete;
}
