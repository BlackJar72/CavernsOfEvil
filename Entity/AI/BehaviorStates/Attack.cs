using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace DLD
{
    [CreateAssetMenu(menuName = "DLD/AI/Generic Attack", fileName = "Attack", order = 4)]
    public class Attack : BehaviorObject
    {
        public override bool IsValidState(EntityMob ownerIn)
        {
            return ownerIn.StasisAI > Time.time;
        }


        public override bool StateUpdate(EntityMob ownerIn)
        {
            if(!IsValidState(ownerIn))
            {
                ownerIn.CurrentBehavior = ownerIn.PreviousBehavior;
                return true;
            }
            return true;
        }


        public override void StateEnter(EntityMob ownerIn)
        {
            ownerIn.SetFactorSpeed(AnimMoveSpeed);
            ownerIn.Attack();
        }
    }

}