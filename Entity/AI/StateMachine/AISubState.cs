using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CevarnsOfEvil {


    public abstract class AISubState : AIState
    {      
        protected AIState parent;


        public override AIState Instantiate(EntityMob mob)
        {
            AISubState state = (AISubState)base.Instantiate(mob);
            state.parent = parent;
            return state;
        }
        // Will wrap constructor, if the relective method doesn't work.
        // protected abstract AIState Make(); 

    }

}
