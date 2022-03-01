using UnityEngine;
using UnityEngine.AI;


namespace CevarnsOfEvil
{
    public abstract partial class EntityNavMeshUser : EntityMob
    {
        protected NavMeshAgent navMeshAgent;
        protected float navmeshTimer;
        protected bool useNavmesh;


        #region NavMesh Integration
        // NavMesh integration
        public void SetNavmeshDestination(Vector3 destination)
        {
            this.destination = destination;
            useNavmesh = true;
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
        }


        public void DisableNavmesh()
        {
            useNavmesh = false;
            navMeshAgent.isStopped = true;
            navMeshAgent.ResetPath();
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
        #endregion


    }

}