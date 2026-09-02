using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CevarnsOfEvil {


    public abstract class AIState 
    {
        protected EntityMob owner = null;
        public EntityMob Owner => owner;


        public virtual AIState Init(EntityMob mob)
        {
            if (owner == null) { owner = mob; }
            else
            {
                Debug.LogError("Trying to call init more that once on the same AIState!");
                throw new System.Exception("Trying to call init more that once ona same AIState!");
            }
            return this;
        }


        public virtual AIState Instantiate(EntityMob mob)
        {
            // Make().Init(mob); // Lets see if we can do this with reflection instead.
            object result = Activator.CreateInstance(GetType());
            return (result as AIState).Init(mob);
        }
        // Will wrap constructor, if the relective method doesn't work.
        // protected abstract AIState Make(); 


        public abstract void StateEnter();
        public abstract void StateExit();
        public abstract void Pause();
        public abstract void Resume();


#region public abstract void Act();

    
        public virtual void StateFixedUpdate() { }
        public virtual void StateLateUpdate() { }
        public virtual bool StateUpdate() => false;
        public void Act() => StateUpdate();


#endregion        

    }


}