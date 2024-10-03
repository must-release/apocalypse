using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using WeaponEnums;

public abstract class WeaponBase : MonoBehaviour
{

    public bool DamagePlayer {get; protected set;}
    public WEAPON_TYPE WeaponType {get; protected set;}

    private Coroutine visibilityCheckCoroutine;

    public virtual void Fire(Vector2 direction)
    {
        gameObject.SetActive(true);
    }

    private void Update() { WeaponUpdate();}
    protected virtual void WeaponUpdate()
    {
        CheckVisibility();
    }

    private void CheckVisibility()
    {
        Vector3 viewportPos = Camera.main.WorldToViewportPoint(transform.position);

        if (viewportPos.x > 0 && viewportPos.x < 1 && viewportPos.y > 0 && viewportPos.y < 1)
        {
            if(visibilityCheckCoroutine!=null) StopCoroutine(visibilityCheckCoroutine);
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

    private void OnCollisionEnter2D(Collision2D other) 
    {
        if(other.transform.TryGetComponent(out CharacterBase character))
        {
            if ((character.CompareTag("player") && DamagePlayer) || !DamagePlayer)
            {
                character.OnDamaged();
            }
        }
    }
}
