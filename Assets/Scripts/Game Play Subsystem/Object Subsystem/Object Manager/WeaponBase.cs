using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using WeaponEnums;

public abstract class WeaponBase : MonoBehaviour
{

    public bool DamagePlayer {get; protected set;}
    public WEAPON_TYPE WeaponType {get; protected set;}
    public float FireSpeed {get; protected set;}
    public DamageInfo WeaponDamageInfo {get; protected set;}

    private Coroutine visibilityCheckCoroutine;

    public virtual void Fire(Vector3 direction)
    {
        gameObject.SetActive(true);
        transform.localRotation = Quaternion.FromToRotation(Vector3.right, direction);
    }

    private void Awake() { InitializeWeapon(); }
    protected virtual void InitializeWeapon()
    {
        gameObject.layer = LayerMask.NameToLayer("Weapon");
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

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.transform.TryGetComponent(out CharacterBase character))
        {
            if (character.CompareTag("Player") == DamagePlayer)
            {
                character.OnDamaged(WeaponDamageInfo);
            }
        }

        if(!other.isTrigger)
        {
            gameObject.SetActive(false);
        }
    }
}
