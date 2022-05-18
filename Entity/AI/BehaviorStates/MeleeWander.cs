using UnityEngine;
using UnityEngine.AI;


namespace CevarnsOfEvil
{

    [CreateAssetMenu(menuName = "DLD/AI/Melee Wander", fileName = "MeleeWander", order = 5)]
    public class MeleeWander : BehaviorObject
    {
        [SerializeField] BehaviorObject fleeState;
        [SerializeField] BehaviorObject chaseState;

        public override bool StateUpdate(EntityMob entityMob)
        {
            EntityNavMeshUser ownerIn = entityMob as EntityNavMeshUser;
            if (IsValidState(ownerIn))
            {
                if (ownerIn.IsFleeing)
                {
                    ownerIn.CurrentBehavior = fleeState;
                    return true;
                }
                if (ownerIn.NavmeshTimer < Time.time)
                {
                    ownerIn.CurrentBehavior = chaseState;
                    return true;
                }
                if(Random.value > 0.5f)
                {
                    ownerIn.SetRandomDestination(5);
                }
                else
                {
                    ownerIn.SetRandomDestinationTarget(5);
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
            ownerIn.SetFactorSpeed(AnimMoveSpeed);
            ownerIn.RoutingAgent.isStopped = false;
            ownerIn.EnableNavmesh();
            ownerIn.StasisAI = Mathf.Max(ownerIn.StasisAI, Time.time + 1f + Random.value);
            ownerIn.IsWandering = true;
        }


        public override void StateExit(EntityMob ownerIn)
        {
            ownerIn.IsWandering = false;
            (ownerIn as EntityNavMeshUser).NavmeshTimer = Time.time;
        }
    }

}
