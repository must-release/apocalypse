using UnityEngine;

public enum PlayerType { Hero, Heroine, PlayerCount }
public enum HeroineLowerState { Idle, Running, Jumping, Aiming, Climbing, Pushing, Tagging, Damaged, Dead, HeroLowerStateCount }
public enum HeroineUpperState { Disabled, Idle, Running, LookingUp, Aiming, Attacking, TopAttacking, HeroUpperStateCount }
public enum HeroLowerState { Idle, Running, Jumping, Aiming, Climbing, Pushing, Tagging, Damaged, Dead, HeroineLowerStateCount }
public enum HeroUpperState { Disabled, Idle, Running, Jumping, LookingUp, Aiming, Attacking, TopAttacking, AimAttacking, HeroineUpperStateCount }
public enum EnemyState { Patrolling, Chasing, Attacking, Damaged, Dead, EnemyStateCount }
public enum FacingDirection { Left, Right, FacingDirectionCount }

public static class AnimatorState
{
    public static class HeroLower
    {
        public static readonly int Idle = Animator.StringToHash("Hero_Lower_Idle");
    }

    public static class HeroUpper
    {
        public static readonly int Idle = Animator.StringToHash("Hero_Upper_Idle");
    }

    public static class HeroineLower
    {
        public static readonly int Idle         = Animator.StringToHash("Heroine_Lower_Idle");
        public static readonly int Running      = Animator.StringToHash("Heroine_Lower_Running");
        public static readonly int Jumping      = Animator.StringToHash("Heroine_Lower_Jumping");
        public static readonly int Aiming       = Animator.StringToHash("Heroine_Lower_Aiming");
        public static readonly int Climbing     = Animator.StringToHash("Heroine_Lower_Climbing");
        public static readonly int Pushing      = Animator.StringToHash("Heroine_Lower_Pushing");
        public static readonly int Tagging      = Animator.StringToHash("Heroine_Lower_Tagging");
        public static readonly int Damaged      = Animator.StringToHash("Heroine_Lower_Damaged");
        public static readonly int Dead         = Animator.StringToHash("Heroine_Lower_Dead");
    }

    public static class HeroineUpper
    {
        public static readonly int Idle         = Animator.StringToHash("Heroine_Upper_Idle");
        public static readonly int Running      = Animator.StringToHash("Heroine_Upper_Running");
        public static readonly int Aiming       = Animator.StringToHash("Heroine_Upper_Aiming");
        public static readonly int Attacking    = Animator.StringToHash("Heroine_Upper_Attacking");
        public static readonly int Disabled     = Animator.StringToHash("Heroine_Upper_Disabled");
        public static readonly int LookingUp    = Animator.StringToHash("Heroine_Upper_LookingUp");
        public static readonly int TopAttacking = Animator.StringToHash("Heroine_Upper_TopAttacking"); 
    }
}

public static class AnimationClipAsset
{
    private const string HeroPrefix     = "AnimationClip/Hero/";
    private const string HeroinePrefix  = "AnimationClip/Heroine/";



    public static class HeroLower
    {

    }

    public static class HeroUpper
    {

    }

    public static class HeroineLower
    {
        public const string Idle        = HeroinePrefix + "Lower/Idle";
        public const string Running     = HeroinePrefix + "Lower/Running";
        public const string Jumping     = HeroinePrefix + "Lower/Jumping";
        public const string Aiming      = HeroinePrefix + "Lower/Aiming";
        public const string Climbing    = HeroinePrefix + "Lower/Climbing";
        public const string Pushing     = HeroinePrefix + "Lower/Pushing";
        public const string Tagging     = HeroinePrefix + "Lower/Tagging";
        public const string Damaged     = HeroinePrefix + "Lower/Damaged";
        public const string Dead        = HeroinePrefix + "Lower/Dead";

    }

    public static class HeroineUpper
    {
        public const string Idle            = HeroinePrefix + "Upper/Idle";
        public const string Running         = HeroinePrefix + "Upper/Running";
        public const string Aiming          = HeroinePrefix + "Upper/Aiming";
        public const string Attacking       = HeroinePrefix + "Upper/Attacking";
        public const string Disabled        = HeroinePrefix + "Upper/Disabled";
        public const string LookingUp       = HeroinePrefix + "Upper/LookingUp";
        public const string TopAttacking    = HeroinePrefix + "Upper/TopAttacking";
    }
}