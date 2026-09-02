using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CevarnsOfEvil
{


    [System.Serializable]
    public enum MoveType
    {
        idle = 0,
        crouch = 1,
        walk = 2,
        run = 3
    }


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
        protected bool onGround;
        protected bool shouldJump;
        protected Quaternion rotation;
        protected Vector3 lastPos;
        protected bool falling;


        public float NavmeshTimer { get { return navmeshTimer; } set { navmeshTimer = value; } }


#region IDestinationSeeker


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


#endregion

#region Do Movement


        public void Move()
        {
            // TODO: Handle movement.  That means facing and following the seeker, and maybe jumping
        }


#endregion

#region Seeker Control


        public void ActivateSeeker()
        {
            seeker.transform.parent = transform.parent;
            seeker.Agent.enabled = true;
            seeker.gameObject.SetActive(true);
        }   


        public void DeactiveSeeker()
        {
            seeker.Agent.enabled = false;
            seeker.gameObject.SetActive(false);
            seeker.transform.parent = transform;
        } 


        protected bool ShouldStop()
        {
            return /*(moveType == MoveType.idle) ||*/ seeker.Agent.isActiveAndEnabled
                || (seeker.Agent.remainingDistance <= seeker.Agent.stoppingDistance)
                || (seeker.Agent.velocity.sqrMagnitude == 0); ;
        }  


        public void StartMoving()
        {
            seeker.stopped = false;
        }


        public void StopMoving()
        {
            seeker.stopped = true;
        }  


        public bool InStopingRange()
        {
            return (destination - transform.position).sqrMagnitude
                < (seeker.Agent.stoppingDistance * seeker.Agent.stoppingDistance);
        }


#endregion

#region Overrides


        public override void Die(Damages damages)
        {
            base.Die(damages);
            StopMoving();
            DeactiveSeeker();
            controller.enabled = false;
        }


#endregion


    }



}
