
using UnityEngine.Assertions;

public enum HorizontalDirection { None = 0, Right = 1, Left = -1 }
public enum VerticalDirection   { None = 0, Up = 1, Down = -1 }
public enum FacingDirection     { Left, Right, FacingDirectionCount }
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
    public const string Ground = "Ground";
    public const string Character = "Character";
    public const string Projectile = "Projectile";
    public const string Obstacle = "Obstacle";
}
