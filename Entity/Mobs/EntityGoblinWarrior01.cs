using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


namespace CevarnsOfEvil {

    public class EntityGoblinWarrior01 : EntityNavMeshUser
    {
        [SerializeField] Collider hitbox;
        [SerializeField] float prefferedSpeedFactor = 2.0f / 3.0f;

        private Rigidbody rigid;

        public override void Start() 
        { 
            base.Start();
            rigid = GetComponent<Rigidbody>(); 
        }


        public override void Update()
        {
            base.Update();
            StepDataAI stepAhead = gameManager.GetAIDataForGround(transform.position, 
                transform.position + (navMeshAgent.velocity.normalized * 0.5f), this);
            onGround = IsOnGround();
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