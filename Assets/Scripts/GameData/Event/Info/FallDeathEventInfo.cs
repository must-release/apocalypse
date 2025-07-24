using UnityEngine;
using UnityEngine.Assertions;
using System;

[Serializable]
[CreateAssetMenu(fileName = "NewFallDeathEventInfo", menuName = "EventInfo/FallDeathEvent", order = 0)]
public class FallDeathEventInfo : GameEventInfo
{
    /****** Public Members ******/

    public int HPDamage { get { return _hpDamage; } private set { _hpDamage = value; } }

    public void Initialize(int hpDamage = 10)
    {
        Assert.IsTrue(hpDamage > 0, $"HP damage must be positive: {hpDamage}");

        _hpDamage = hpDamage;
        IsInitialized = true;
        IsRuntimeInstance = true;
    }

    public override GameEventInfo Clone()
    {
        var clone = Instantiate(this);
        clone.IsRuntimeInstance = true;

        return clone;
    }

    /****** Protected Members ******/

    protected override void OnEnable()
    {
        EventType = GameEventType.FallDeath;
    }

    protected override void OnValidate()
    {
        if (_hpDamage <= 0)
        {
            _hpDamage = 10; // Default value
        }
    }


    /****** Private Members ******/

    [SerializeField] private int _hpDamage = 10;
}