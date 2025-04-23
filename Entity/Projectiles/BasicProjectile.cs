using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CevarnsOfEvil
{

    public class BasicProjectile : MonoBehaviour
    {
        [SerializeField] protected int damageBase;
        [SerializeField] protected float speed;
        [SerializeField] protected DamageType damageType;

        [HideInInspector] public Entity attacker;


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
        /// Sets the projectiles velocity; assumes a normalized direction vector as a direction.
        /// </summary>
        /// <param name="dir"></param>
        public void Launch(Vector3 dir, Entity attacker)
        {
            transform.rotation = Quaternion.LookRotation(dir);
            GetComponent<Rigidbody>().linearVelocity = dir * speed;
            this.attacker = attacker;
        }


        /// <summary>
        /// Sets the projectiles velocity; does not need a normalized direction vector, as 
        /// it will normalize it.
        /// </summary>
        /// <param name="dir"></param>
        public void LaunchSimple(Vector3 dir, Entity attacker)
        {
            transform.rotation = Quaternion.LookRotation(dir);
            GetComponent<Rigidbody>().linearVelocity = dir.normalized * speed;
            this.attacker = attacker;
        }


        /// <summary>
        /// Handle collision by applying damage to damagable targets and 
        /// instantiating on special effects (such particle, explosion, or 
        /// summons), before destroying the projectile.
        /// </summary>
        /// <param name="collision"></param>
        public virtual void OnCollisionEnter(Collision collision)
        {
            Entity victim = collision.gameObject.GetComponent<Entity>();
            if (victim != null)
            {
                EntityHealth health = collision.gameObject.GetComponent<Entity>().Health;
                health.BeHitByAttack(damageBase, damageType, attacker);
            }
            Destroy(gameObject);
        }
    }
}