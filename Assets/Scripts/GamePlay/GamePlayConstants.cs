
using UnityEngine.Assertions;


public enum ProjectileType : byte
{
    // Player
    Bullet, NormalGranade,

    // Monster
    PigeonPoop,

    // Enum count
    ProjectileTypeCount
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
    public const string Projectile  = "Projectile";
    public const string Obstacle    = "Obstacle";
}
