using UnityEngine;

public class Scratch : ShortRangeWeaponBase
{
    public override WeaponType  WeaponType      => WeaponType.Scratch;
    public override bool        CanDamagePlayer => true;
    public override float       ActiveDuration  => 0.5f;
    public override float       PostDelay       => 0.8f;


    private Rigidbody2D rb;

    protected override void Awake() 
    {
        base.Awake();

        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        WeaponDamageInfo.damageValue = 1;
    }
}