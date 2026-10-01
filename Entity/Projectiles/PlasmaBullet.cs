using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CevarnsOfEvil
{

    public class PlasmaBullet : SimpleProjectile
    {

        private Vector3 lastPos;

        // Start is called before the first frame update
        void Start()
        {
            lastPos = transform.position;
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            Vector3 toLast = lastPos - transform.position;
            float distance = Mathf.Min(1.0f, toLast.magnitude);
            toLast = toLast.normalized;

            if (Physics.SphereCast(transform.position, 0.25f, toLast, out RaycastHit target, distance, GameConstants.MobHitMask))
            {            
                if(target.collider.gameObject.TryGetComponent<EntityHealth>(out var health))
                {
                    health.BeHitByAttack(damageBase, damageType, attacker);
                    Instantiate(impactEffect, transform.position, transform.rotation).SetActive(true);
                    Destroy(gameObject);
                }
            }
        }



    }


}