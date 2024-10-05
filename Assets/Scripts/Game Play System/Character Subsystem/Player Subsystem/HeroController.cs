using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeaponEnums;

public class HeroController : MonoBehaviour, IPlayer
{
    public bool IsLoaded {get; set;}
    private Queue<WeaponBase> weapons;
    private int weaponCount;
    private Transform shootingPointPivot;
    private Transform shootingPoint;
    private float attackCoolTime;

    private void Awake() {
        weapons = new Queue<WeaponBase>();
        weaponCount = 20;
        IsLoaded = false;
        shootingPointPivot = transform.Find("Shooting Point Pivot");
        shootingPoint = shootingPointPivot.Find("Shooting Point").transform;
        attackCoolTime = 0.5f;
    }

    private void Start() { StartCoroutine(LoadWeapons()); }
    IEnumerator LoadWeapons()
    {
        yield return WeaponFactory.Instance.PoolWeapons(WEAPON_TYPE.BULLET, weapons, weaponCount);
        IsLoaded = true;
    }
    
    // Show or hide character object
    public void ShowCharacter(bool value)
    {
        gameObject.SetActive(value);
    }

    public void RotateUpperBody(float rotateAngle)
    {
        shootingPointPivot.localRotation = Quaternion.Euler(0, 0, rotateAngle);
    }

    public void RotateUpperBody(Vector3 target)
    {
        Vector3 direction = target - shootingPointPivot.position;
        int flip = direction.x > 0 ? 0 : 180;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        shootingPointPivot.rotation = Quaternion.Euler(0, 0, flip + angle);
    }

    public float Attack()
    {
        WeaponBase bullet = weapons.Dequeue();
        bullet.transform.position = shootingPoint.position;
        bullet.Fire(shootingPoint.position - shootingPointPivot.position);
        weapons.Enqueue(bullet);
        return attackCoolTime;
    }
}
