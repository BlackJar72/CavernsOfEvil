using UnityEngine;
using UnityEngine.AI;


namespace CevarnsOfEvil
{
    public abstract partial class EntityNavMeshUser : EntityMob
    {
        protected NavMeshAgent navMeshAgent;
        protected float navmeshTimer;
        protected bool useNavmesh;

        public float NavmeshTimer { get { return navmeshTimer; } set { navmeshTimer = value; } }


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
            if ((navMeshAgent != null) && navMeshAgent.isActiveAndEnabled && navMeshAgent.isOnNavMesh)
            {
                navMeshAgent.ResetPath();
            }
            useNavmesh = false;
            setAnimSpeed = SetAnimSpeedVelocity;
        }


        public void SetNavmeshDestination()
        {
            if ((navMeshAgent != null) && navMeshAgent.isActiveAndEnabled && navMeshAgent.isOnNavMesh)
            {
                navMeshAgent.SetDestination(destination);
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
            navMeshAgent.isStopped = true;
            navMeshAgent.ResetPath();
            setAnimSpeed = SetAnimSpeedVelocity;
        }


        public void UpdateNavmesh()
        {
            if (useNavmesh && (navMeshAgent.destination != destination) && navMeshAgent.isOnNavMesh
                && ((Time.time > navmeshTimer)
                    || (navMeshAgent.remainingDistance < navMeshAgent.stoppingDistance)))
            {
                navMeshAgent.SetDestination(destination);
                navmeshTimer = Time.time + 1.0f;
            }
        }


        public void SetDestinationAndUpdate(Vector3 destination)
        {
            SetNavmeshDestination(destination);
            UpdateNavmesh();
        }


        public void ForceNavmeshUpdate()
        {
            navMeshAgent.SetDestination(destination);
            navmeshTimer = Time.time + 1.0f;
        }


        public bool LineToTargetClear()
        {
            NavMeshHit hit;
            return !navMeshAgent.Raycast(targetObject.transform.position, out hit);
        }


        public virtual void MoveNM()
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
                    rigid.AddForce(new Vector3(0, 5 * rigid.mass, 0), ForceMode.Impulse);
                    anim.SetTrigger("Jump");
                }
            }

            navMeshAgent.Move(movement * Time.deltaTime);
            AIVelocity = Vector3.zero;
            shouldJump = false;
        }
        #endregion


    }

}