using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace DLD
{

    public class WingedImp : EntityRangedMob
    {
        [SerializeField] protected Collider hitbox;


        public override void Start()
        {
            base.Start();
            SetFactorSpeed(0);
        }


        public override Collider GetCollider()
        {
            return hitbox;
        }


        public override bool TakeDamage(ref Damages damage)
        {
            if (damage.type == DamageType.fire) return false;
            else if (Random.value < 0.2)
            {
                entitySounds.PlayHurt(voice, 0);
                anim.SetTrigger("Pain");
                nextAttack += 0.625f;
            }
            return base.TakeDamage(ref damage);
        }


        public override void Die(Damages damages)
        {
            entitySounds.PlayDeath(voice, 0);
            base.Die(damages);
        }


        public override void Attack()
        {
            if (nextFireTime < Time.time)
            {
                nextAttack = Time.time + attackTime;
                RangedAttack();
            }
        }


        public override void TriggerHit(Collider other)
        {
            if (!isDead && (nextAttack < Time.time) && (other.gameObject == targetObject))
            {
                nextAttack = Time.time + attackTime;
                entitySounds.PlayAttack(voice, 0);
                routingAgent.isStopped = true;

                MeleeAttack();
            }
        }


        public override void TriggerLeft(Collider other)
        {
            if (!isDead && (other.gameObject == targetObject))
            {
                routingAgent.isStopped = false;
            }
        }


        public override void MeleeAttack()
        {
            base.MeleeAttack();
            SetFactorSpeed(0);
            anim.SetInteger("AnimID", 2);
            entitySounds.PlayAttack(voice, 1);
            nextFireTime = Mathf.Max(nextFireTime, nextAttack);
        }


        public override void RangedAttack()
        {
            if ((targetEntity != null) && (targetEntity.enabled))
            {
                NextFireTime = nextAttack + FireDelay + Random.value;
                AimParams aim;
                GetAimParams(out aim);
                GameObject proj = Instantiate(projectile, aim.from, ProjSpawn.rotation);
                proj.GetComponent<SimpleProjectile>().LaunchSimple(aim.toward, this);
            }
            SetFactorSpeed(0);
            anim.SetInteger("AnimID", 1);
            entitySounds.PlayAttack(voice, 0);
        }


    }


}