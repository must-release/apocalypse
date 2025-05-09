using System.Collections;
using UnityEngine;

public abstract class LongRangeWeaponBase : WeaponBase
{
    /****** Public Members ******/

    public abstract float FireSpeed { get; }

    public override void Attack(Vector3 direction)
    {
        gameObject.SetActive(true);
        transform.localRotation = Quaternion.FromToRotation(Vector3.right, direction);
    }


    /****** Protected Members ******/

    protected virtual void OnEnable()
    {
        if (null != _visibilityCheckCoroutine)
        {
            StopCoroutine(_visibilityCheckCoroutine);
        }
    }


    /****** Private Members ******/

    private const float _ActiveDuration = 10f;

    private Coroutine _visibilityCheckCoroutine = null;


    private void Update()
    {
        CheckVisibility();
    }

    private void CheckVisibility()
    {
        Vector3 viewportPos = Camera.main.WorldToViewportPoint(transform.position);

        if ( 0 < viewportPos.x && viewportPos.x < 1 && 0 < viewportPos.y  && viewportPos.y < 1)
        {
            if( null != _visibilityCheckCoroutine ) 
                StopCoroutine(_visibilityCheckCoroutine);
        }
        else
        {
            _visibilityCheckCoroutine = StartCoroutine(AsyncInactivateWeapon());
        }
    }

    private IEnumerator AsyncInactivateWeapon()
    {
        yield return new WaitForSeconds(_ActiveDuration);

        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.transform.TryGetComponent(out CharacterBase character))
        {
            if (character.CompareTag("Player") == CanDamagePlayer)
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
