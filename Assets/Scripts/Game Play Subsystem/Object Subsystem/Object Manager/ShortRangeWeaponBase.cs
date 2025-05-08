using System.Collections;
using UnityEngine;

public abstract class ShortRangeWeaponBase : WeaponBase
{
    /****** Public Members ******/

    public override void Attack(Vector3 direction)
    {
        gameObject.SetActive(true);

        _attackTime = 0;
        _isAttacking = true;
    }


    /****** Protected Members ******/

    protected virtual void Update()
    {
        CountActiveDuration();
    }


    /****** Private Members ******/

    private float _attackTime = 0f;
    private bool _isAttacking = false;

    private void OnTriggerStay2D(Collider2D other) 
    {
        if(other.transform.TryGetComponent(out CharacterBase character))
        {
            if (character.CompareTag("Player") == CanDamagePlayer)
            {
                character.OnDamaged(WeaponDamageInfo);
            }
        }
    }

    private void CountActiveDuration()
    {
        if (false == _isAttacking)
            return;

        _attackTime += Time.deltaTime;
        if (ActiveDuration < _attackTime)
        {
            _isAttacking = false;
            gameObject.SetActive(false);
        }
    }
}
