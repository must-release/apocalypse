using UnityEngine;

public class HeroineWeapon : PlayerWeaponBase
{
    /****** Public Members ******/

    public override float       ReloadTime          => 0.5f;
    public override WeaponType  PlayerWeaponType    => WeaponType.NormalGranade;

    public override void Aim(Vector3 direction)
    {
        // Implement aiming logic here
        Debug.Log($"Aiming in direction: {direction}");
    }
}
