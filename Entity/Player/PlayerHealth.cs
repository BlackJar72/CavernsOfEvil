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
        public override float RelatvieShock { get { return (float)shock / (float)BaseShock; } }

        public override int Health { get { return health; } set { health = value; } }
        public override float Shock { get { return shock; } set { shock = value; } }
        public override int Armor { get { return armor; } set { armor = value; } }


        public static void Init()
        {
            health = BASE_HEALTH;
            shock = BASE_SHOCK;
        }


        #region Hits and Damages

        public override void BeHitByBullet(RaycastHit hit, Entity attacker)
        {
            BeHitByRaycastAttack(hit, 20, attacker);
        }


        public override void BeHitByRaycastAttack(RaycastHit hit, int damageBase, Entity attacker)
        {
            Damages damage;
            if (Armor < 1)
            {
                damage = DamageUtils.CalcDamageNoArmor(damageBase, attacker);
            }
            else
            {
                damage = DamageUtils.CalcDamage(damageBase, Armor, attacker);
            }
            if (owner.TakeDamage(ref damage))
            {
                Shock -= damage.shock;
                Health -= damage.wound;
                if ((Shock < 1) || (Health < 1))
                {

                    (owner as Player).Killer = damage;
                }
                else if (bloodParticles != null)
                {
                    GameObject blood = Instantiate(bloodParticles, hit.point,
                        Quaternion.FromToRotation(Vector3.forward, hit.normal));
                    blood.transform.parent = hit.collider.transform;
                }
                owner.ShowDamage();
            }
        }


        public override void BeHitByRaycastAttack(RaycastHit hit, int damageBase, DamageType type, Entity attacker)
        {
            Damages damage;
            if (Armor < 1)
            {
                damage = DamageUtils.CalcDamageNoArmor(damageBase, type, attacker);
            }
            else
            {
                damage = DamageUtils.CalcDamage(damageBase, Armor, type, attacker);
            }
            if (owner.TakeDamage(ref damage))
            {
                Shock -= damage.shock;
                Health -= damage.wound;
                if ((Shock < 1) || (Health < 1))
                {
                    (owner as Player).Killer = damage;
                }
                else if (bloodParticles != null)
                {
                    GameObject blood = Instantiate(bloodParticles, hit.point,
                        Quaternion.FromToRotation(Vector3.forward, hit.normal));
                    blood.transform.parent = hit.collider.transform;
                }
                owner.ShowDamage();
            }
        }


        public override void BeHitByAttack(int damageBase, DamageType type, Entity attacker)
        {
            Damages damage;
            if (Armor < 1)
            {
                damage = DamageUtils.CalcDamageNoArmor(damageBase, type, attacker);
            }
            else
            {
                damage = DamageUtils.CalcDamage(damageBase, Armor, type, attacker);
            }
            if (owner.TakeDamage(ref damage))
            {
                Shock -= damage.shock;
                Health -= damage.wound;
                if ((Shock < 1) || (Health < 1))
                {
                    (owner as Player).Killer = damage;
                }
                owner.ShowDamage();
            }
        }


        public override void BeHitByAttack(int damageBase, Entity attacker)
        {
            Damages damage;
            if (Armor < 1)
            {
                damage = DamageUtils.CalcDamageNoArmor(damageBase, attacker);
            }
            else
            {
                damage = DamageUtils.CalcDamage(damageBase, Armor, attacker);
            }
            if (owner.TakeDamage(ref damage))
            {
                Shock -= damage.shock;
                Health -= damage.wound;
                if ((Shock < 1) || (Health < 1))
                {
                    (owner as Player).Killer = damage;
                }
                owner.ShowDamage();
            }
        }


        public override void TakePoisonDamage(int amount)
        {
            Shock--;
            if (Shock < 1)
            {
                Damages damages = new Damages();
                damages.type = DamageType.poison;
                (owner as Player).Killer = damages;
                owner.ShowDamage();
            }
        }

        #endregion


    }

}
