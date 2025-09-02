using UnityEngine;

namespace AD.GamePlay
{
    public class CharacterStats
    {
        /****** Public Members ******/

        public DamageInfo RecentDamagedInfo { get; set; }
        public float CharacterHeight { get; set; }
        public int CurrentHitPoint { get; set; }

        public float MovingSpeed { get; private set; }
        public float JumpingSpeed { get; private set; }
        public float Gravity { get; private set; }
        public int MaxHitPoint { get; private set; }


        public CharacterStats(CharacterData characterData)
        {
            Debug.Assert(null != characterData, "CharacterData cannot be null");

            MovingSpeed = characterData.MovingSpeed;
            JumpingSpeed = characterData.JumpingSpeed;
            Gravity = characterData.Gravity;
            MaxHitPoint = characterData.MaxHitPoint;
            CurrentHitPoint = MaxHitPoint;
            RecentDamagedInfo = new DamageInfo();
            CharacterHeight = 0f;
        }
    }
}