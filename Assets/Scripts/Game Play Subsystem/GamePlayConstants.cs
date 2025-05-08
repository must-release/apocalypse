
using UnityEngine.Assertions;

public enum WeaponType
{
    // Player weapon
    Bullet, NormalGranade,

    // Monster weapon
    Scratch,

    // Enum count
    WEAPON_TYPE_COUNT

}

public static class WeaponAsset
{
    public static string GetWeaponPath(WeaponType weaponType)
    {
        return "Weapon/" + weaponType.ToString();
    }

    public static string GetAimingDotPath(WeaponType weaponType)
    {
        Assert.IsTrue(WeaponType.Bullet == weaponType || WeaponType.NormalGranade == weaponType, "Only player's weapon have aiming dots " + weaponType.ToString());

        return "AimingDot/" + weaponType.ToString();
    }
}
