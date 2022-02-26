using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace DLD {

    public abstract class BehaviorObject : ScriptableObject, IBehaviorState
    {
        [SerializeField] protected float AnimMoveSpeed;
        [SerializeField] protected int   AnimID;

        public virtual bool IsValidState(EntityMob ownerIn) => false; // must be overriden!

        public virtual IBehaviorState NextState(EntityMob ownerIn) 
            => EmptyState.Instance.NextState(ownerIn);

        public virtual void StateEnter(EntityMob ownerIn) { }

        public virtual void StateExit(EntityMob ownerIn) { }

        public virtual void StateFixedUpdate(EntityMob ownerIn) { }

        public virtual void StateLateUpdate(EntityMob ownerIn) { }

        public virtual bool StateUpdate(EntityMob ownerIn) => false;
    }
}