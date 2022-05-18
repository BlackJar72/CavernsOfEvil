using UnityEngine;


namespace CevarnsOfEvil
{

    [CreateAssetMenu(menuName = "DLD/AI/Melee Chase", fileName = "MeleeChase", order = 1)]
    public class MeleeChase : BehaviorObject
    {
        [SerializeField] BehaviorObject wanderState;
        [SerializeField] BehaviorObject fleeState;

        public override bool StateUpdate(EntityMob entityMob)
        {
            EntityNavMeshUser ownerIn = entityMob as EntityNavMeshUser;
            if (IsValidState(ownerIn))
            {
                ownerIn.SetDestinationAndUpdate(ownerIn.targetObject.gameObject.transform.position);
                if(ownerIn.IsFleeing)
                {
                    ownerIn.CurrentBehavior = fleeState;
                }
                else if(!ownerIn.CanReachDestinationBetter() 
                    && (ownerIn.CanSeeTarget() || ownerIn.DistanceToTarget() < 5))
                {
                    ownerIn.CurrentBehavior = wanderState;
                }
                return true;
            }
            ownerIn.RoutingAgent.isStopped = true;
            return false;
        }


        public override bool IsValidState(EntityMob ownerIn)
        {
            return (ownerIn.targetEntity != null)
                && ownerIn.targetEntity.enabled
                && (ownerIn.targetObject != null) 
                && ownerIn.targetObject.activeInHierarchy;
        }


        public override void StateEnter(EntityMob entityMob)
        {
            EntityNavMeshUser ownerIn = entityMob as EntityNavMeshUser;
            if(!ownerIn.RoutingAgent.isOnNavMesh) Destroy(ownerIn.gameObject);
            ownerIn.SetFactorSpeed(AnimMoveSpeed);
            ownerIn.RoutingAgent.isStopped = false;
            ownerIn.EnableNavmesh();
        }
    }


}