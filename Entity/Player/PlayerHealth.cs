using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CevarnsOfEvil {

    public class PlayerHealth : EntityHealth
    {
        public const int BASE_HEALTH = 100;
        public const int BASE_SHOCK = 100;
        public const int BASE_ARMOR = 0;

        private static volatile int armor;
        private static volatile int health;
        private static volatile float shock;

        public override int BaseHealth { get { return BASE_HEALTH; } }
        public override int BaseShock { get { return BASE_SHOCK; } }
        public override float RelativeHealth { get { return (float)health / (float)BASE_HEALTH; } }
        public override float RelatvieShock { get { return shock / (float)BaseShock; } }

        public override int Health { get { return health; } set { health = value; } }
        public override float Shock { get { return shock; } set { shock = value; } }
        public override int Armor { get { return armor; } set { armor = value; } }


        public static void Init()
        {
            health = BASE_HEALTH;
            shock = BASE_SHOCK;
        }
    }

}
