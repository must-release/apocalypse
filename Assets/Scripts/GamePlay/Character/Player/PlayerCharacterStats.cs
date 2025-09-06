using UnityEngine;

namespace AD.GamePlay
{
    public class PlayerCharacterStats : CharacterStats
    {
        /****** Public Members ******/

        public float JumpingSpeed   { get; private set; }
        public float PushingSpeed   { get; private set; }
        public float Gravity        { get; private set; }


        /****** Private Members ******/

        public PlayerCharacterStats(PlayerCharacterData playerData) : base(playerData)
        {
            Debug.Assert(null != playerData, "PlayerCharacterData cannot be null");

            JumpingSpeed    = playerData.JumpingSpeed;
            PushingSpeed    = playerData.PushingSpeed;
            Gravity         = playerData.Gravity;
        }
    }
}