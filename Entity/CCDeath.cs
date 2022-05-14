using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CevarnsOfEvil
{

    public class CCDeath : EntityDeath
    {

        [SerializeField] CCPhysics ccphysics;


        protected virtual void Awake()
        {
            if(ccphysics == null) ccphysics = GetComponent<CCPhysics>();
        }


        protected virtual void OnEnable()
        {
            ccphysics.Die();
        }


        protected override void Update()
        {
            Entity mob = gameObject.GetComponent<EntityMob>();
            if(mob != null) 
            {
                float floorHeight = mob.Manager.Dungeon.map.GetFloorY((int)(transform.position.x + 0.5f), 
                        (int)(transform.position.z + 0.5f));
                if(transform.position.y < floorHeight) 
                {
                    Vector3 landing = transform.position;
                    landing.y = floorHeight;
                    transform.position = landing;
                    ForceImmediate();
                    return;
                }
            }
            if (ccphysics.ShouldCorpseStop())
            {
                ccphysics.enabled = false;
                foreach (Collider collider in colliders) { collider.enabled = false; }
                enabled = false;
            }
        }



        public override void ForceImmediate() 
        {
            ccphysics.enabled = false;
            foreach (Collider collider in colliders) { collider.enabled = false; }
            enabled = false;
        }


        public override void Reset()
        {
            foreach (Collider collider in colliders) { collider.enabled = true; }
            ccphysics.enabled = true;
        }
    }

}
