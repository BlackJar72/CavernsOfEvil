using UnityEngine;


namespace CevarnsOfEvil
{

    [CreateAssetMenu(menuName = "DLD/AI/Melee Chase", fileName = "MeleeChase", order = 1)]
    public class MeleeChase : BehaviorObject
    {
        public override bool StateUpdate(EntityMob entityMob)
        {
            EntityNavMeshUser ownerIn = entityMob as EntityNavMeshUser;
            if (IsValidState(ownerIn))
            {
                ownerIn.SetDestinationAndUpdate(ownerIn.targetObject.gameObject.transform.position);
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
            ownerIn.SetFactorSpeed(AnimMoveSpeed);
            ownerIn.RoutingAgent.isStopped = false;
            ownerIn.EnableNavmesh();
        }
    }


}