using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CevarnsOfEvil
{

    public class ChaoticImp : ChaoticRanged
    {
        [SerializeField] protected Collider hitbox;


        protected float nextChangeIdle;

        public Vector3 DesiredDirection { get { return desiredDirection; } set { desiredDirection = value; } }
        public float NextChangeIdle { get { return nextChangeIdle; } set { nextChangeIdle = value; } }


        public override void Start()
        {
            nextChangeIdle = Time.time;
            animSpeed = 0.75f;
            base.Start();
        }


        public override Collider GetCollider()
        {
            return hitbox;
        }


        public void RefreshIdle()
        {
            anim.SetFloat("LookState", Random.Range(0.0f, 2.0f));
            nextChangeIdle = Time.time + Random.Range(0.5f, 4.0f);
        }


        public override bool TakeDamage(ref Damages damage)
        {
            wasHit = true;
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
            ccphysics.Die();
            ccphysics.Falling = true;            
        }


        public override void Attack()
        {
            if (nextFireTime < Time.time)
            {
                nextAttack = Time.time + attackTime;
                RangedAttack();
            }
        }


        public void OnCollisionEnter(Collision collision)
        {
            if(collision.gameObject.layer == GameConstants.levelLayer)
            {
                shouldTurn = true;
            }
        }


        public override void TriggerHit(Collider other)
        {
            if (!isDead && (nextAttack < Time.time) && (other.gameObject == targetObject))
            {
                nextAttack = Time.time + attackTime;
                entitySounds.PlayAttack(voice, 0);
                ccphysics.AIMove = Vector3.zero; 
                MeleeAttack();
            }
        }


        public override void TriggerLeft(Collider other) {/*DO NOTHING*/}


        public override void MeleeAttack()
        {
            base.MeleeAttack();
            anim.SetInteger("AnimID", 1);
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
            anim.SetInteger("AnimID", 2);
            entitySounds.PlayAttack(voice, 0);
        }


        public override void FaceHeading()
        {
            // If target is in view, keep facing target to avoid target going out of sight...
            if(CanSeeTarget()) transform.LookAt(targetEntity.transform.position, Vector3.up);
            // ...otherwise face the direction of movement.
            else transform.LookAt(transform.position + direction);
        }


    }


}