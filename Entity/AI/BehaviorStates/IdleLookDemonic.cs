using UnityEngine;


namespace CevarnsOfEvil
{
    [CreateAssetMenu(menuName = "DLD/AI/Idle Look (Demons)", fileName = "IdleLook", order = 7)]
    public class IdleLookDemonic : BehaviorObject
    {
        [SerializeField] BehaviorObject onSeePlayer;
        [SerializeField] float vocalRate = 1.0f;

        public override bool StateUpdate(EntityMob entityMob)
        {
            EntityMob ownerIn = entityMob;
            if (ownerIn.LookForPlayer() || (ownerIn.targetEntity != null))
            {
                ownerIn.CurrentBehavior = onSeePlayer;
            }
            else 
            {
                float time = Time.time;
                if ((ownerIn.NextIdleTalk < time) && (Random.value < (Time.deltaTime * vocalRate)))
                {
                    ownerIn.Sounds.PlayIdle(ownerIn.Voice);
                    ownerIn.NextIdleTalk += (2 / vocalRate) + (Random.value * 3);
                }
                WingedImp demon = (ownerIn as WingedImp);
                if (demon.NextChangeIdle < time)
                {
                    demon.RefreshIdle();
                }
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
            if (ownerIn)
            {
                ownerIn.SetFactorSpeed(AnimMoveSpeed);
                ownerIn.ClearNavmeshDestination();
                if ((ownerIn.RoutingAgent != null)
                    && ownerIn.RoutingAgent.isActiveAndEnabled
                    && ownerIn.RoutingAgent.isOnNavMesh)
                {
                    ownerIn.RoutingAgent.isStopped = true;
                }
            }
        }


        public override void StateExit(EntityMob ownerIn)
        {
            ownerIn.Sounds.PlayAggro(ownerIn.Voice, 0);
        }
    }
}