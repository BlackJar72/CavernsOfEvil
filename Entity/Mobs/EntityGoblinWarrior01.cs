using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


namespace CevarnsOfEvil {

    public class EntityGoblinWarrior01 : EntityNavMeshUser
    {
        [SerializeField] protected Collider hitbox;
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
            bool output = base.TakeDamage(ref damage);
            if (!fleeing && (targetEntity != null)
                && ((targetEntity == damage.attacker) 
                && wandering
                && (Random.Range(0, health.Shock) < ((damage.shock + damage.wound) * 2))))
            {
                fleeing = true;
            }
            return output;
        }


        public override void Die(Damages damages)
        {
            entitySounds.PlayDeath(voice, 0);
            hitbox.gameObject.SetActive(false);
            base.Die(damages);
            // FIXME: This is a very hacky way to keep goblins from falling through the floor.
            //        Its fine currently, as they never leave the ground, but that limitation is 
            //        itself undesireable.  If they were given the ability to jump up a step this
            //        system would break.
            //
            //        I wonder if changing from a capsule collider + rigidbody to a character controller 
            //        would fix this in the process...?
            GetComponent<EntityDeath>().ForceImmediate();
        }


        public override void TriggerHit(Collider other)
        {
            if (!isDead && (nextAttack < Time.time) && (other.gameObject == targetObject))
            {
                nextAttack = Time.time + attackTime;
                anim.SetInteger("Variant", Random.Range(0, 3));
                anim.SetTrigger("Attack");
                SetFactorSpeed(0);
                entitySounds.PlayAttack(voice, 0);
                navMeshAgent.isStopped = true;
                useNavmesh = false;

                EntityHealth victim = other.gameObject.GetComponent<EntityHealth>();
                if (victim != null)
                {
                    victim.BeHitByAttack(meleeDamage, this);
                }
            }
        }


        public override void TriggerLeft(Collider other)
        {
            if (!isDead && (other.gameObject == targetObject) && navMeshAgent.isOnNavMesh)
            {
                navMeshAgent.isStopped = false;
                useNavmesh = true;
                SetFactorSpeed(prefferedSpeedFactor);
            }
        }


    }

}