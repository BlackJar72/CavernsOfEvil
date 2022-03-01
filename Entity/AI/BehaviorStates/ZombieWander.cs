using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CevarnsOfEvil
{


    [CreateAssetMenu(menuName = "DLD/AI/Zombie Wander", fileName = "ZombieWander", order = 20)]
    public class ZombieWander : BehaviorObject
    {
        public override bool IsValidState(EntityMob ownerIn) => true;

        public virtual IBehaviorState NextState(EntityMob ownerIn)
            => EmptyState.Instance.NextState(ownerIn);

        public override bool StateUpdate(EntityMob ownerIn)
        {

            return true;
        }

        public override void StateEnter(EntityMob ownerIn) 
        { 
        
        }


        public override void StateExit(EntityMob ownerIn) 
        { 
        
        }



        //public override void StateFixedUpdate(EntityMob ownerIn) { }
        // public override void StateLateUpdate(EntityMob ownerIn) { }

    }

}