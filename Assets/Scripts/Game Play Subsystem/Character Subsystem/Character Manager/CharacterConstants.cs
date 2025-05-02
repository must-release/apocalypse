using UnityEngine;

public enum PlayerType { Hero, Heroine, PlayerCount }
public enum HeroLowerState { Idle, Running, Jumping, Aiming, Climbing, Pushing, Tagging, Damaged, Dead, HeroLowerStateCount }
public enum HeroUpperState { Disabled, Idle, Running, Jumping, LookingUp, Aiming, Attacking, TopAttacking, AimAttacking, HeroUpperStateCount }
public enum HeroineLowerState { Idle, Running, Jumping, Aiming, Climbing, Pushing, Tagging, Damaged, Dead, HeroineLowerStateCount }
public enum HeroineUpperState { Disabled, Idle, Running, Jumping, LookingUp, Aiming, Attacking, TopAttacking, AimAttacking, HeroineUpperStateCount }
public enum EnemyState { Patrolling, Chasing, Attacking, Damaged, Dead, EnemyStateCount }
public enum FacingDirection { Left, Right, FacingDirectionCount }

public static class AnimatorParams
{
    // Bool Parameters
    public static readonly int IsRunning    = Animator.StringToHash("IsRunning");
    public static readonly int IsOnGround   = Animator.StringToHash("IsOnGround");

    // Trigger Parameters
    public static readonly int JumpTrigger  = Animator.StringToHash("JumpTrigger");
}

public static class AnimationClipAsset
{
    private const string CommonPrefix   = "AnimationClip/";
    private const string HeroPrefix     = CommonPrefix + "Hero/";
    private const string HeroinePrefix  = CommonPrefix + "Heroine/";



    public static class HeroLower
    {

    }

    public static class HeroUpper
    {

    }

    public static class HeroineLower
    {
        public const string Idle        = HeroinePrefix + "LowerIdle";
        public const string Running     = HeroinePrefix + "LowerRunning";
        public const string Jumping     = HeroinePrefix + "LowerJumping";
        public const string Aiming      = HeroinePrefix + "LowerAiming";
        public const string Climbing    = HeroinePrefix + "LowerClimbing";
        public const string Pushing     = HeroinePrefix + "LowerPushing";
        public const string Tagging     = HeroinePrefix + "LowerTagging";
        public const string Damaged     = HeroinePrefix + "LowerDamaged";
        public const string Dead        = HeroinePrefix + "LowerDead";

    }

    public static class HeroineUpper
    {

    }
}