using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CevarnsOfEvil
{

    [RequireComponent(typeof(CharacterController))]
    public class EntitySeekerFollower : EntityMob, IDestinationSeeker
    {
        [SerializeField] protected NavSeeker seeker;
        [SerializeField] protected CharacterController controller;

        protected float navmeshTimer;
        protected bool useNavmesh;

        protected Vector3 heading;
        protected Vector3 movement;
        public Vector3 hVelocity;
        public Vector3 velocity;
        public float vSpeed;
        protected bool onGround, shouldJump;


        public float NavmeshTimer { get { return navmeshTimer; } set { navmeshTimer = value; } }


        public bool CanReachDestinationBetter()
        {
            throw new System.NotImplementedException();
        }


        public void ClearNavmeshDestination()
        {
            throw new System.NotImplementedException();
        }


        public void DisableNavmesh()
        {
            throw new System.NotImplementedException();
        }


        public void EnableNavmesh()
        {
            throw new System.NotImplementedException();
        }


        public void ForceNavmeshUpdate()
        {
            throw new System.NotImplementedException();
        }

        public override void GetAimParams(out AimParams aim)
        {
            throw new System.NotImplementedException();
        }

        public bool LineToTargetClear()
        {
            throw new System.NotImplementedException();
        }


        public void SetDestination(Vector3 destination)
        {
            throw new System.NotImplementedException();
        }


        public void SetDestinationAndUpdate(Vector3 destination)
        {
            throw new System.NotImplementedException();
        }

        public void SetNavmeshDestination(Vector3 destination)
        {
            throw new System.NotImplementedException();
        }


        public void SetNavmeshDestination()
        {
            throw new System.NotImplementedException();
        }


        public void SetRandomDestination(int range)
        {
            throw new System.NotImplementedException();
        }


        public void SetRandomDestinationCurrent(int range)
        {
            throw new System.NotImplementedException();
        }


        public void SetRandomDestinationTarget(int range)
        {
            throw new System.NotImplementedException();
        }


        public void UpdateNavmesh()
        {
            throw new System.NotImplementedException();
        }


        public void Move()
        {
            // TODO: Handle movement.  That means facing and following the seeker, and maybe jumping
        }
    }

}
