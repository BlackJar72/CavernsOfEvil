using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace DLD {

    public class MobHealth : EntityHealth
    {
        [SerializeField] private int baseHealth = 100;
        [SerializeField] private int baseShock = -1;
        [SerializeField] private int armor = 0;


        private volatile int health;
        private volatile float shock;

        public override int BaseHealth { get { return baseHealth; } }
        public override int BaseShock { get { return baseShock; } }  
        public override float RelativeHealth { get { return (float)health / (float)baseHealth; } }
        public override float RelatvieShock { get { return (float)shock / (float)BaseShock; } }

        public override int Health { get { return health; } set { health = value; } }
        public override float Shock { get { return shock; } set { shock = value; } }
        public override int Armor { get { return armor; }  set { armor = value; } }


        // Start is called before the first frame update
        void Awake()
        {
            if (baseShock < 1) baseShock = baseHealth;
            health = baseHealth;
            shock = baseShock;
        }
    }
}
