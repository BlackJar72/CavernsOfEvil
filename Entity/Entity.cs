using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CevarnsOfEvil {

    public abstract class Entity : MonoBehaviour
    {
        [SerializeField] string entityName;
        [SerializeField] string translationKey;
        [SerializeField] protected EntityHealth health;
                 
        protected bool isDead = false;
        protected GameManager gameManager;

        public bool IsDead { get { return isDead; } }
        public EntityHealth Health { get { return health; } }
        public string EntityName { get { return entityName; } }
        public string LocalKey => translationKey;


        public virtual bool TakeDamage(ref Damages damage)
        {
            return !isDead;
        }


        public void SetManager(GameManager gameManager)
        {
            this.gameManager = gameManager;
        }


        public virtual void ShowDamage() { }


        public abstract void GetAimParams(out AimParams aim);


        public virtual Transform GetTransform() 
        {
            return transform;
        }


        public virtual Collider GetCollider()
        {
            return GetComponent<Collider>();
        }


        public virtual void TakeHealingPoition(int healAmount)
        {
            health.BeHealed(healAmount, true);
        }


        public virtual void Die(Damages damages) 
        {
            if (!isDead)
            {
                EntityDeath death = GetComponent<EntityDeath>();
                if (death != null)
                {
                    isDead = true;
                    death.enabled = true;
                    health.enabled = false;
                    enabled = false;
                }
                else Destroy(gameObject);
            }
        }


        public bool CheckLineOfSight(Collider collider, Collider alerter)
        {
            return !Physics.Linecast(alerter.bounds.center, collider.bounds.center, GameConstants.LevelMask);
        }


        public abstract void BeHitByEnviroDamage(int damage, DamageType type);


        #region Special Signals

        public virtual void TriggerHit(Collider other) { }
        public virtual void TriggerLeft(Collider other) { }


        #endregion


    }

}