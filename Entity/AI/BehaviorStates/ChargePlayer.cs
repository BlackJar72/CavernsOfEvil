using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CevarnsOfEvil
{
    [CreateAssetMenu(menuName = "DLD/AI/Charge Player", fileName = "ChargePlayer", order = 3)]
    public class ChargePlayer : BehaviorObject
    {
        public BehaviorObject trackState;

        public override void StateEnter(EntityMob ownerIn)
        {

        }


        public override bool StateUpdate(EntityMob ownerIn)
        {
            bool output = IsValidState(ownerIn);
            if(output && !(ownerIn.InMeleeRange()) 
                && (ownerIn.NextAttack < Time.time))
            {
                ownerIn.Destination = ownerIn.targetObject.transform.position;
                ownerIn.TurnToDestination();
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
            return ((ownerIn != null) && ownerIn.CanSeeTarget());
        }


        public override IBehaviorState NextState(EntityMob ownerIn)
        {
            return trackState;
        }

    }

}