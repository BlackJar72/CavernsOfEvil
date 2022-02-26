using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


namespace DLD
{
    public abstract partial class EntityMob : Entity
    {
        [SerializeField] protected float turnSpeed = 10;
        [SerializeField] protected Transform feet;

        protected Vector3 destination;
        protected Vector3 direction, desiredDirection;
        protected Vector3 velocity, movement, AIVelocity, physicalVelocity;
        protected float vSpeed;

        protected bool onGround;
        protected bool shouldJump;

        protected StepDataAI aiStep;
        protected Collider[] footContats;

        [SerializeField] protected BehaviorObject[] behaviorStates;
        protected IBehaviorState currentBehavior = EmptyState.Instance;
        protected IBehaviorState previousBehavior = EmptyState.Instance;
        protected float stasisAI; // To force some AI state to linger

        protected NavMeshAgent routingAgent;
        protected float navmeshTimer;
        protected bool useNavmesh;


        //Properties
        public BehaviorObject[] Behaviors { get { return behaviorStates; } }
        public IBehaviorState CurrentBehavior
        {
            get { return currentBehavior; }
            set
            {
                currentBehavior.StateExit(this);
                previousBehavior = currentBehavior;
                currentBehavior = value;
                currentBehavior.StateEnter(this);
            }
        }
        public IBehaviorState PreviousBehavior { get { return previousBehavior; } }
        public NavMeshAgent RoutingAgent { get { return routingAgent; } }


        #region Behavior States
        /// <summary>
        /// This will look for a new state in the base list of 
        /// states by priority.  This is called if the current 
        /// state is no longer valid.
        /// </summary>
        public void FindNewBehavior()
        {
            CurrentBehavior = EmptyState.Instance.NextState(this);
        }


        #endregion



        #region Movement
        #region NavMesh Integration
        // NavMesh integration
        public void SetNavmeshDestination(Vector3 destination)
        {
            this.destination = destination;
            useNavmesh = true;
            setAnimSpeed = SetAnimSpeedNavMesh;
        }

        public void SetDestination(Vector3 destination)
        {
            this.destination = destination;
        }


        public void ClearNavmeshDestination()
        {
            if ((routingAgent != null) && routingAgent.isActiveAndEnabled && routingAgent.isOnNavMesh)
            {
                routingAgent.ResetPath();
            }
            useNavmesh = false;
            setAnimSpeed = SetAnimSpeedVelocity;
        }


        public void SetNavmeshDestination()
        {
            if ((routingAgent != null) && routingAgent.isActiveAndEnabled && routingAgent.isOnNavMesh)
            {
                routingAgent.SetDestination(destination);
            }
        }


        public void EnableNavmesh()
        {
            useNavmesh = true;
            navmeshTimer = Time.time + Random.value;
            setAnimSpeed = SetAnimSpeedNavMesh;
        }


        public void DisableNavmesh()
        {
            useNavmesh = false;
            routingAgent.isStopped =  true;
            routingAgent.ResetPath();
            setAnimSpeed = SetAnimSpeedVelocity;
        }


        public void UpdateNavmesh()
        {
            if (useNavmesh && (routingAgent.destination != destination) && routingAgent.isOnNavMesh
                && ((Time.time > navmeshTimer)
                    || (routingAgent.remainingDistance < routingAgent.stoppingDistance)))
            {
                routingAgent.SetDestination(destination);
                navmeshTimer = Time.time + 1.0f;
            }
        }


        public void SetDestinationAndUpdate(Vector3 destination)
        {
            SetNavmeshDestination(destination);
            UpdateNavmesh();
        }


        public bool LineToTargetClear()
        {            
            NavMeshHit hit;
            return !routingAgent.Raycast(targetObject.transform.position, out hit);
        }
        #endregion


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


        public void SetDirection(Vector3 dir)
        {
            desiredDirection = dir;
        }


        public void StartTurnToDestination()
        {
            desiredDirection = destination - transform.position;
            if(desiredDirection != Vector3.zero) desiredDirection.Normalize();
        }


        public void FaceHeading()
        {
            transform.LookAt(transform.position + direction);
        }


        public void SetDirectionZero()
        {
            AIVelocity = direction = desiredDirection = Vector3.zero;
            setAnimSpeed = setAnimToZero;
            animSpeed = 0;
        }


