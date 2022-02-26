using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace DLD
{
    [CreateAssetMenu(menuName = "DLD/AI/Charge Player", fileName = "ChargePlayer", order = 3)]
    public class ChargePlayer : BehaviorObject
    {
        public BehaviorObject trackState;

        public override void StateEnter(EntityMob ownerIn)
        {
            ownerIn.DisableNavmesh();
            ownerIn.SetFactorSpeed(AnimMoveSpeed);
        }


        public override bool StateUpdate(EntityMob ownerIn)
        {
            bool output = IsValidState(ownerIn);
            if(output && !(ownerIn.InMeleeRange() || ownerIn.InStopingRange()) 
                && (ownerIn.NextAttack < Time.time))
            {
                ownerIn.SetDestination(ownerIn.targetObject.transform.position);
                ownerIn.StartTurnToDestination();
                ownerIn.SetMovement();
                ownerIn.FaceHeading();
            }
            else
            {
                ownerIn.SetDirectionZero();
            }
            return output;
        }


        public override bool IsValidState(EntityMob ownerIn)
        {
            return ((ownerIn != null) && ownerIn.CanSeeTarget() 
                && ownerIn.LineToTargetClear());
        }


        public override IBehaviorState NextState(EntityMob ownerIn)
        {
            return trackState;
        }

    }

}