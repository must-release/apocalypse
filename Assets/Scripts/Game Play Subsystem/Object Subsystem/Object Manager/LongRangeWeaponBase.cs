using System.Collections;
using UnityEngine;
using WeaponEnums;

public abstract class LongRangeWeaponBase : WeaponBase
{
    public float FireSpeed {get; protected set;}

    private Coroutine visibilityCheckCoroutine;

    public override void Attack(Vector3 direction)
    {
        gameObject.SetActive(true);
        transform.localRotation = Quaternion.FromToRotation(Vector3.right, direction);
    }

    protected override void WeaponUpdate()
    {
        CheckVisibility();
    }

    private void CheckVisibility()
    {
        Vector3 viewportPos = Camera.main.WorldToViewportPoint(transform.position);

        if ( 0 < viewportPos.x && viewportPos.x < 1 && 0 < viewportPos.y  && viewportPos.y < 1)
        {
            if( null != visibilityCheckCoroutine ) 
                StopCoroutine(visibilityCheckCoroutine);
        }
        else
        {
            visibilityCheckCoroutine = StartCoroutine(AsyncInactivateWeapon());
        }
    }
    IEnumerator AsyncInactivateWeapon()
    {
        yield return new WaitForSeconds(5f);

        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.transform.TryGetComponent(out CharacterBase character))
        {
            if (character.CompareTag("Player") == DamagePlayer)
            {
                character.OnDamaged(WeaponDamageInfo);
            }
        }

        if( false == other.isTrigger )
        {
            gameObject.SetActive(false);
        }
    }
}
