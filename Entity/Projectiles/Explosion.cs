using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CevarnsOfEvil
{

    public class Explosion : SpawnableByProjectile
    {
        [SerializeField] int damage;
        [SerializeField] float range;
        [SerializeField] DamageType damageType;
        [SerializeField] ParticleSystem visuals;


        public void Explode(GameObject projectile, Collision hit, Entity attacker)
        {
            Instantiate(visuals, transform.position, transform.rotation);
            Collider[] colliders = Physics.OverlapSphere(transform.position, range, GameConstants.DamageMask);
            foreach(Collider collider in colliders)
            {
                if (CheckSight(collider, projectile, hit))
                {
                    float distance = (transform.position - collider.transform.position).magnitude;
                    float effect = damage * ((range - distance) / range);
                    EntityHealth health = collider.GetComponent<EntityHealth>();
                    if (health != null)
                    {
                        if ((health.Owner == attacker) && (health.Owner is EntityMob)) continue;
                        else health.BeHitByAttack((int)effect, damageType, attacker);
                    }
                    Rigidbody rb = collider.gameObject.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        rb.AddExplosionForce(effect * 10, transform.position, range, 0, ForceMode.Impulse);
                    }
                }
            }
            Destroy(gameObject);
        }


        public bool CheckSight(Collider collider, GameObject projectile, Collision hit)
        {
            return !Physics.Linecast(hit.collider.bounds.center, collider.bounds.center, GameConstants.LevelMask);
        }


        public override void OnSpawn(GameObject projectile, Collision hit, Entity attacker)
        {
            Explode(projectile, hit, attacker);
        }
    }


}