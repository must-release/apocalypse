using UnityEngine;

public enum PlayerAvatarType { Hero, Heroine, PlayerAvatarTypeCount }

public enum EnemyStateType { Idle, Patrolling, Chasing, Attacking, Damaged, Dead, EnemyStateCount }

public abstract class StateType
{
    public string Name { get; private set; }
    public override string ToString() => Name;
    public override bool Equals(object obj)
    {
        if (obj is StateType stateType)
        {
            if (stateType.Name == Name)
            {
                return true;
            }
        }
        return false;
    }
    public override int GetHashCode()
    {
        return Name.GetHashCode();
    }

    protected StateType(string name) { Name = name; }
}

public class LowerStateType : StateType
{
    public static readonly LowerStateType Idle          = new LowerStateType("Idle");
    public static readonly LowerStateType Running       = new LowerStateType("Running");
    public static readonly LowerStateType Jumping       = new LowerStateType("Jumping");
    public static readonly LowerStateType Aiming        = new LowerStateType("Aiming");
    public static readonly LowerStateType Climbing      = new LowerStateType("Climbing");
    public static readonly LowerStateType Pushing       = new LowerStateType("Pushing");
    public static readonly LowerStateType Tagging       = new LowerStateType("Tagging");
    public static readonly LowerStateType Damaged       = new LowerStateType("Damaged");
    public static readonly LowerStateType Dead          = new LowerStateType("Dead");

    protected LowerStateType(string name) : base(name) {}
}

public class UpperStateType : StateType
{   
    public static readonly UpperStateType Disabled     = new UpperStateType("Disabled");
    public static readonly UpperStateType Idle         = new UpperStateType("Idle");
    public static readonly UpperStateType Running      = new UpperStateType("Running");
    public static readonly UpperStateType LookingUp    = new UpperStateType("LookingUp");

    protected UpperStateType(string name) : base(name) {}
}

public class HeroineLowerStateType : LowerStateType
{
    private HeroineLowerStateType(string name) : base(name) { }

    public static readonly HeroineLowerStateType Attacking = new HeroineLowerStateType("Attacking");
    public static readonly HeroineLowerStateType AimAttacking = new HeroineLowerStateType("AimAttacking");
}

public class HeroLowerStateType : LowerStateType
{
    private HeroLowerStateType(string name) : base(name) { }

    public static readonly HeroLowerStateType StandingAttack = new HeroLowerStateType("StandingAttack");
    public static readonly HeroLowerStateType IdleLookingUp = new HeroLowerStateType("IdleLookingUp");
    public static readonly HeroLowerStateType IdleTopAttacking = new HeroLowerStateType("IdleTopAttacking");
}

public class HeroUpperStateType : UpperStateType
{
    private HeroUpperStateType(string name) : base(name) { }

    public static readonly HeroUpperStateType Jumping           = new HeroUpperStateType("Jumping");
    public static readonly HeroUpperStateType Aiming            = new HeroUpperStateType("Aiming");
    public static readonly HeroUpperStateType Attacking         = new HeroUpperStateType("Attacking");
    public static readonly HeroUpperStateType RunningTopAttack  = new HeroUpperStateType("RunningTopAttack");
    public static readonly HeroUpperStateType AimAttacking      = new HeroUpperStateType("AimAttacking");
}

public static class AnimatorState
{
    public static int GetHash(PlayerAvatarType player, LowerStateType state, string variation = "") => Animator.StringToHash($"{player}_Lower_{state}{variation}");
    public static int GetHash(PlayerAvatarType player, UpperStateType state, string variation = "") => Animator.StringToHash($"{player}_Upper_{state}{variation}");
}