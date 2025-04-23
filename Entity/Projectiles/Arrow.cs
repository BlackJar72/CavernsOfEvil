using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CevarnsOfEvil
{

    public class Arrow : BasicProjectile
    {
        Rigidbody rb;

        private const int LEVELLAYER = 9;

        void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }


        // Update is called once per frame
        void Update()
        {
            Vector3 heading = rb.linearVelocity.normalized;
            rb.transform.forward = heading;
        }


        /// <summary>
        /// Handle collision by applying damage to damagable targets and 
        /// instantiating on special effects (such particle, explosion, or 
        /// summons), before destroying the projectile.
        /// </summary>
        /// <param name="collision"></param>
        public override void OnCollisionEnter(Collision collision)
        {
            EntityHealth victim = collision.gameObject.GetComponent<EntityHealth>();
            if (victim != null)
            {
                victim.BeHitByAttack(damageBase, damageType, attacker);
            }
            /*if (collision.gameObject.layer == LEVELLAYER)
            {
                StartCoroutine(DestroyAfterDelay());
                transform.parent = collision.transform;
                transform.position = collision.contacts[0].point;
                rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
                GetComponent<Collider>().enabled = false;
                rb.isKinematic = true;
                enabled = false;
            }
            else Destroy(gameObject);*/
            Destroy(gameObject);
        }


        private IEnumerator DestroyAfterDelay()
        {
            yield return new WaitForSeconds(60);
            Destroy(gameObject);
        }

    }

}