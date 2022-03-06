using UnityEngine;


namespace CevarnsOfEvil
{


    [CreateAssetMenu(menuName = "DLD/AI/Skeleton Chase", fileName = "SkeletonChase", order = 25)]
    public class SkeletonChase : BehaviorObject
    {
        [SerializeField] ArcherManeuver maneuverState;
        [SerializeField] ArcherAttack attackState;


        public override void StateEnter(EntityMob entityMob)
        {
            EntityNavMeshUser ownerIn = entityMob as EntityNavMeshUser;
            ownerIn.RoutingAgent.isStopped = false;
            ownerIn.Anim.SetInteger("AnimID", AnimID);
            ownerIn.SetFactorSpeed(AnimMoveSpeed);
        }


        public override bool IsValidState(EntityMob ownerIn)
        {
            return ownerIn.targetEntity != null;
        }


        public override bool StateUpdate(EntityMob entityMob)
        {
            EntityNavMeshUser ownerIn = entityMob as EntityNavMeshUser;
            ownerIn.SetDestinationAndUpdate(ownerIn.targetObject.transform.position);
            Vector3 toTarget = ownerIn.targetObject.transform.position - ownerIn.transform.position;
            IArcher archer = ownerIn as IArcher;
            if (AIHelper.CanShootTarget(ownerIn))
            {
                if (archer.ReadyToShoot && (ownerIn.NextAttack < Time.time))
                {
                    ownerIn.RoutingAgent.stoppingDistance = 10;
                    ownerIn.CurrentBehavior = attackState;
                }
            }
            else
            {
                ownerIn.RoutingAgent.stoppingDistance = ownerIn.MeleeStopDistance;
            }
            if((ownerIn.RoutingAgent.velocity.sqrMagnitude > 0.1) && (Random.value < Time.deltaTime))
            {
                ownerIn.Sounds.PlayIdle(ownerIn.Voice);
                ownerIn.NextIdleTalk += 2 + (Random.value * 3);
            }

            return IsValidState(ownerIn);
        }
    }

}