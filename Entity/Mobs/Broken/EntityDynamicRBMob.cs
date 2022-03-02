using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CevarnsOfEvil
{

    public class EntityDynamicRBMob : EntityMob
    {
        [SerializeField] protected Rigidbody rb;


        public override void Start()
        {
            base.Start();
            anim = GetComponent<Animator>();
            aggroRangeSq = aggroRange * aggroRange;
            player = GameObject.Find("FemalePlayer");
            enviroCooldown = nextIdleTalk = stasisAI = nextAttack = Time.time;
            IsOnGround = IsOnPhyscialGround;
            CurrentBehavior = EmptyState.Instance.NextState(this);
            collider = GetComponent<CharacterController>();
        }



        public virtual void FixedUpdate()
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
            Move();
        }


        public override Collider GetCollider()
        {
            return collider;
        }

        #region Movement


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


        protected virtual bool IsDirectionGood()
        {
            return GetNextStepAI().Desireable;
        }


        protected virtual bool IsDirectionBlocked()
        {
            return !GetNextStepAI().Valid;
        }


        protected virtual bool IsFlyingBlocked()
        {
            return !GetNextMoveFlying().Valid;
        }


        public virtual void SetDirection(Vector3 dir)
        {
            desiredDirection = dir;
        }


        public virtual void TurnToDestination()
        {
            desiredDirection = destination - transform.position;
            desiredDirection.y = 0;
            if (desiredDirection != Vector3.zero) desiredDirection.Normalize();
        }


        public virtual void FaceHeading()
        {
            transform.LookAt(transform.position + direction);
        }


        public virtual void SetDirectionZero()
        {
            AIVelocity = direction = desiredDirection = Vector3.zero;
        }


        public virtual void SetMovement()
        {
            float turnFactor = Time.deltaTime * turnSpeed;
            direction = (desiredDirection * turnFactor) + (direction * (1 - turnFactor));
            if (direction != Vector3.zero) direction.Normalize();
            AIVelocity = animSpeed * baseMoveSpeed * direction;
        }


        public virtual void Move()
        {
            SetMovement();

            if (stepData.floorEffect == FloorEffect.ice)
            {
                float slipFactor = Time.deltaTime * 1.5f;
                movement = (AIVelocity * slipFactor) + (movement * (1 - slipFactor));
            }
            else movement = AIVelocity;

            //onGround = IsOnGround();

            velocity = movement;
            if (rb.useGravity) velocity.y = vSpeed = rb.velocity.y;
            shouldJump = false;
            rb.velocity = velocity;
            AIVelocity = Vector3.zero;
        }


        public virtual bool IsOnDungeonGround()
        {
            float offFloor = feet.position.y - dungeon.map.GetFloorY((int)feet.position.x, (int)feet.position.z);
            return offFloor < 0.05f;
        }


        // For some reason this doesn't work for mobs, always returning false, despites
        // the exact same logic working perfectly for the player character -- weird!
        public virtual bool IsOnPhyscialGround()
        {
            return (Physics.OverlapSphereNonAlloc(feet.position,
                0.1f, footContats, GameConstants.LevelMask) > 0);
        }

        public override void GetAimParams(out AimParams aim)
        {
            throw new System.NotImplementedException();
        }


        #endregion

    }

}