using UnityEngine;

public class DamageInfo
{
    public DamageInfo(GameObject gameObject = null, int damageValue = 1)
    {   
        attacker = gameObject;
        this.damageValue = damageValue;
    }

    public GameObject attacker;
    public int damageValue;
}