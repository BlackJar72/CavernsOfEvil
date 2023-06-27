using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CevarnsOfEvil {

    public abstract class EntityHealth : MonoBehaviour
    {
        [SerializeField] Entity owner;
        [SerializeField] GameObject bloodParticles;

        public Entity Owner { get { return owner; } }

        public abstract int BaseHealth { get; }
        public abstract int BaseShock { get; }  
        public abstract float RelativeHealth { get; }
        public abstract float RelatvieShock { get; }

        public abstract int Health { get; set; }
        public abstract float Shock { get; set;  }
        public abstract int Armor { get; set; }

        public bool ShouldDie { get => ((Health < 1) || (Shock < 1)); }

        public int DamageToKill
        {
            get
            {
                return (int)(Health / (1.0f - Mathf.Min(Armor / 25f, 0.75f)));
            }
        }


        public void Regen()
        {
            Shock = Mathf.Clamp(Shock + Time.deltaTime, 0, BaseShock);
        }


        public bool PlayerRegen()
        {
            bool regen = (Shock < BaseShock);
            Shock = Mathf.Clamp(Shock + Time.deltaTime, 0, BaseShock);
            return regen;
        }


        #region Hits and Damages

        public void BeHitByBullet(RaycastHit hit, Entity attacker)
        {
            BeHitByRaycastAttack(hit, 20, attacker);
        }


        public void BeHitByRaycastAttack(RaycastHit hit, int damageBase, Entity attacker)
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
                    owner.Die(damage);
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


        public void BeHitByRaycastAttack(RaycastHit hit, int damageBase, DamageType type, Entity attacker)
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
                    owner.Die(damage);
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


        public void BeHitByAttack(int damageBase, DamageType type, Entity attacker)
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
                    owner.Die(damage);
                }
                owner.ShowDamage();
            }
        }


        public void BeHitByAttack(int damageBase, Entity attacker)
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
                    owner.Die(damage);
                }
                owner.ShowDamage();
            }
        }


        public void TakePoisonDamage(int amount)
        {
            Shock--;
            if (Shock < 1)
            {
                Damages damages = new Damages();
                damages.type = DamageType.poison;
                owner.Die(damages);
            }
        }


        public void BeHitByEnviroDamage(int damage, DamageType type) {
            owner.BeHitByEnviroDamage(damage, type);
        }

        #endregion


        #region Healing

        public void BeHealed(int amount, bool healShock)
        {
            Health = Mathf.Clamp(Health + amount, 1, BaseHealth);
            if(healShock)
            {
                Shock = Mathf.Max(Shock, BaseShock);
            }
        }


        public void HealWoulds(int amount)
        {
            Health = Mathf.Clamp(Health + amount, 1, BaseHealth);
        }


        public void HealShock(int amount)
        {
            Shock = Mathf.Clamp(Shock + amount, 1, BaseShock);
        }


        public void HealFully()
        {
            Health = Mathf.Max(Health, BaseHealth);
            Shock = Mathf.Max(Shock, BaseShock);
        }

        #endregion

    }
}
