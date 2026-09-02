using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


namespace CevarnsOfEvil
{
    [RequireComponent(typeof(Rigidbody))]
    public abstract partial class EntityNavMeshUser : EntityMob
    {
        //Accessors
        public NavMeshAgent RoutingAgent { get { return navMeshAgent; } }

        // Delegates
        protected SetAnimSpeed setAnimByNavmesh;
        protected Rigidbody rb;


        public override void Start()
        {
            rb = GetComponent<Rigidbody>();
            navMeshAgent = GetComponent<NavMeshAgent>();
            if(!navMeshAgent.isOnNavMesh) Destroy(gameObject);
            navMeshAgent.stoppingDistance = meleeStopDistance
                = Mathf.Clamp(meleeRange - 1, meleeRange / 2, 3)
                    + GetCollider().bounds.extents.z;
            anim = GetComponent<Animator>();
            aggroRangeSq = aggroRange * aggroRange;
            CurrentBehavior = EmptyState.Instance.NextState(this);
            player = GameObject.Find("FemalePlayer"); 
            if(CanSeeCollider(player)) Destroy(gameObject); // Should not start ready to aggro!
            navmeshTimer = enviroCooldown = nextIdleTalk = stasisAI = nextAttack = Time.time;
            setAnimByNavmesh = new SetAnimSpeed(SetAnimSpeedNavMesh);
            setAnimByVelocity = new SetAnimSpeed(SetAnimSpeedVelocity);
            setAnimToZero = new SetAnimSpeed(SetAnimSpeedZero);
            setAnimSpeed = setAnimByNavmesh;
            wandering = fleeing = false;
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
            setAnimSpeed();
        }


        private void SetAnimSpeedNavMesh()
        {
            float tFactor = Time.deltaTime * 10;
            animSpeed = (navMeshAgent.desiredVelocity.magnitude * tFactor) + (animSpeed * (1 - tFactor));
            anim.SetFloat("SpeedFactor", animSpeed);
        }


        public virtual void SetFactorSpeed(float speedFactor)
        {
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

        
        public virtual void FixedUpdate()
        {
            rb.velocity *= 0.95f;
        }



    }

}