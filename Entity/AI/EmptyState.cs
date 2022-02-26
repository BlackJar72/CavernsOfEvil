using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace DLD
{


    public class EmptyState : IBehaviorState
    {
        public static readonly EmptyState Instance = new EmptyState();

        public void StateFixedUpdate(EntityMob ownerIn) {}

        public bool IsValidState(EntityMob ownerIn) => true;

        public void StateLateUpdate(EntityMob ownerIn) {}

        public IBehaviorState NextState(EntityMob ownerIn) {
            int choice = -1;
            int stop = ownerIn.Behaviors.Length - 1;
            bool found = false;
            while (!found && (choice < stop))
            {
                choice++;
                found = ownerIn.Behaviors[choice].IsValidState(ownerIn);
            }
            if (found) return ownerIn.Behaviors[choice]; 
            return ownerIn.CurrentBehavior;
        }

        public bool StateUpdate(EntityMob ownerIn) => false;

        public void StateEnter(EntityMob ownerIn) {}

        public void StateExit(EntityMob ownerIn) {}
    }

}