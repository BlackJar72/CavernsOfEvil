using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CevarnsOfEvil
{
    public class EntityGoblinWarrior01 : EntityMob
    {
        [SerializeField] Collider hitbox;
        [SerializeField] float prefferedSpeedFactor = 2.0f / 3.0f;

        public override void Start() 
        { 
            base.Start();
        }


        public override Collider GetCollider()
        {
            return hitbox;
        }


        public override void GetAimParams(out AimParams aim)
        {
            throw new System.NotImplementedException();
        }


        public override bool TakeDamage(ref Damages damage)
        {
            entitySounds.PlayHurt(voice, 0);
            return base.TakeDamage(ref damage);
        }


        public override void Die(Damages damages)
        {
            entitySounds.PlayDeath(voice, 0);
            base.Die(damages);
        }


        public override void TriggerHit(Collider other)
        {
            if (!isDead && (nextAttack < Time.time) && (other.gameObject == targetObject))
            {
                nextAttack = Time.time + attackTime;
                anim.SetInteger("Variant", Random.Range(0, 3));
                anim.SetTrigger("Attack");
                entitySounds.PlayAttack(voice, 0); 
                AIVelocity = Vector3.zero;

                EntityHealth victim = other.gameObject.GetComponent<EntityHealth>();
                if (victim != null)
                {
                    victim.BeHitByAttack(meleeDamage, this);
                }
            }
        }


        public override void TriggerLeft(Collider other)
        {
            if (!isDead && (other.gameObject == targetObject))
            {
            }
        }


    }

}