using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class EffectController : MonoBehaviour, IEffect
{
    /****** Public Members ******/

    public event Action OnEffectComplete;

    public bool IsPlaying
    {
        get
        {
            foreach (var ps in _particleSystems)
            {
                if (ps.isPlaying) return true;
            }
            return false;
        }
    }

    public float TotalDuration
    {
        get
        {
            float max = 0f;
            foreach (var ps in _particleSystems)
            {
                var main = ps.main;
                float duration = main.duration + main.startLifetime.constantMax;
                if (duration > max) max = duration;
            }
            return max;
        }
    }

    public void Play()
    {
        foreach (var ps in _particleSystems)
        {
            ps.Play();
        }

        if (_monitorCoroutine != null)
            StopCoroutine(_monitorCoroutine);

        _monitorCoroutine = StartCoroutine(MonitorParticleCompletion());
    }

    public void Stop()
    {
        foreach (var ps in _particleSystems)
        {
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }

    public void Reset()
    {
        Stop();
        Play();
    }

    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }

    public void OnGetFromPool()
    {
        gameObject.SetActive(true);
        Play();
    }

    public void OnReturnToPool()
    {
        Stop();
        gameObject.SetActive(false);
    }


    /****** Private Members ******/

    private List<ParticleSystem>    _particleSystems    = new List<ParticleSystem>();
    private Coroutine               _monitorCoroutine   = null;

    private void Awake()
    {
        ParticleSystem[] particleSystems = GetComponentsInChildren<ParticleSystem>();
        foreach (var ps in particleSystems)
        {
            if (false == _particleSystems.Contains(ps))
            {
                _particleSystems.Add(ps);
            }
        }
    }

    private IEnumerator MonitorParticleCompletion()
    {
        yield return new WaitForSeconds(TotalDuration);
        OnEffectComplete?.Invoke();
    }
}
