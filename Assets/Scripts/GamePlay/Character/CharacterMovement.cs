using UnityEngine;

namespace AD.GamePlay
{
    public class CharacterMovement : ActorMovement
    {
        /****** Public Members ******/

        public GameObject StandingGround { get; set; }
        public bool IsMoving => Mathf.Abs(CurrentVelocity.x) > 0.01f;
        public bool IsFalling => CurrentVelocity.y < -0.01f;
    }
}