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
                if ((ownerIn.RoutingAgent.remainingDistance < 1))
                {
                    if(ownerIn.NavmeshTimer < Time.time)
                    {
                        SetFleeDestination(ownerIn);
                    }
                    else return false; 
                }
                if (!ownerIn.CanReachDestination)
                {
                    SetFleeDestination(ownerIn);
                }
                return true;
            }
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
            ownerIn.StasisAI = Mathf.Max(ownerIn.StasisAI, Time.time + Random.Range(5.0f, 15.0f));
            ownerIn.alerted = false;
        }


        public override void StateExit(EntityMob ownerIn)
        {
            ownerIn.IsFleeing = false;
            if(Random.value < 0.5f)
            {
                ownerIn.RemoveTarget();
            }
        }


        private void SetFleeDestination(EntityNavMeshUser ownerIn)
        {
            Level dungeon = ownerIn.Dungeon;
            GameManager manager = dungeon.Manager;
            Room rroom = null;
            for(int tries = 0; tries < 5; tries++)
            while (rroom == null)
            {
                rroom = dungeon.rooms[Random.Range(0, dungeon.rooms.Count)];
            } 
            Vector3? destination = rroom.GetRandomDestination();
            if (destination != null)
            {
                StepDataAI stepdata = manager.GetAIDataForGround(ownerIn.transform.position, 
                    (Vector3)destination, ownerIn);
                if (stepdata.Desireable)
                {
                    ownerIn.SetDestinationAndUpdate((Vector3)destination);
                    return;
                }
            }
            ownerIn.ForceNavmeshUpdate();
        }
    }


}