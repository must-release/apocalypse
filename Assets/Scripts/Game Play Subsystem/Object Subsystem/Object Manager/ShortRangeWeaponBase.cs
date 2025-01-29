using System.Collections;
using UnityEngine;
using WeaponEnums;

public abstract class ShortRangeWeaponBase : WeaponBase
{
    public override void Attack(Vector3 direction)
    {
        gameObject.SetActive(true);
    }

    protected override void WeaponUpdate()
    {
    }

    private void OnTriggerStay2D(Collider2D other) 
    {
        if(other.transform.TryGetComponent(out CharacterBase character))
        {
            if (character.CompareTag("Player") == DamagePlayer)
            {
                character.OnDamaged(WeaponDamageInfo);
            }
        }
    }
}
