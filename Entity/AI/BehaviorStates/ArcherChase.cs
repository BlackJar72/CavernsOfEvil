using UnityEngine;


namespace CevarnsOfEvil {


    [CreateAssetMenu(menuName = "DLD/AI/Archer Chase", fileName = "ArcherChase", order = 11)]
    public class ArcherChase : BehaviorObject
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
            // TODO: Go to maneuver when close to and can see target!
            ownerIn.SetDestinationAndUpdate(ownerIn.targetObject.transform.position);
            Vector3 toTarget = ownerIn.targetObject.transform.position - ownerIn.transform.position;
            IArcher archer = ownerIn as IArcher;
            if (ownerIn.CanSeeTarget())
            {
                if (archer.ReadyToShoot && (ownerIn.NextAttack < Time.time))
                {
                    ownerIn.RoutingAgent.stoppingDistance = 10;
                    ownerIn.CurrentBehavior = attackState;
                }
                else if(toTarget.sqrMagnitude < 64)
                {
                    ownerIn.RoutingAgent.stoppingDistance = ownerIn.MeleeStopDistance;
                    ownerIn.SetNavmeshDestination(ownerIn.transform.position - toTarget.normalized * 10);
                    ownerIn.ForceNavmeshUpdate();
                }
            }
            else
            {
                ownerIn.RoutingAgent.stoppingDistance = ownerIn.MeleeStopDistance;
            }

            return IsValidState(ownerIn);
        }
    }

}