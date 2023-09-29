using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CevarnsOfEvil
{

    public class Projectile : BasicProjectile
    {
        [SerializeField] SpawnableByProjectile impactEffect;

        /*
         * If this game was intended to include outdoor settings these would 
         * need a time-to-live or a max-distance-traveled (range) and an update 
         * method to destroy them after reaching that limit.  As they are being 
         * created for a game where they will be spawned indoors and never go 
         * far the extra check is a waste of processing; however, this is 
         * something to remember (and possibly change) if re-used in a different 
         * setting.
         */ 


        /// <summary>
        /// Handle collision by applying damage to damagable targets and 
        /// instantiating on special effects (such particle, explosion, or 
        /// summons), before destroying the projectile.
        /// </summary>
        /// <param name="collision"></param>
        public override void OnCollisionEnter(Collision collision)
        {
            Entity victim = collision.gameObject.GetComponent<Entity>();
            if (victim != null)
            {
                EntityHealth health = collision.gameObject.GetComponent<Entity>().Health;
                health.BeHitByAttack(damageBase, damageType, attacker);
            }
            Instantiate(impactEffect.gameObject, transform.position, transform.rotation)
                .GetComponent<SpawnableByProjectile>().OnSpawn(gameObject, collision, attacker);
            Destroy(gameObject); 
        }

    }
}