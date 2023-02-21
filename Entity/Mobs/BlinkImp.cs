using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


namespace CevarnsOfEvil
{

    public class BlinkImp : EntityRangedNavMeshUser
    {
        [SerializeField] protected Collider hitbox;
        [SerializeField] protected GameObject telepuff;
        [SerializeField] protected GameObject deathExplosion;


        private bool shouldtp = false;


        public override Collider GetCollider()
        {
            return hitbox;
        }


        public override void Update() {
            if(shouldtp || (targetEntity && ((Random.value < (0.1 * Time.deltaTime)) || (DistanceToTarget() < 2.5f)))) {
                Teleport();
            }
            base.Update();
        }


        public void Teleport()
        {
            for (int tries = 0; tries < 10; tries++)
            {
                Vector3 destination;
                float distance = Random.Range(4, 16);
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
                if (dungeon.map.GetInBounds(tilex, tilez) && dungeon.map.GetPassableAndSafe(tilex, tilez))
                {
                    NavMeshHit hit;
                    destination.y = dungeon.map.GetFloorY(tilex, tilez);
                    if (NavMesh.SamplePosition(destination, out hit, 0.25f, NavMesh.AllAreas)
                            && CanSeeTargetFrom(destination)) {
                        shouldtp = false;
                        Instantiate(telepuff, transform.position, transform.rotation);
                        Instantiate(telepuff, destination, Quaternion.identity);
                        destination.y = destination.y;
                        GetComponent<Rigidbody>().position = destination;
                        RoutingAgent.Warp(destination);
                        if (targetObject != null) transform.rotation
                                    .SetLookRotation(targetObject.transform.position - destination, Vector3.up);
                        break;
                    }
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
            location.y += 1.5f;
            return ((targetEntity != null)
                && (targetEntity.GetCollider() != null)
                && !Physics.Linecast(location, targetEntity.GetCollider().bounds.center, GameConstants.LevelMask));
        }


        public override void Die(Damages damages)
        {
            Debug.Log("Imp Died!");
            Instantiate(deathExplosion, transform.position, transform.rotation);
            Destroy(gameObject);
        }


        public override bool TakeDamage(ref Damages damage)
        {
            if (damage.type == DamageType.fire) return false;
            else {
                shouldtp = shouldtp || (Random.value < 0.666);
            }
            return !isDead;
        }


        public override void Attack()
        {
            if (nextFireTime < Time.time)
            {
                anim.SetTrigger("Attack");
                nextAttack = Time.time + attackTime;
                SetFactorSpeed(0.0f);
                RoutingAgent.isStopped = true;
            }
        }


        public override void TriggerHit(Collider other)
        {
            if (!isDead && (nextAttack < Time.time) && (other.gameObject == targetObject))
            {
                nextAttack = Time.time + attackTime;
                entitySounds.PlayAttack(voice, 0);

                MeleeAttack();
            }
        }


        public override void TriggerLeft(Collider other)
        {
            if (!isDead && (other.gameObject == targetObject))
            {

            }
        }


        public override void MeleeAttack()
        {
            base.MeleeAttack();
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
            entitySounds.PlayAttack(voice, 0);
        }
    }

}