        public virtual void SetMovement()
        {
            float turnFactor = Time.deltaTime * turnSpeed;
            direction = (desiredDirection * turnFactor) + (direction * (1 - turnFactor));
            if (direction != Vector3.zero) direction.Normalize();
            AIVelocity = direction * baseMoveSpeed;
        }


        public virtual void Move()
        {
            if (stepData.floorEffect == FloorEffect.ice)
            {
                float slipFactor = Time.deltaTime * 1.5f;
                movement = (AIVelocity * slipFactor) + (movement * (1 - slipFactor));
            }
            else movement = AIVelocity;

            shouldJump = onGround = IsOnGround();

            if (onGround)
            {
                if (shouldJump)
                {
                    vSpeed = 5;
                    // TODO: Jump in animation controller
                    //animator.SetTrigger("Jump");
                }
                else
                {
                    vSpeed = Mathf.Max(vSpeed, 0);
                }
            }
            else
            {
                vSpeed -= 15 * Time.deltaTime;
            }

            velocity = movement + physicalVelocity;
            velocity.y += vSpeed;
            GetComponent<CharacterController>().Move(velocity * Time.deltaTime);
            //transform.Translate(velocity * Time.deltaTime, Space.World);
            shouldJump = false;
            physicalVelocity *= (1 - Time.deltaTime);
            AIVelocity = Vector3.zero;
        }


        public bool IsOnGround()
        {
            if(feet != null) { 
                return (Physics.OverlapSphereNonAlloc(feet.position,
                    0.1f, footContats, GameConstants.LevelMask) > 0);
            }
            return true;
        }


        #endregion


        #region Senses

        /// <summary>
        /// Test to see if the mob could see the game object by first testing 
        /// range (cheapest test), them to see of the game object is in front 
        /// of the mob (cosine > 0), then if the game object is not behind 
        /// anything.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool CanSeeCollider(GameObject other)
        {
            Vector3 otherLoc = other.GetComponent<Collider>().bounds.center;
            Vector3 toOther = otherLoc - eyes.position;
            return ((toOther.sqrMagnitude < aggroRangeSq)
                && (Vector3.Dot(eyes.forward, toOther) > 0)
                && !Physics.Linecast(eyes.position, otherLoc, GameConstants.LevelMask));
        }


        /// <summary>
        /// Test to see if the mob could see the game object by first testing 
        /// range (cheapest test), them to see of the game object is in front 
        /// of the mob (cosine > 0), then if the game object is not behind 
        /// anything.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool CanSeeCollider(Entity other)
        {
            Vector3 otherLoc = other.GetCollider().bounds.center;
            Vector3 toOther = otherLoc - eyes.position;
            return ((toOther.sqrMagnitude < aggroRangeSq)
                && (Vector3.Dot(eyes.forward, toOther) > 0)
                && !Physics.Linecast(eyes.position, otherLoc, GameConstants.LevelMask));
        }


        public bool CanSeeTarget()
        {
            return ((targetObject != null)
                && (targetObject.GetComponent<Collider>() != null)
                && CanSeeCollider(targetObject));
        }


        public bool LookForPlayer()
        {
            bool output = (((player != null) && CanSeeCollider(player))
                || alerted) && !player.GetComponent<MovePlayer>().flying;
            if (output)
            {
                SetTarget(player);
            }
            return output;
        }


        public bool ListenForPlayer(GameObject player)
        {
            // For now treat hearability as line of sight.
            // TODO: This *OR* check of same (or adjacent?) room.
            //
            // (Using eyes and a proxy for all sense organs, being typically
            // near the ears and betters than the using the center of mass.
            alerted = InSameRoom(player) ||
                !Physics.Linecast(eyes.position,
                player.GetComponent<Collider>().bounds.center,
                GameConstants.LevelMask);
            return alerted;
        }


        public bool HearAllies(Vector3 allyVoiceLocation)
        {
            // For now treat hearability as line of sight.
            // TODO: This *OR* check of same (or adjacent?) room.
            alerted = InSameRoom(allyVoiceLocation) ||
                !Physics.Linecast(eyes.position, allyVoiceLocation,
                GameConstants.LevelMask);
            return alerted;
        }

        public void BeAlerted()
        {
            alerted = true;
        }

        #endregion

    }

}