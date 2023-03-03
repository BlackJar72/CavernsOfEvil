using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CevarnsOfEvil
{

    [CreateAssetMenu(menuName ="DLD/AI/Idle Look", fileName = "IdleLook", order = 0)]
    public class IdleLook : BehaviorObject
    {
        [SerializeField] BehaviorObject onSeePlayer;
        [SerializeField] float vocalRate = 1.0f;

        public override bool StateUpdate(EntityMob entityMob)
        {
            EntityNavMeshUser ownerIn = entityMob as EntityNavMeshUser;
            if (ownerIn.LookForPlayer() || (ownerIn.targetEntity != null))
            {
                ownerIn.CurrentBehavior = onSeePlayer;
            } 
            else if((ownerIn.NextIdleTalk < Time.time) && (Random.value < (Time.deltaTime * vocalRate)
                    && ownerIn.DistanceSqrToPlayer() < 1024))
            {
                ownerIn.Sounds.PlayIdle(ownerIn.Voice);
                ownerIn.NextIdleTalk += (2 / vocalRate) + (Random.value * 3);
            }
            return true;
        }


        public override bool IsValidState(EntityMob ownerIn)
        {
            return true;
        }


        public override void StateEnter(EntityMob entityMob)
        {
            EntityNavMeshUser ownerIn = entityMob as EntityNavMeshUser;
            ownerIn.SetFactorSpeed(AnimMoveSpeed);
            ownerIn.ClearNavmeshDestination();
            if ((ownerIn.RoutingAgent != null)
                && ownerIn.RoutingAgent.isActiveAndEnabled
                && ownerIn.RoutingAgent.isOnNavMesh)
            {
                ownerIn.RoutingAgent.isStopped = true;
            }
        }


        public override void StateExit(EntityMob ownerIn)
        {
            ownerIn.Sounds.PlayAggro(ownerIn.Voice, 0);
        }
    }

}