using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CevarnsOfEvil 
{

    public abstract partial class ChaoticFlyer : EntityMob
    {
        [SerializeField] protected Rigidbody rigid;
        protected Vector3 direction3d;
        protected float prefferedSpeed;
        protected float wanderUpdateTime;
        protected bool shouldTurn = false;

        // Accessors
        public float PrefferedSpeed { get => prefferedSpeed; set { prefferedSpeed = value; } }
        public float WanderUpdateTime { get => wanderUpdateTime; set { wanderUpdateTime = value; } }
        public bool ShouldTurn { get { return shouldTurn; } set { shouldTurn = value; } }

        public Vector3 DesiredDirection { get {return desiredDirection; } set {desiredDirection = value; } }


        // Start is called before the first frame update
        public override void Start()
        {
            base.Start();
        }


        // Update is called once per frame
        public override void Update()
        {
                if (!currentBehavior.StateUpdate(this)) FindNewBehavior();
                float tFactor = Time.deltaTime * 10;
            }


        public void SetAnimSpeed(float speed)
        {
            animSpeed = speed;
        }



#region Movement
//*

        // Non-Navmesh Movement
        protected StepDataAI GetNextStepAI()
        {
            return aiStep = dungeon.Manager.GetAIDataForGround(transform.position,
                (transform.position + direction * 0.5f), this);
        }


        protected StepDataAI GetNextMoveFlying()
        {
            return aiStep = dungeon.Manager.GetAIDataForFlying(transform.position,
                (transform.position + direction * 0.5f), this);
        }


        protected override bool IsDirectionGood()
        {
            return GetNextStepAI().Desireable;
        }


        protected override bool IsDirectionBlocked()
        {
            return !GetNextStepAI().Valid;
        }


        protected override bool IsFlyingBlocked()
        {
            return !GetNextMoveFlying().Valid;
        }


        public override void SetDirection(Vector3 dir)
        {
            desiredDirection = dir;
        }


        public override void TurnToDestination()
        {
            desiredDirection = destination - transform.position;
            if (desiredDirection != Vector3.zero) desiredDirection.Normalize();
        }


        public override void FaceHeading()
        {
            transform.LookAt(transform.position + direction);
        }


        public override void SetDirectionZero()
        {
            AIVelocity = direction3d = direction = desiredDirection = Vector3.zero;
        }


        public override void SetMovement()
        {
            float turnFactor = Time.deltaTime * turnSpeed;
            direction3d = (desiredDirection * turnFactor) + (direction3d * (1 - turnFactor));
            direction = direction3d;
            direction.y = 0;
            if (direction3d != Vector3.zero) direction3d.Normalize();
            if (direction != Vector3.zero) direction.Normalize();
            AIVelocity = baseMoveSpeed * direction3d;
        }


        public override void Move()
        {
            SetMovement();   
            rigid.velocity = AIVelocity;
        }


        

//*/
#endregion


   }

}