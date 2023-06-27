using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CevarnsOfEvil {

    public class TrollHealth : EntityHealth {

        [SerializeField] private int baseHealth = 100;
        [SerializeField] private int baseShock = -1;
        [SerializeField] private int armor = 0;

        public float maxHealth;

        public volatile float health;
        public volatile float shock;

        public override int BaseHealth { get { return baseHealth; } }
        public override int BaseShock { get { return baseShock; } }
        public override float RelativeHealth { get { return health / (float)baseHealth; } }
        public override float RelatvieShock { get { return shock / (float)BaseShock; } }

        public override int Health { get { return (int) health; } set { health = value; } }
        public override float Shock { get { return shock; } set { shock = value; } }
        public override int Armor { get { return armor; }  set { armor = value; } }


        // Start is called before the first frame update
        void Awake()
        {
            if (baseShock < 1) baseShock = baseHealth;
            health = maxHealth = baseHealth;
            shock = baseShock;
        }


        void Update() {
            if(!ShouldDie) {
                health = Mathf.Min(maxHealth, health + (((shock / BaseShock) * 3 * Time.deltaTime)));
                Regen(); // This is for shock
            } else {
                Owner.Die(new Damages());
            }
        }


        public void BeDamagedByFire(Damages damage) {
            maxHealth -= damage.wound;
        }
    }

}