using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CevarnsOfEvil
{

    public class FlyingDeath : EntityDeath
    {
        public const float SLOW_GRAVITY = -3.13155712067f;
        private static readonly Vector3 SLOW_GRAV_VEC = new(0, SLOW_GRAVITY, 0); 


        public override void Update()
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            int floory = GameManager.Instance.Dungeon.map.GetFloorY((int)transform.position.x, (int)transform.position.z);
            if((floory >= transform.position.y) || (rb && (rb.velocity.magnitude < 0.01) && (rb.angularVelocity.magnitude < 0.01f)))
            {
                rb.isKinematic = true;
                rb.Sleep();
                foreach (Collider collider in colliders) { collider.enabled = false; }
                Vector3 restingPlace = transform.position;
                restingPlace.y = floory;
                transform.position = restingPlace;
                enabled = false;
            }
        }
    }


}