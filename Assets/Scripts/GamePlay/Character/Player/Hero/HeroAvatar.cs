using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroAvatar : MonoBehaviour, IPlayerAvatar
{
    /****** Public Members ******/

    public bool IsLoaded => _isLoaded; 

    public void InitializeAvatar(IMotionController motionController, ICharacterInfo characterInfo)
    {
        // TODO: Add initialization logic here
    }

    public void ControlAvatar(ControlInfo controlInfo)
    {
        // TODO: Add control logic here
    }

    public void OnUpdate()
    {
        // TODO: Add update logic here
    }

    public void OnAir()
    {
        // TODO: Add logic for when the avatar is in the air
    }

    public void OnGround()
    {
        // TODO: Add logic for when the avatar is on the ground
    }

    public void OnDamaged(DamageInfo damageInfo)
    {
        // TODO: Add logic for when the avatar is damaged
    }

    public void OnDead()
    {

    }
    
    // Show or hide character object
    public void ActivateAvatar(bool value)
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

    public void Aim(bool value)
    {
        if(value)
        {
            Vector3 direction = (shootingPoint.position - shootingPointPivot.position).normalized;
            for (int i=0; i<aimingDotsCount; i++ )
            {
                aimingDots[i].gameObject.SetActive(true);
                aimingDots[i].transform.position = shootingPoint.position;
                aimingDots[i].transform.rotation = Quaternion.FromToRotation(Vector3.right, direction);
                aimingDots[i].transform.Translate(direction * i * 0.4f, Space.World);
            }
        }
        else
        {
            aimingDots.ForEach((dot)=>dot.gameObject.SetActive(false));
        }
    }

    public float Attack()
    {
        WeaponBase bullet = weapons.Dequeue();
        bullet.transform.position = shootingPoint.position;
        bullet.Attack((shootingPoint.position - shootingPointPivot.position).normalized);
        weapons.Enqueue(bullet);
        return attackCoolTime;
    }

    /****** Private Members ******/

    private PlayerController playerController;
    private Queue<WeaponBase> weapons;
    private List<GameObject> aimingDots;
    private int aimingDotsCount;
    private Transform shootingPointPivot;
    private Transform shootingPoint;
    private float attackCoolTime;
    private bool _isLoaded = false;

    private void Awake() {
        playerController = transform.parent.GetComponent<PlayerController>();
        weapons = new Queue<WeaponBase>();
        aimingDots = new List<GameObject>();
        aimingDotsCount = 40;
        shootingPointPivot = transform.Find("Shooting Point Pivot");
        shootingPoint = shootingPointPivot.Find("Shooting Point").transform;
        attackCoolTime = 0.5f;
    }

    private void Update() { }
}
