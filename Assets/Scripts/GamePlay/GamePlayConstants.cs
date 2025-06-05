
using UnityEngine.Assertions;


public enum WeaponType : byte
{
    // Player weapon
    Bullet, NormalGranade,

    // Monster weapon
    Scratch, PigeonPoop,

    // Enum count
    WeaponTypeCount
}

public enum EffectType : byte
{
    NormalExplosion,

    EffectTypeCount
}

public static class Layer
{
    public const string Ground      = "Ground";
    public const string Character   = "Character";
    public const string Weapon      = "Weapon";
    public const string Obstacle    = "Obstacle";
}
