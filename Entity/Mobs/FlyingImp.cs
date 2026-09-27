using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CevarnsOfEvil
{

    [RequireComponent(typeof(Rigidbody))]
    public class FlyingImp : PhysicalMob, IWorldSensorUser
    {
        [SerializeField] WorldSensor sensor;

        private bool movementDecided;


        public override void Attack()
        {
            nextAttack = Time.time + attackTime;
            if ((nextFireTime < Time.time) && (DistanceSqrToPlayer() > meleeStopDistance))
            {
                RangedAttack();
            } else if(DistanceSqrToPlayer() < meleeStopDistance) {
                MeleeAttack();
            }
        }


        public override void MeleeAttack()
        {
            ToTargetDir();
            base.MeleeAttack();
            anim.SetInteger("AnimID", 2);
            entitySounds.PlayAttack(voice, 1);
            nextFireTime = Mathf.Max(nextFireTime, nextAttack);
        }


        public void RangedAttack()
        {
            if ((targetEntity != null) && (targetEntity.enabled))
            {   
                ToTargetDir();
                NextFireTime = nextAttack + FireDelay + Random.value;
                AimParams aim;
                GetAimParams(out aim);
                GameObject proj = Instantiate(projectile, aim.from, ProjSpawn.rotation);
                proj.GetComponent<SimpleProjectile>().LaunchSimple(aim.toward, this);
            }
            anim.SetInteger("AnimID", 1);
            entitySounds.PlayAttack(voice, 0);
        }


        public void OnWorldSensorTriggered(Collider other)
        {
            //Debug.Log("Sensor collided with " + other.gameObject.name);
        }


        public void OnWorldSensorExit(Collider other)
        {
            //Debug.Log("Sensor left " + other.gameObject.name);
        }


    }


}
