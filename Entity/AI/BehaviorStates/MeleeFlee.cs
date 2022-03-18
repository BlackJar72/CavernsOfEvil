using UnityEngine;
using UnityEngine.AI;


namespace CevarnsOfEvil
{

    [CreateAssetMenu(menuName = "DLD/AI/Melee Flee", fileName = "MeleeFlee", order = 6)]
    public class MeleeFlee : BehaviorObject
    {

        public override bool StateUpdate(EntityMob entityMob)
        {
            EntityNavMeshUser ownerIn = entityMob as EntityNavMeshUser;
            if (IsValidState(ownerIn))
            {
                if (!ownerIn.CanReachDestination)
                {
                    if(Random.value < 0.5f)
                    {
                        ownerIn.CurrentBehavior = ownerIn.PreviousBehavior;
                    }
                    SetFleeDestination(ownerIn);
                }
                else if (ownerIn.RoutingAgent.remainingDistance < 1) return false;
                return true;
            }
            ownerIn.RoutingAgent.isStopped = true;
            ownerIn.FindNewBehavior();
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
            SetFleeDestination(ownerIn);
            ownerIn.alerted = false;
        }


        public override void StateExit(EntityMob ownerIn)
        {
            ownerIn.IsFleeing = false;
        }


        private void SetFleeDestination(EntityNavMeshUser ownerIn)
        {
            Level dungeon = ownerIn.Dungeon;
            Room rroom = null;
            while (rroom == null)
            {
                rroom = dungeon.rooms[Random.Range(0, dungeon.rooms.Count)];
            }
            Vector3? destination = rroom.GetRandomDestination();
            if (destination != null) ownerIn.SetDestinationAndUpdate((Vector3)destination);
        }
    }


}