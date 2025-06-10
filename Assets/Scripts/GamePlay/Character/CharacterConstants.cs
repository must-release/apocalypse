using UnityEngine;

public enum PlayerType { Hero, Heroine, PlayerCount }
public enum HeroineLowerState { Idle, Running, Jumping, Aiming, Attacking, AimAttacking, Climbing, Pushing, Tagging, Damaged, Dead, HeroLowerStateCount }
public enum HeroineUpperState { Disabled, Idle, Running, LookingUp, Attacking, TopAttacking, HeroUpperStateCount }
public enum HeroLowerState { Idle, Running, Jumping, Aiming, AimAttacking, Climbing, Pushing, Tagging, Damaged, Dead, HeroineLowerStateCount }
public enum HeroUpperState { Disabled, Idle, Running, Jumping, LookingUp, Aiming, Attacking, TopAttacking, AimAttacking, HeroineUpperStateCount }
public enum EnemyState { Patrolling, Chasing, Attacking, Damaged, Dead, EnemyStateCount }
public enum FacingDirection { Left, Right, FacingDirectionCount }

public static class AnimatorState
{
    public static class Hero
    {
        public static int GetHash(HeroLowerState state, string variation = "") => Animator.StringToHash($"Hero_Lower_{state}{variation}");
        public static int GetHash(HeroUpperState state, string variation = "") => Animator.StringToHash($"Hero_Upper_{state}{variation}");
    }

    public static class Heroine
    {
        public static int GetHash(HeroineLowerState state, string variation = "") => Animator.StringToHash($"Heroine_Lower_{state}{variation}");
        public static int GetHash(HeroineUpperState state, string variation = "") => Animator.StringToHash($"Heroine_Upper_{state}{variation}");
    }
}