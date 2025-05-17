
using UnityEngine.Assertions;


public enum WeaponType : byte
{
    // Player weapon
    Bullet, NormalGranade,

    // Monster weapon
    Scratch,

    // Enum count
    WeaponTypeCount
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

public enum EffectType : byte
{
    NormalExplosion,

    EffectTypeCount
}

public static class Layer
{
    public const string Ground = "Ground";
    public const string Character = "Character";
    public const string Weapon = "Weapon";
}
