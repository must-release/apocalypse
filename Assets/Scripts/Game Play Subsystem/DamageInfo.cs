using UnityEngine;

public class DamageInfo
{
    public DamageInfo(GameObject gameObject = null, int damageValue = 1, bool isSingleHit = false)
    {   
        attacker = gameObject;
        this.damageValue = damageValue;
        this.isSingleHit = isSingleHit;
    }

    public GameObject   attacker;
    public int          damageValue;
    public bool         isSingleHit;
}