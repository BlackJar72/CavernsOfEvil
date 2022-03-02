using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CevarnsOfEvil
{
    [CreateAssetMenu(menuName = "DLD/AI/Zombie Charge", fileName = "ZombieCharge", order = 21)]
    public class ZombieCharge : BehaviorObject
    {
        public override bool IsValidState(EntityMob ownerIn)
        {
            return (ownerIn.targetEntity != null)
                && ownerIn.targetEntity.enabled
                && (ownerIn.targetObject != null)
                && ownerIn.targetObject.activeInHierarchy;
        }

        public override IBehaviorState NextState(EntityMob ownerIn)
            => EmptyState.Instance.NextState(ownerIn);

        public override bool StateUpdate(EntityMob ownerIn)
        {
            bool output = IsValidState(ownerIn);
            if (output)
            {
                EntityZombie zombie = ownerIn as EntityZombie;
                StepDataAI nextStep = ownerIn.Manager.GetAIDataForGround(ownerIn.transform.position,
                    ownerIn.transform.position + ownerIn.DesiredDirection, zombie);
                float sqDistToTaget = ownerIn.DistanceSqrToTarget();
                if ((ownerIn.targetEntity != null) && (ownerIn.CanSeeTarget() || (sqDistToTaget < 144f)))
                {
                    if (sqDistToTaget < 16f)
                    {
                        zombie.Destination = zombie.targetObject.transform.position;
                        zombie.TurnToDestination();
                    }
                    else if (zombie.ShouldTurn || !nextStep.Desireable ||
                        (zombie.WanderUpdateTime < Time.time))
                    {
                        zombie.Destination = zombie.targetObject.transform.position;
                        zombie.TurnToDestination();
                        zombie.WanderUpdateTime = Time.time + 2f + (Random.value * 2f);
                        zombie.DesiredDirection = AIHelper.FindTurnDirection() * zombie.DesiredDirection;
                        zombie.ShouldTurn = false;
                    }
                }
                // If the target is not visible, just wander util it can be seen again
                else if(zombie.ShouldTurn || !nextStep.Desireable || (zombie.WanderUpdateTime < Time.time))
                {
                    zombie.WanderUpdateTime = Time.time + 2f + (Random.value * 2f);
                    zombie.DesiredDirection = AIHelper.FindTurnDirection() * zombie.DesiredDirection;
                    zombie.ShouldTurn = false;
                }
                zombie.FaceHeading();
            }
            return true;
        }



        //public override void StateFixedUpdate(EntityMob ownerIn) { }
        // public override void StateLateUpdate(EntityMob ownerIn) { }

    }

}