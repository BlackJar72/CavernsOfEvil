using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


namespace CevarnsOfEvil
{
    public abstract partial class EntityNavMeshUser : EntityMob
    {
        //Accessors
        public NavMeshAgent RoutingAgent { get { return navMeshAgent; } }

        // Delegates
        private SetAnimSpeed setAnimByNavmesh;


        public override void Start()
        {
            base.Start();
            navMeshAgent = GetComponent<NavMeshAgent>();
            navMeshAgent.stoppingDistance = meleeStopDistance
                = Mathf.Clamp(meleeRange - 1, meleeRange / 2, 3)
                    + GetCollider().bounds.extents.z;
            navmeshTimer = Time.time;
            setAnimByNavmesh = new SetAnimSpeed(SetAnimSpeedNavMesh);
        }


        private void SetAnimSpeedNavMesh()
        {
            float tFactor = Time.deltaTime * 10;
            animSpeed = (navMeshAgent.desiredVelocity.magnitude * tFactor) + (animSpeed * (1 - tFactor));
        }


        public virtual void SetFactorSpeed(float speedFactor)
        {
            anim.SetFloat("SpeedFactor", speedFactor);
            navMeshAgent.speed = baseMoveSpeed * speedFactor;
        }


        public override void Die(Damages damages)
        {
            base.Die(damages);
            navMeshAgent.enabled = false;
        }


        public bool InStopingRange()
        {
            return (destination - transform.position).sqrMagnitude
                < (navMeshAgent.stoppingDistance * navMeshAgent.stoppingDistance);
        }



    }

}