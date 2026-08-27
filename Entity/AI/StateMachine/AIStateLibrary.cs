using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CevarnsOfEvil {


    public enum AIStateID
    {
        
    }


    // Sub-States should not need to be registered here, 
    // since they will be create with their parent state.


    public static class AIStateLibrary
    {
        private static Dictionary<AIStateID, AIState> states = new();
        private static Dictionary<AIStateID, AISubState> substates = new();


        static AIStateLibrary() 
        {
            RegisterStates();            
        }


        private static void RegisterStates()
        {
            
        }


        public static AIState GetState(AIStateID id, EntityMob mob) 
                            => states[id].Instantiate(mob); 
        

    }


}
