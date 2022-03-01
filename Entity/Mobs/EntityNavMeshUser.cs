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


        public override void Start()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            navMeshAgent.stoppingDistance = meleeStopDistance
                = Mathf.Clamp(meleeRange - 1, meleeRange / 2, 3)
                    + GetCollider().bounds.extents.z;
            anim = GetComponent<Animator>();
            aggroRangeSq = aggroRange * aggroRange;
            CurrentBehavior = EmptyState.Instance.NextState(this);
            player = GameObject.Find("FemalePlayer");
            navmeshTimer = enviroCooldown = nextIdleTalk = stasisAI = nextAttack = Time.time;
        }


        public override void Update()
        {
            if (!currentBehavior.StateUpdate(this)) FindNewBehavior();
#if UNITY_EDITOR
            if (dungeon != null)
            {
                stepData = dungeon.map.GetStepData(transform.position, dungeon,
                    health, ref enviroCooldown);
            }
#else
            stepData = dungeon.map.GetStepData(transform.position, dungeon,
                health, ref enviroCooldown);
#endif
            float tFactor = Time.deltaTime * 10;
            SetAnimSpeedNavMesh();
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


        public override void Deactivate()
        {
            anim.enabled = false;
            navMeshAgent.isStopped = true;
            navMeshAgent.enabled = false;
            enabled = false;
        }



    }

}