using UnityEngine;

public class DamageInfo 
{
    public DamageInfo(GameObject attacker = null, int damageValue = 1, bool isContinuousHit = false)
    {   
        Attacker        = attacker;
        DamageValue     = damageValue;
        IsContinuousHit = isContinuousHit;
    }

    public GameObject   Attacker        { get; set; }
    public int          DamageValue     { get; set; }
    public bool         IsContinuousHit { get; set; }
}