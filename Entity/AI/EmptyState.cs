using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CevarnsOfEvil
{


    public class EmptyState : IBehaviorState
    {
        public static readonly EmptyState Instance = new EmptyState();

        public void StateFixedUpdate(EntityMob ownerIn) {}

        public bool IsValidState(EntityMob ownerIn) => true;

        public void StateLateUpdate(EntityMob ownerIn) {}

        public IBehaviorState NextState(EntityMob ownerIn) {
            return ownerIn.CurrentBehavior;
        }

        public bool StateUpdate(EntityMob ownerIn) => false;

        public void StateEnter(EntityMob ownerIn) {}

        public void StateExit(EntityMob ownerIn) {}
    }

}