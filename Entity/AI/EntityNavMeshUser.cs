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

        // Accessor properties
        public override bool CanReachDestination { get 
            { 
            StepDataAI stepdata = dungeon.Manager.GetAIDataForGround(transform.position, RoutingAgent.destination, this);
            return (stepdata.Desireable ||
                    (RoutingAgent.hasPath // Don't fail just because the path has not yet been determined!
                    && RoutingAgent.pathStatus.Equals(NavMeshPathStatus.PathComplete)));} }


        #region NavMesh Integration
        // NavMesh integration
        public bool CanReachDestinationBetter()
        {
            StepDataAI stepdata = dungeon.Manager.GetAIDataForGround(transform.position, RoutingAgent.destination, this);
            return (stepdata.Desireable ||
                    (RoutingAgent.hasPath // Don't fail just because the path has not yet been determined!
                    && RoutingAgent.pathStatus.Equals(NavMeshPathStatus.PathComplete)));
        }


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

        #region Randomizers
        public void SetRandomDestination(int range)
        {
            if (useNavmesh && navMeshAgent.isOnNavMesh
                && ((Time.time > navmeshTimer)
                    || (navMeshAgent.remainingDistance < navMeshAgent.stoppingDistance)))
            {
                destination = transform.position;
                destination.x += Random.Range(-range, range + 1);
                destination.z += Random.Range(-range, range + 1);
                navMeshAgent.SetDestination(destination);
                navmeshTimer = Time.time + 1.0f;
            }

        }


        public void SetRandomDestinationCurrent(int range)
        {
            if (useNavmesh && navMeshAgent.isOnNavMesh
                && ((Time.time > navmeshTimer)
                    || (navMeshAgent.remainingDistance < navMeshAgent.stoppingDistance)))
            {
                destination.x += Random.Range(-range, range + 1);
                destination.z += Random.Range(-range, range + 1);
                navMeshAgent.SetDestination(destination);
                navmeshTimer = Time.time + 1.0f;
            }

        }


        public void SetRandomDestinationTarget(int range)
        {
            if (useNavmesh && navMeshAgent.isOnNavMesh
                && ((Time.time > navmeshTimer)
                    || (navMeshAgent.remainingDistance < navMeshAgent.stoppingDistance)))
            {
                destination = targetObject.transform.position;
                destination.x += Random.Range(-range, range + 1);
                destination.z += Random.Range(-range, range + 1);
                navMeshAgent.SetDestination(destination);
                navmeshTimer = Time.time + 1.0f;
            }

        }
        #endregion


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
        #endregion


    }

}