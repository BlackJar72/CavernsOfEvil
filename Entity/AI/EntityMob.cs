using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CevarnsOfEvil
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
        protected bool jumping;

        protected StepDataAI aiStep;
        protected Collider[] footContats;

        [SerializeField] protected BehaviorObject[] behaviorStates;
        protected IBehaviorState currentBehavior = EmptyState.Instance;
        protected IBehaviorState previousBehavior = EmptyState.Instance;
        protected float stasisAI; // To force some AI state to linger
        

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
        public Vector3 Destination { get { return destination; } set { destination = value; } }
        public Vector3 Direction { get { return direction; } set { direction = value; } }
        public Vector3 DesiredDirection { get { return desiredDirection; } 
            set { desiredDirection = value; desiredDirection.Normalize();  } }
        public Vector3 Velocity { get { return velocity; } set { velocity = value; } }
        public Vector3 Movement { get { return movement; } set { movement = value; } }
        public Vector3 PhysicalVelocity { get { return physicalVelocity; } set { physicalVelocity = value; } }
        public float VSpeed { get { return vSpeed; } set { vSpeed = value; } }
        public bool OnGround { get => onGround; }
        public bool ShouldJump { get => shouldJump; set { shouldJump = value; } }
        public float AnimSpeed { get => animSpeed; set { animSpeed = value; } }


        // Delegates
        public delegate bool IsOnGroundDelegate();
        public IsOnGroundDelegate IsOnGround;


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