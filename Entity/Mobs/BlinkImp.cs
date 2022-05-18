using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CevarnsOfEvil
{

    public class BlinkImp : EntityRangedNavMeshUser
    {        
        [SerializeField] protected Collider hitbox;
        [SerializeField] protected GameObject telepuff;
        [SerializeField] protected GameObject deathExplosion;

        private bool shouldTeleport;


        public bool ShouldTeleport { get { return shouldTeleport; } set { shouldTeleport = value; }  }


        public override void Start()
        {
            shouldTeleport = false;
            base.Start();
        }
        

        public override void Update()
        {
            if(shouldTeleport) Teleport();
            base.Update();
        }


        public override Collider GetCollider()
        {
            return hitbox;
        }


        public void Teleport()
        {
            shouldTeleport = false;
            for (int tries = 0; tries < 10; tries++)
            {
                Vector3 destination;
                float distance = Random.Range(8, 16);
                float angle = Random.Range(0, 360);
                if (targetObject != null)
                {
                    Vector3 toTarget = targetObject.transform.position - transform.position;
                    destination = transform.position + (toTarget * Random.value);
                }
                else
                {
                    destination = transform.position;
                }
                int tilex = (int)(destination.x + distance * Mathf.Cos(angle));
                int tilez = (int)(destination.z + distance * Mathf.Sin(angle));
                destination.x = tilex + 0.5f;
                destination.z = tilez + 0.5f;
                destination.y = dungeon.map.GetNFloorY(tilex, tilez) + 1.5f;
                if (dungeon.map.GetPassableAndSafe(tilex, tilez)
                    && ((targetObject == null) || (CanSeeTargetFrom(destination))))
                {
                    Instantiate(telepuff, transform.position, transform.rotation);
                    Instantiate(telepuff, destination, Quaternion.identity);
                    destination.y = destination.y - 1.5f;
                    GetComponent<Rigidbody>().position = destination;
                    RoutingAgent.Warp(destination);
                    if (targetObject != null) transform.rotation
                             .SetLookRotation(targetObject.transform.position - destination, Vector3.up);
                    break;
                }
            }
        }


        public bool CanSeeColliderFrom(GameObject other, Vector3 location)
        {
            Vector3 otherLoc = other.GetComponent<Collider>().bounds.center;
            Vector3 toOther = otherLoc - location;
            return ((toOther.sqrMagnitude < aggroRangeSq)
                && !Physics.Linecast(eyes.position, otherLoc, GameConstants.LevelMask));
        }


        public bool CanSeeTargetFrom(Vector3 location)
        {
            return ((targetObject != null)
                && (targetObject.GetComponent<Collider>() != null)
                && CanSeeColliderFrom(targetObject, location));
        }


        public override void Die(Damages damages)
        {
            shouldTeleport = false;
            Instantiate(deathExplosion, transform.position, transform.rotation);
            Destroy(gameObject);
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
            Teleport();
            return base.TakeDamage(ref damage);
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
            if (!isDead && (other.gameObject == targetObject))
            {
                shouldTeleport = true;
            }
        }


        public override void TriggerLeft(Collider other) {/*Not Applicable*/}


        public override void MeleeAttack() {/*Not Applicable*/}


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
            anim.SetInteger("AnimID", 1);
            entitySounds.PlayAttack(voice, 0);
        }
    }

}